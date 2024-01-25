namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class BooleanLiteralSyntax: LiteralExpressionSyntax<bool>
{
    internal BooleanLiteralSyntax(bool value) : base(value) { }
    public override SyntaxKind Kind => SyntaxKind.BooleanLiteral;
}
