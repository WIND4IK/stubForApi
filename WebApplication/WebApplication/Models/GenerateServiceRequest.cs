namespace WebApplication1.Models
{
    public class GenerateServiceRequest
    {
        public string UserId { get; set; }
        public required string ServiceName { get; set; }
        public List<CustomType> CustomTypes { get; set; }
        public List<BellaBlock> FlowBlocks { get; set; }
        public List<Variable> Variables { get; set; }
    }
}
