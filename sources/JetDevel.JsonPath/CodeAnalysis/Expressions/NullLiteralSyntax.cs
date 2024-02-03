namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class NullLiteralSyntax : LiteralExpressionSyntax<object>
{
    internal NullLiteralSyntax() : base(null!) { }
    public override SyntaxKind Kind => SyntaxKind.NullLiteral;
}