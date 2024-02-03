namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class LogicalNotExpressionSyntax: ExpressionSyntax
{
    internal LogicalNotExpressionSyntax(ExpressionSyntax expression) =>
        Expression = expression;
    public ExpressionSyntax Expression { get; }
    public override SyntaxKind Kind => SyntaxKind.LogcalNotExpression;
    public override string ToString() => "!" + Expression;
}