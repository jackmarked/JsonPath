using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace JetDevel.JsonPath;
static class KnownFunctions
{
    public static readonly FunctionDefinition Length = new FunctionDefinitionInstance("length", FunctionParameterType.Value, LengthBody, [FunctionParameterType.Value]);
    static ExpressionValue LengthBody(IReadOnlyList<ExpressionValue> arguments, FunctionExecutionContext context)
    {
        static long GetStringLength(string s) => Encoding.UTF32.GetByteCount(s) / 4;
        if(arguments is null or [])
            return ExpressionValue.Nothing;
        var argument = arguments[0];

        if(argument.ValueType == ValueType.Node)
        {
            var jsonElement = argument.AsJson();
            if(jsonElement.ValueKind == JsonValueKind.Array)
                return new(jsonElement.GetArrayLength());
            if(jsonElement.ValueKind == JsonValueKind.Object)
                return new(jsonElement.EnumerateObject().LongCount());
            if(jsonElement.ValueKind == JsonValueKind.String)
                return new(GetStringLength(jsonElement.GetString()!));
            return ExpressionValue.Nothing;
        }
        if(argument.PrimitiveKind == PrimitiveKind.String)
            return new(GetStringLength(argument.AsString()));
        return ExpressionValue.Nothing;
    }
    public static readonly FunctionDefinition Count = new FunctionDefinitionInstance("count", FunctionParameterType.Value, CountBody, [FunctionParameterType.Nodes]);
    static ExpressionValue CountBody(IReadOnlyList<ExpressionValue> arguments, FunctionExecutionContext context)
    {
        if(arguments is null or [])
            return ExpressionValue.Nothing;
        var argument = arguments[0];
        if(argument.ValueType != ValueType.Nodes)
            return ExpressionValue.Nothing;
        return new(argument.AsNodes().Count);
    }
    public static readonly FunctionDefinitionInstance Value = new FunctionDefinitionInstance("value", FunctionParameterType.Value, ValueBody, [FunctionParameterType.Nodes]);
    static ExpressionValue ValueBody(IReadOnlyList<ExpressionValue> arguments, FunctionExecutionContext context)
    {
        if(arguments is null or [])
            return ExpressionValue.Nothing;
        var argument = arguments[0];
        if(argument.ValueType != ValueType.Nodes)
            return ExpressionValue.Nothing;
        var nodes = argument.AsNodes();
        if(nodes.Count != 1)
            return ExpressionValue.Nothing;
        return new(argument.AsNodes()[0]);
    }
    public static readonly FunctionDefinitionInstance Match = new FunctionDefinitionInstance("match", FunctionParameterType.Logical, MatchBody, [FunctionParameterType.Value, FunctionParameterType.Value]);
    static ExpressionValue MatchBody(IReadOnlyList<ExpressionValue> arguments, FunctionExecutionContext context)
    {
        if(arguments is null or [] || arguments.Count < 2)
            return ExpressionValue.Nothing;
        var input = ConvetToString(arguments[0]);
        var pattern = ConvetToString(arguments[1]);
        if(input == null || pattern == null)
            return ExpressionValue.LogicalFalse;
        var regex = new Regex(pattern);
        var match = regex.Match(input);
        return new(match.Success && match.ValueSpan.Length == input.Length);
    }
    static string? ConvetToString(ExpressionValue value)
    {
        if(value.PrimitiveKind == PrimitiveKind.String)
            return value.AsString();
        if(value.ValueType != ValueType.Node)
            return null;
        var json = value.AsJson();
        if(json.ValueKind == JsonValueKind.String)
            return json.GetString();
        return null;
    }
}