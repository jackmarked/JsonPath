namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class StringLiteralSyntax: LiteralExpressionSyntax<string>
{
    internal StringLiteralSyntax(Token token) : base(SyntaxFacts.GetStringLiteralValue(token.Text)) { }
    public override SyntaxKind Kind => SyntaxKind.StringLiteral;
    public override string ToString() => @"""" + Value + @"""";
}