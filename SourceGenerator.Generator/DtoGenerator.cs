using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SourceGenerator.Generator
{
    [Generator]
    public class DtoGenerator : ISourceGenerator
    {
        private readonly string generatedNameSpace = "GeneratedMappers";
        private readonly string hintNamePrefix = "MapperGenerator";
        private readonly string defaultClassNameSuffix = "Dto";

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            foreach (var syntaxTree in syntaxTrees)
            {
                var descendantNodes = syntaxTree.GetRoot().DescendantNodes();

                var mappableTypeDeclarations = descendantNodes.OfType<TypeDeclarationSyntax>()
                    .Where(x => x.AttributeLists.Any(x => x.ToString().StartsWith("[Mappable"))).ToList();

                foreach (var mappableTypeDeclaration in mappableTypeDeclarations)
                {
                    var usingDirectivesAsText = GetFilteredUsingDìrectives(descendantNodes);
                    var sourceBuilder = new StringBuilder(usingDirectivesAsText);

                    var className = mappableTypeDeclaration.Identifier.ToString();

                    var generatedClassNameSuffix = GetGeneratedClassNameSuffix(mappableTypeDeclaration);
                    var generatedClassName = $"{className}{generatedClassNameSuffix}";

                    var filteredMappableTypeDeclarations = GetFilteredMappableTypeDeclarations(mappableTypeDeclaration);

                    var splitClass = filteredMappableTypeDeclarations.ToString().Split(['{'], 2);

                    sourceBuilder.Append($@"
namespace {generatedNameSpace}
{{
    public class {generatedClassName}
    {{
");

                    sourceBuilder.AppendLine(splitClass[1].Replace(className, generatedClassName));
                    sourceBuilder.AppendLine("}");
                    context.AddSource($"{hintNamePrefix}_{generatedClassName}", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }
            }
        }

        /// <summary>
        /// Filter out using directives properties
        /// </summary>
        /// <param name="syntaxNodes"></param>
        /// <returns></returns>
        private static string GetFilteredUsingDìrectives(IEnumerable<SyntaxNode> syntaxNodes)
        {
            //filter out SourceGenerator.Domain.Attributes
            var usingDirectives = syntaxNodes.OfType<UsingDirectiveSyntax>();
                //.Where(x => !x.GetText().Container.CurrentText.ToString().Contains("SourceGenerator.Domain.Attributes"));
            //.DefaultIfEmpty();

            var usingDirectivesAsText = string.Join("\r\n", usingDirectives);

            return usingDirectivesAsText;
        }

        /// <summary>
        /// Filter out MappableIgnore properties
        /// </summary>
        /// <param name="mappableTypeDeclaration"></param>
        /// <returns></returns>
        private static TypeDeclarationSyntax GetFilteredMappableTypeDeclarations(TypeDeclarationSyntax mappableTypeDeclaration)
        {
            var ignoredProperties = mappableTypeDeclaration.ChildNodes().OfType<PropertyDeclarationSyntax>()
                .Where(x => x.AttributeLists.Any(x => x.ToString().StartsWith("[MappableIgnore]")));

            var filteredMappableTypeDeclarations = mappableTypeDeclaration.RemoveNodes(ignoredProperties, SyntaxRemoveOptions.KeepEndOfLine);

            return filteredMappableTypeDeclarations;
        }

        /// <summary>
        /// Generate suffix based of given suffix in Mappable attribute, defaults to "Dto"
        /// </summary>
        /// <param name="mappableTypeDeclaration"></param>
        /// <returns></returns>
        private string GetGeneratedClassNameSuffix(TypeDeclarationSyntax mappableTypeDeclaration)
        {
            var mappableAttributeArgumentList = mappableTypeDeclaration
                .AttributeLists
                .Single(x => x.ToString().StartsWith("[Mappable"))
                .Attributes
                .Single()
                .ArgumentList;

            if (mappableAttributeArgumentList == null)
            {
                return defaultClassNameSuffix;
            }

            var suffix = mappableAttributeArgumentList
                .Arguments
                .Select(x => x.Expression)
                .Single()
                .ToString()
                .Trim(['"']);

            var generatedClassNameSuffix = string.IsNullOrWhiteSpace(suffix) ? defaultClassNameSuffix : suffix;

            return generatedClassNameSuffix;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif  
        }
    }
}