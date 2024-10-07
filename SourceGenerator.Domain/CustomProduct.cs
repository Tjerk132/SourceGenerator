using SourceGenerator.Domain.Attributes;

namespace SourceGenerator.Domain
{
    [Mappable(Suffix = "Record")]
    public class CustomProduct
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MappableIgnore]
        public string Description { get; set; }
    }
}