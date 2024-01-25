namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class NameSelectorSyntax: SelectorSyntax
{
    internal NameSelectorSyntax(string name)
    {
        Name = name;
    }
    public string Name { get; }
    public override string ToString() => Name;
    public override SyntaxKind Kind => SyntaxKind.NameSelector;
}