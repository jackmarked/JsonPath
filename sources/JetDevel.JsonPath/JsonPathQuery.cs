using System.Text;
using System.Text.Json;
using JetDevel.JsonPath.CodeAnalysis;

namespace JetDevel.JsonPath;

public abstract class JsonPathQuery
{
    protected private JsonPathQuery() { }
    public abstract JsonDocument Execute(JsonDocument document, CancellationToken cancellationToken = default);
    public static JsonPathQuery FromSyntax(JsonPathQuerySyntax syntax)
    {
        ArgumentNullException.ThrowIfNull(syntax);
        return new SyntaxBasedJsonPathQuery(syntax);
    }
    public static JsonPathQuery FromSource(string source)
    {
        var bytes = Encoding.UTF8.GetBytes(source);
        return FromUtf8(bytes);
    }
    public static JsonPathQuery FromUtf8(ReadOnlySpan<byte> utf8Bytes)
    {
        var lexer = new Lexer(utf8Bytes);
        Parser parser = new Parser(lexer);
        return FromSyntax(parser.ParseQuery());
    }
}