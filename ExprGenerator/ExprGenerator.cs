using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExprGenerator
{
    [Generator]
    public class TestGenerator: IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initialContext)
        {
            var expressions = initialContext.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is RecordDeclarationSyntax cds &&
                                    cds.BaseList?.Types.Any(type => type.ToString() == "Expr") == true,
                static (node, _) => (RecordDeclarationSyntax)node.Node);

            initialContext.RegisterSourceOutput(expressions, (context, node) =>
            {
                context.AddSource($"{node.Identifier.ValueText}Expr.g.cs", $@"
namespace LoxCS;

public partial interface IVisitor<out T>
{{
    public T Visit({node.Identifier} expr);
}}

public partial record {node.Identifier}
{{
    public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
}}
");
            });
        }
    }
}

