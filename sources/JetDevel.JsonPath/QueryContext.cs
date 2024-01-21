using System.Text.Json;

namespace JetDevel.JsonPath;

sealed class QueryContext
{
    public QueryContext(JsonElement root, CancellationToken cancellationToken)
    {
        Root = root;
        CancellationToken = cancellationToken;
    }
    public JsonElement Root { get; }
    public CancellationToken CancellationToken { get; }
}