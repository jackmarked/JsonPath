namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class StringLiteralSyntax: LiteralExpressionSyntax<string>
{
    internal StringLiteralSyntax(Token readToken) : base(SyntaxFacts.GetStringLiteralValue(readToken.Text))
    {
    }
    public override SyntaxKind Kind => SyntaxKind.Unknown;
    public override string ToString()
    {
        return @"""" + Value + @"""";
    }
}