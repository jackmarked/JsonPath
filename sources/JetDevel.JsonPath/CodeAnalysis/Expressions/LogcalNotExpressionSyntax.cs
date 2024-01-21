namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class LogcalNotExpressionSyntax: ExpressionSyntax
{
    internal LogcalNotExpressionSyntax(ExpressionSyntax expression)
    {
        Expression = expression;
    }
    public ExpressionSyntax Expression { get; }
    public override SyntaxKind Kind => SyntaxKind.LogcalNotExpression;
    public override string ToString()
    {
        return "!" + Expression;
    }
}