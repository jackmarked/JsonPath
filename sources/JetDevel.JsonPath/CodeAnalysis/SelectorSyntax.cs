namespace JetDevel.JsonPath.CodeAnalysis;

public abstract class SelectorSyntax: JsonPathSyntaxNode
{
    protected private SelectorSyntax() { }
    public override SyntaxKind Kind => SyntaxKind.Unknown;
}