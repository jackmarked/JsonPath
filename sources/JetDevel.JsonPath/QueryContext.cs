using System.Text.Json;

namespace JetDevel.JsonPath;

sealed class QueryContext
{
    public QueryContext(JsonElement root, JsonPathServices services, CancellationToken cancellationToken)
    {
        Root = root;
        CancellationToken = cancellationToken;
        Services = services;
    }
    public JsonElement Root { get; }
    public CancellationToken CancellationToken { get; }
    public JsonPathServices Services { get; }
}