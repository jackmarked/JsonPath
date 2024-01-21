using System.Text.Json;

namespace JetDevel.JsonPath;

public sealed class ExpressionEvaluationContext
{
    internal ExpressionEvaluationContext(JsonElement root, JsonElement current, CancellationToken cancellationToken)
    {
        Root = root;
        CancellationToken = cancellationToken;
        Current = current;
    }
    public JsonElement Root { get; }
    public CancellationToken CancellationToken { get; }
    public JsonElement Current { get; }
}