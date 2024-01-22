namespace JetDevel.JsonPath;
public sealed class FunctionExecutionContext
{
    public FunctionExecutionContext(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }
    public CancellationToken CancellationToken { get; }
}