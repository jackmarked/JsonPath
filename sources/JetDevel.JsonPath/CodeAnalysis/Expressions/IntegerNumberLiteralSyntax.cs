namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class IntegerNumberLiteralSyntax: LiteralExpressionSyntax<long>
{
    internal IntegerNumberLiteralSyntax(Token token) : base(long.Parse(token.Text)) { }
    public override SyntaxKind Kind => SyntaxKind.IntegerNumberLiteral;
}