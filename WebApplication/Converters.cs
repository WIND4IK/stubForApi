using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication
{
    public class TypeJsonConverter<TBase> : JsonConverter<TBase>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TBase) == objectType;
        }

        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
        {
            ExpandoObject expando = ToExpandoObject(value);

            JsonSerializer.Serialize(writer, expando, options);
        }

        private static Dictionary<Type, List<PropertyInfo>> Getters = new Dictionary<Type, List<PropertyInfo>>();
        private static object MainLock = new object();


        private static ExpandoObject ToExpandoObject(object obj)
        {
            var expando = new ExpandoObject();
            if (obj != null)
            {
                var type = obj.GetType();
                if (!Getters.ContainsKey(type))
                {
                    lock (MainLock)
                    {
                        if (!Getters.ContainsKey(type))
                        {
                            Getters.Add(type, type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead).ToList());
                        }
                    }
                }
                var properties = Getters[type];

                foreach (var property in properties)
                {
                    expando.TryAdd(property.Name, property.GetValue(obj));
                }
            }

            return expando;
        }

        public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            var optionsNew = new JsonSerializerOptions();
            optionsNew.Converters.Add(new DateTimeConverterUsingDateTimeParse());
            optionsNew.Converters.Add(new DoubleConverter());
            optionsNew.Converters.Add(new IntConverter());
            optionsNew.Converters.Add(new StringConverter());
            return (TBase)JsonSerializer.Deserialize(ref reader, typeToConvert, optionsNew);
        }
    }
    public class InheritedTypeJsonConverter<TBase> : TypeJsonConverter<TBase>
    {
        private static IEnumerable<TypeHolder> _originalTypeHolders;
        private const string TypeDiscriminator = "__type";
        static InheritedTypeJsonConverter()
        {
            var classes = Assembly.GetAssembly(typeof(TBase)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TBase))).ToList();
            classes.Add(typeof(TBase));
            _originalTypeHolders = classes.Select(c => new TypeHolder { Type = c, Properties = c.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase) }).ToList().Select(t => t);
        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(TBase) == objectType;
        }

        public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            var originReader = reader;
            var depth = originReader.CurrentDepth + 1;
            var typeHolders = _originalTypeHolders.ToList();
            while (originReader.Read())
            {
                var valueSpan = originReader.ValueSpan;

                if (originReader.TokenType == JsonTokenType.EndObject && originReader.CurrentDepth == depth - 1)
                    break;
                if (originReader.TokenType == JsonTokenType.PropertyName && originReader.CurrentDepth == depth)
                {
                    var propertyName = new string(valueSpan.ToArray().Select(x => (char)x).ToArray());
                    if (propertyName == TypeDiscriminator)
                    {
                        originReader.Read();
                        var typeName = originReader.GetString();
                        typeHolders = typeHolders.Where(t => t.Type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)).Take(1).ToList();
                        break;
                    }
                    foreach (var typeHolder in typeHolders.ToList().Where(typeHolder => !typeHolder.Properties.Contains(propertyName)))
                    {
                        typeHolders.Remove(typeHolder);
                    }
                }
            }

            if (typeHolders.Count > 1)
                throw new AmbiguousMatchException("Could not find correct type. Please specify more fields or type field");
            if (typeHolders.Count == 0)
                throw new TypeNotFoundException("Could not find correct type");
            var type = typeHolders.Single().Type;

            //Does not work with base class
            return (TBase)JsonSerializer.Deserialize(ref reader, type, options);
        }
    }

    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return DateTime.MinValue;
            return DateTime.Parse(reader.GetString());
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToIsoString());
        }
    }

    public class DoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return 0;
            return reader.TokenType == JsonTokenType.Number ? JsonSerializer.Deserialize(ref reader, doubleContext.Default.Double) : double.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(JsonSerializer.Serialize(value, doubleContext.Default.Double));
            //writer.WriteNumberValue(value);
        }
    }
    [JsonSerializable(typeof(double))]
    public partial class doubleContext : JsonSerializerContext
    {
    }

    public class StringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            else if (reader.TokenType == JsonTokenType.Number)
            {
                double dValue = 0;
                if (reader.TryGetDouble(out dValue))
                    return reader.GetDouble().ToString();
                else
                    return reader.GetInt32().ToString();
            }
            else
            {
                return reader.GetString();
            }

        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    public class IntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return 0;
            return reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : int.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string message) : base(message)
        {
        }
    }

    struct TypeHolder
    {
        public Type Type { get; set; }
        public HashSet<string> Properties { get; set; }
    }

    public static class DateTimeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToIsoString(this DateTime dt)
        {
            Span<char> chars = stackalloc char[24];
            Write4Chars(chars, 0, dt.Year);
            chars[4] = '-';
            Write2Chars(chars, 5, dt.Month);
            chars[7] = '-';
            Write2Chars(chars, 8, dt.Day);
            chars[10] = 'T';
            Write2Chars(chars, 11, dt.Hour);
            chars[13] = ':';
            Write2Chars(chars, 14, dt.Minute);
            chars[16] = ':';
            Write2Chars(chars, 17, dt.Second);
            chars[19] = '.';
            Write3Chars(chars, 20, dt.Millisecond);
            chars[23] = 'Z';
            return new string(chars);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Write3Chars(Span<char> chars, int offset, int value)
        {
            chars[offset] = ToCharDigit(value / 100);
            chars[offset + 1] = ToCharDigit(value / 10 % 10);
            chars[offset + 2] = ToCharDigit(value % 10);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Write4Chars(Span<char> chars, int offset, int value)
        {
            chars[offset] = ToCharDigit(value / 1000);
            chars[offset + 1] = ToCharDigit(value / 100 % 10);
            chars[offset + 2] = ToCharDigit(value / 10 % 10);
            chars[offset + 3] = ToCharDigit(value % 10);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Write2Chars(Span<char> chars, int offset, int value)
        {
            chars[offset] = ToCharDigit(value / 10);
            chars[offset + 1] = ToCharDigit(value % 10);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char ToCharDigit(int value)
        {
            return (char)(value + '0');
        }
    }
}
