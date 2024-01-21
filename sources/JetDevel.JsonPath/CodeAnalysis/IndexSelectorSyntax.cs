namespace JetDevel.JsonPath.CodeAnalysis;

public class IndexSelectorSyntax: SelectorSyntax
{
    public IndexSelectorSyntax(string text)
    {
        Index = int.Parse(text);
    }
    public int Index { get; }
    public override string ToString()
    {
        return Index.ToString();
    }
}