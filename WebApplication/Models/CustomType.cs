using System.Reflection;

namespace WebApplication1.Models
{
    public class CustomType
    {
        public string Name { get; set; }
        public List<TypeField> Fields{ get; set; }
        public bool IsPersistent { get; set; }
    }

    public class TypeField
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
