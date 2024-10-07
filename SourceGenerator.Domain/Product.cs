using SourceGenerator.Domain.Attributes;

namespace SourceGenerator.Domain
{
    [Mappable]
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [MappableIgnore]
        public string Description { get; set; }
    }
}