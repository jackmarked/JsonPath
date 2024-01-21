namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class FlterSelectorSyntax: SelectorSyntax
{
    internal FlterSelectorSyntax(ExpressionSyntax expression)
    {
        Expression = expression;
    }
    public ExpressionSyntax Expression { get; }
    public override string ToString()
    {
        return "?" + Expression?.ToString();
    }
}