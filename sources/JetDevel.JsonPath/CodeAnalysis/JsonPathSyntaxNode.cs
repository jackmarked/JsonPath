namespace JetDevel.JsonPath.CodeAnalysis;

public abstract class JsonPathSyntaxNode
{
    private protected JsonPathSyntaxNode() { }
    public abstract SyntaxKind Kind { get; }
}