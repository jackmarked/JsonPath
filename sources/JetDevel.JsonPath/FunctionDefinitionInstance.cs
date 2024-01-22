namespace JetDevel.JsonPath;
public sealed class FunctionDefinitionInstance: FunctionDefinition
{
    public FunctionDefinitionInstance(string name, FunctionParameterType returnType, Func<IReadOnlyList<ExpressionValue>, FunctionExecutionContext, ExpressionValue> body,
        IReadOnlyList<FunctionParameterType> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(body);
        if(!IsValidType(returnType))
            throw new ArgumentException("Invalid return type value.", nameof(returnType));
        var functionParameters = new List<FunctionParameterType>();
        for(var i = 0; i < parameters.Count; i++)
            if(!IsValidType(parameters[i]))
                throw new ArgumentException($"Invalid parameter type with index {i}.", nameof(parameters));
            else
                functionParameters.Add(parameters[i]);
        Name = name;
        ResultType = returnType;
        Parameters = functionParameters.AsReadOnly();
        Body = body;
    }
    public override ExpressionValue Execute(IReadOnlyList<ExpressionValue> arguments, FunctionExecutionContext context)
    {
        return Body(arguments, context);
    }
    private static bool IsValidType(FunctionParameterType returnType)
    {
        return returnType == FunctionParameterType.Value ||
                    returnType == FunctionParameterType.Logical ||
                    returnType == FunctionParameterType.Nodes;
    }

    public override string Name { get; }
    public override FunctionParameterType ResultType { get; }
    public override IReadOnlyList<FunctionParameterType> Parameters { get; }
    Func<IReadOnlyList<ExpressionValue>, FunctionExecutionContext, ExpressionValue> Body { get; }
}