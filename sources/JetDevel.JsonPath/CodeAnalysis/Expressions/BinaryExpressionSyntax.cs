namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class BinaryExpressionSyntax: ExpressionSyntax
{
    Token operatorToken;
    internal BinaryExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, Token operatorToken)
    {
        Left = left;
        Right = right;
        Kind = SyntaxFacts.GetBinaryExpressionKind(operatorToken);
        this.operatorToken = operatorToken;
    }
    public ExpressionSyntax Left { get; }
    public ExpressionSyntax Right { get; }
    public override SyntaxKind Kind { get; }

    public override string ToString() =>
        Left + operatorToken.Text + Right;
}