using JetDevel.JsonPath.CodeAnalysis;

namespace JetDevel.JsonPath;

public static class SyntaxFactory
{
    public static JsonPathQuerySyntax Parse(string s)
    {
        var lexer = new Lexer(s);
        var parser = new Parser(lexer);
        var query = parser.ParseQuery();
        return query;
    }
}