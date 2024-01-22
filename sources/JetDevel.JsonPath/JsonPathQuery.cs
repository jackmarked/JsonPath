using System.Text.Json;

namespace JetDevel.JsonPath;

public abstract class JsonPathQuery
{
    protected JsonPathServices Services { get; }

    protected private JsonPathQuery(JsonPathServices services)
    {
        Services = services;
    }
    public JsonDocument Execute(JsonDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        return ExecuteCore(document, cancellationToken);
    }
    protected private abstract JsonDocument ExecuteCore(JsonDocument document, CancellationToken cancellationToken = default);
}