namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class WildcardSelectorSyntax: SelectorSyntax
{
    internal WildcardSelectorSyntax() { }
    public override SyntaxKind Kind => SyntaxKind.WildcardSelector;
    public override string ToString() => "*";
}