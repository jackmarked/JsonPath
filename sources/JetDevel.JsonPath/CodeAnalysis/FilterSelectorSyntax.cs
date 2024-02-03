namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class FilterSelectorSyntax: SelectorSyntax
{
    internal FilterSelectorSyntax(ExpressionSyntax expression) => Expression = expression;
    public ExpressionSyntax Expression { get; }
    public override SyntaxKind Kind => SyntaxKind.FilterSelector;
    public override string ToString() => "?" + Expression?.ToString();
}