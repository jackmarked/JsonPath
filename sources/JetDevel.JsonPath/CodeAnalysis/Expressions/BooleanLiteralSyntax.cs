namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class BooleanLiteralSyntax: LiteralExpressionSyntax<bool>
{
    internal BooleanLiteralSyntax(bool value) : base(value) { }
}
