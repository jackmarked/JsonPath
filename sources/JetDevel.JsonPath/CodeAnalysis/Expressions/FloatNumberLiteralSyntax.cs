using System.Globalization;

namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class FloatNumberLiteralSyntax: LiteralExpressionSyntax<double>
{
    internal FloatNumberLiteralSyntax(Token token) : base(double.Parse(token.Text, CultureInfo.InvariantCulture))
    {
    }

}