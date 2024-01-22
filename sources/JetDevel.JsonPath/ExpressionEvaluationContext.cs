using System.Text.Json;

namespace JetDevel.JsonPath;

public sealed class ExpressionEvaluationContext
{
    internal ExpressionEvaluationContext(JsonElement root, JsonElement current, JsonPathServices services, CancellationToken cancellationToken)
    {
        Root = root;
        CancellationToken = cancellationToken;
        Current = current;
        Services = services;
    }
    public JsonElement Root { get; }
    public CancellationToken CancellationToken { get; }
    public JsonPathServices Services { get; }
    public JsonElement Current { get; }
}