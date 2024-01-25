using JetDevel.JsonPath.CodeAnalysis;
using System.Text;

namespace JetDevel.JsonPath;
public sealed partial class JsonPathServices
{
    readonly Dictionary<string, FunctionDefinition> functionMap;
    public JsonPathServices()
    {
        functionMap = [];
        RegisterFunctionDefinition(KnownFunctions.Length);
        RegisterFunctionDefinition(KnownFunctions.Count);
        RegisterFunctionDefinition(KnownFunctions.Value);
        RegisterFunctionDefinition(KnownFunctions.Match);
        RegisterFunctionDefinition(KnownFunctions.Search);
    }
    void RegisterFunctionDefinition(FunctionDefinition function)
    {
        ArgumentNullException.ThrowIfNull(function);
        functionMap[function.Name] = function;
    }
    internal FunctionDefinition? GetFunction(string functionName)
    {
        if(string.IsNullOrEmpty(functionName))
            return null;
        if(functionMap.TryGetValue(functionName, out var fuction))
            return fuction;
        return null;
    }
    public JsonPathQuery FromSource(string source)
    {
        var bytes = Encoding.UTF8.GetBytes(source);
        return FromUtf8(bytes);
    }
    public bool TryParse(ReadOnlySpan<byte> utf8Bytes, out JsonPathQuerySyntax? query)
    {
        var charReader = new Utf8BytesUnicodeCharacterReader(utf8Bytes.ToArray());
        var lexer = new Lexer(charReader);
        Parser parser = new Parser(lexer);
        var result = parser.ParseQuery();
        query = result.JsonPathQuery;
        if(query == null)
            return false;
        return true;
    }
    public JsonPathQuery FromUtf8(ReadOnlySpan<byte> utf8Bytes)
    {
        var charReader = new Utf8BytesUnicodeCharacterReader(utf8Bytes.ToArray());
        var lexer = new Lexer(charReader);
        Parser parser = new Parser(lexer);
        return FromSyntax(parser.ParseQuery().JsonPathQuery!);
    }
    public JsonPathQuery FromSyntax(JsonPathQuerySyntax syntax)
    {
        ArgumentNullException.ThrowIfNull(syntax);
        return new SyntaxBasedJsonPathQuery(syntax, this);
    }
}