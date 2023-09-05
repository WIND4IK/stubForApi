using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    [JsonDerivedType(typeof(RequestBlock))]
    //[JsonDerivedType(typeof(VariableBlock))]
    [JsonDerivedType(typeof(ExternalCallBlock))]
    [JsonDerivedType(typeof(LoopBlock))]
    [JsonDerivedType(typeof(ObjectBlock))]
    [JsonDerivedType(typeof(ResponseBlock))]
    [JsonDerivedType(typeof(RequestDetailsBlock))]
    public class BellaBlock
    {
        public string __type { get; set; }
    }

    public class RequestBlock : BellaBlock
    {
        public string RequestType { get; set; }
    }

    //public class VariableBlock : BellaBlock
    //{
    //    public string Name { get; set; }
    //    public string Type { get; set; }
    //}

    public class ExternalCallBlock : BellaBlock
    {
        public string Url { get; set; }
        public string ResponseType { get; set; }
        public string RequestType { get; set; }
        public string Body { get; set; }
        public List<BellaBlock> BodyBlocks { get; set; }
    }

    public class LoopBlock : BellaBlock
    {
        public string Loop { get; set; }
        public string Body { get; set; }
    }

    public class ObjectBlock : BellaBlock
    {
        public string Type { get; set; }
        public string Body { get; set; }
    }

    public class ResponseBlock : BellaBlock
    {
        public string Response { get; set; }
    }

    public class RequestDetailsBlock : BellaBlock
    {
        public List<string> Details { get; set; }
    }
}
