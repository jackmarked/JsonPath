namespace JetDevel.JsonPath.CodeAnalysis;
public sealed class ParserResult
{
    internal ParserResult(IReadOnlyList<string> errors)
    {
        Errors = errors;
    }
    internal ParserResult(JsonPathQuerySyntax jsonPathQuery)
    {
        JsonPathQuery = jsonPathQuery;
        Errors = [];
    }

    public JsonPathQuerySyntax? JsonPathQuery { get; }
    public IReadOnlyList<string> Errors { get; }
}
