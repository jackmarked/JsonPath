namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class IndexSelectorSyntax: SelectorSyntax
{
    internal IndexSelectorSyntax(string text)
    {
        Index = int.Parse(text);
    }
    public int Index { get; }
    public override string ToString()
    {
        return Index.ToString();
    }
    public override SyntaxKind Kind => SyntaxKind.IndexSelector;
}