namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class ParenthesizedExpressionSyntax: ExpressionSyntax
{
    internal ParenthesizedExpressionSyntax(ExpressionSyntax expression) =>
        Expression = expression;
    public ExpressionSyntax Expression { get; }
    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
    public override string ToString() => "(" + Expression + ")";
}