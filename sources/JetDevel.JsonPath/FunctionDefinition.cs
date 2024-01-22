namespace JetDevel.JsonPath;
public abstract class FunctionDefinition
{
    public abstract string Name { get; }
    public abstract FunctionParameterType ResultType { get; }
    public abstract IReadOnlyList<FunctionParameterType> Parameters { get; }
    public abstract ExpressionValue Execute(IReadOnlyList<ExpressionValue> arguments, FunctionExecutionContext context);
}
