namespace SourceGenerator.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MappableAttribute : Attribute
    {
        public MappableAttribute(string suffix = "")
        {
            Suffix = suffix;
        }

        public string Suffix { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MappableIgnoreAttribute : Attribute
    { }
}
