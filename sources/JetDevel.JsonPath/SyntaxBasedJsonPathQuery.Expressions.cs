using JetDevel.JsonPath.CodeAnalysis;
using JetDevel.JsonPath.CodeAnalysis.Expressions;
using System.Text.Json;

namespace JetDevel.JsonPath;

partial class SyntaxBasedJsonPathQuery
{
    static bool EvaluateLogicalExpression(ExpressionSyntax expression, ExpressionEvaluationContext context)
    {
        var result = EvaluateExpression(expression, context);
        return ConvertToBoolean(result);
    }
    static bool ConvertToBoolean(ExpressionValue result)
    {
        if(result.ValueType == ValueType.Logical || result.PrimitiveKind == PrimitiveKind.Boolean)
            return result.AsBoolean();
        if(result.ValueType == ValueType.Nodes)
            return result.AsNodes().Count > 0;
        return false;
    }
    static ExpressionValue EvaluateExpression(ExpressionSyntax expression, ExpressionEvaluationContext context)
    {
        switch(expression)
        {
            case FunctionExpressionSyntax function:
                return EvaluateFunction(function, context);
            case FilterQueryExpressionSyntax filterQuery:
                return EvaluateQuery(filterQuery.Segments, filterQuery.QueryType, false, context);
            case SingularQueryExpressionSyntax singularQuery:
                return EvaluateQuery(singularQuery.Segments, singularQuery.QueryType, true, context);
            case BinaryExpressionSyntax binary:
                ExpressionValue left = EvaluateExpression(binary.Left, context);
                ExpressionValue right = EvaluateExpression(binary.Right, context);
                return EvaluateBinary(left, right, binary.Kind);
            case LogcalNotExpressionSyntax logcalNot:
                ExpressionValue baseExpression = EvaluateExpression(logcalNot.Expression, context);
                return EvaluateLogicalNot(baseExpression);
            case ParenthesizedExpressionSyntax parenthesized:
                return EvaluateExpression(parenthesized.Expression, context);
            case FloatNumberLiteralSyntax floatNumberLiteral:
                return new ExpressionValue(floatNumberLiteral.Value);
            case IntegerNumberLiteralSyntax integerNumberLiteral:
                return new ExpressionValue(integerNumberLiteral.Value);
            case StringLiteralSyntax stringLiteral:
                return new ExpressionValue(stringLiteral.Value);
            case BooleanLiteralSyntax booleanLiteral:
                return new ExpressionValue(booleanLiteral.Value);
            case NullLiteralSyntax:
                return ExpressionValue.Null;
        }

        return new ExpressionValue();
    }
    static ExpressionValue EvaluateFunction(FunctionExpressionSyntax functionxpression, ExpressionEvaluationContext context)
    {
        var function = context.Services.GetFunction(functionxpression.Name);
        if(function == null)
            return ExpressionValue.Nothing;
        IReadOnlyList<ExpressionValue> argments = PepareArguments(functionxpression.Arguments, function.Parameters, context);

        return function.Execute(argments, new FunctionExecutionContext(context.CancellationToken));
    }

    static ExpressionValue PrepareArgumnt(IReadOnlyList<ExpressionSyntax> arguments, FunctionParameterType parameterType, int parameteIndex, ExpressionEvaluationContext context)
    {
        var argument = arguments.Count > parameteIndex ? arguments[parameteIndex] : null;
        if(argument == null)
            return ExpressionValue.Nothing;
        var value = EvaluateExpression(argument, context);
        switch(parameterType)
        {
            case FunctionParameterType.Logical:
                if(value.PrimitiveKind == PrimitiveKind.Boolean)
                    return value;
                if(value.ValueType == ValueType.Nodes)
                {
                    var nodes = value.AsNodes();
                    return new(nodes.Count > 0);
                }
                return ExpressionValue.Nothing;
            case FunctionParameterType.Value:
                if(value.ValueType == ValueType.Node || value.ValueType == ValueType.PrimitiveValue)
                    return value;
                if(value.ValueType == ValueType.Nodes)
                {
                    var nodes = value.AsNodes();
                    if(nodes.Count == 1)
                        return new(nodes[0]);
                }
                return ExpressionValue.Nothing;
            case FunctionParameterType.Nodes:
                if(value.ValueType == ValueType.Nodes)
                    return value;
                return ExpressionValue.Nothing;
        }
        return ExpressionValue.Nothing;
    }
    private static IReadOnlyList<ExpressionValue> PepareArguments(IReadOnlyList<ExpressionSyntax> arguments, IReadOnlyList<FunctionParameterType> parameters, ExpressionEvaluationContext context)
    {
        var result = new List<ExpressionValue>(parameters.Count);
        for(int i = 0; i < parameters.Count; i++)
        {
            var argument = PrepareArgumnt(arguments, parameters[i], i, context);
            result.Add(argument);
            context.CancellationToken.ThrowIfCancellationRequested();
        }
        return result;
    }

    static ExpressionValue EvaluateJson(List<JsonElement> nodeList)
    {
        if(nodeList.Count == 0)
            return ExpressionValue.EmptyNodes;
        return new ExpressionValue(nodeList);
    }
    static ExpressionValue EvaluateJson(JsonElement json)
    {
        switch(json.ValueKind)
        {
            case JsonValueKind.Number:
                if(json.TryGetInt64(out var longValue))
                    return new ExpressionValue(longValue);
                return new ExpressionValue(json.GetDouble());
            case JsonValueKind.String:
                return new ExpressionValue(json.GetString()!);
            case JsonValueKind.Object:
            case JsonValueKind.Array:
            case JsonValueKind.True:
                return ExpressionValue.LogicalTrue;
            case JsonValueKind.False:
                return ExpressionValue.LogicalFalse;
            case JsonValueKind.Null:
                return ExpressionValue.Null;
        }
        return ExpressionValue.Nothing;
    }
    static ExpressionValue EvaluateQuery(IReadOnlyList<SegmentSyntax> segments, QueryType queryType, bool isSingular, ExpressionEvaluationContext context)
    {
        var startNode = queryType == QueryType.RootNode ? context.Root : context.Current;
        var result = ProcSegments(startNode, segments, new QueryContext(context.Root, context.Services, context.CancellationToken));
        if(isSingular && result.Count == 1)
            return new ExpressionValue(result[0]);
        return EvaluateJson(result);
    }
    static ExpressionValue EvaluateLogicalNot(ExpressionValue baseExpression)
    {
        if(baseExpression.IsNothing)
            return baseExpression;
        return new(!ConvertToBoolean(baseExpression));
    }
    static ExpressionValue EvaluateBinary(ExpressionValue left, ExpressionValue right, SyntaxKind kind)
    {
        switch(kind)
        {
            case SyntaxKind.LogicalAndExpression:
                return new(ConvertToBoolean(left) && ConvertToBoolean(right));
            case SyntaxKind.LogicalOrExpression:
                return new(ConvertToBoolean(left) || ConvertToBoolean(right));
            case SyntaxKind.EqualsExpression:
                return EvaluateEquals(left, right);
            case SyntaxKind.LessThanExpression:
                return LessThan(left, right);
            case SyntaxKind.NotEqualsExpression:
                var equals = EvaluateEquals(left, right);
                if(equals.PrimitiveKind == PrimitiveKind.Boolean)
                    return new(!equals.AsBoolean());
                break;
            case SyntaxKind.GreaterThanExpression:
                return LessThan(right, left);
            case SyntaxKind.GreaterThanOrEqualsExpression:
                var equalsInGreaterThan = EvaluateEquals(left, right);
                if(equalsInGreaterThan.IsNothing)
                    return ExpressionValue.Nothing;
                var greaterThan = LessThan(right, left);
                if(greaterThan.IsNothing)
                    return ExpressionValue.Nothing;
                return new(equalsInGreaterThan.AsBoolean() || greaterThan.AsBoolean());
            case SyntaxKind.LessThanOrEqualsExpression:
                var equalsLessThan = EvaluateEquals(left, right);
                if(equalsLessThan.IsNothing)
                    return ExpressionValue.Nothing;
                var lessThan = LessThan(left, right);
                if(lessThan.IsNothing)
                    return ExpressionValue.Nothing;
                return new(equalsLessThan.AsBoolean() || lessThan.AsBoolean());
        }
        return ExpressionValue.Nothing;
    }
    static ExpressionValue EvaluateEquals(ExpressionValue leftValue, ExpressionValue rightValue)
    {
        var left = leftValue;
        var right = rightValue;
        if(left.ValueType == ValueType.Node)
            left = EvaluateJson(left.AsJson());
        if(right.ValueType == ValueType.Node)
            right = EvaluateJson(right.AsJson());
        switch(left.ValueType)
        {
            case ValueType.Logical:
                switch(right.ValueType)
                {
                    case ValueType.Logical:
                        return EqualsPrimitive(left, right);
                    case ValueType.Node:
                        return EqualsNodeAndPrimtive(right, left);
                }
                break;
            case ValueType.PrimitiveValue:
                switch(right.ValueType)
                {
                    case ValueType.PrimitiveValue:
                        return EqualsPrimitive(left, right);
                    case ValueType.Node:
                        return EqualsNodeAndPrimtive(right, left);
                }
                break;
            case ValueType.Node:
                switch(right.ValueType)
                {
                    case ValueType.PrimitiveValue:
                        return EqualsNodeAndPrimtive(left, right);
                    case ValueType.Node:
                        return EqualsNodeAndNode(left, right);
                }
                break;
        }
        return ExpressionValue.Nothing;
    }
    static ExpressionValue EqualsNodeAndNode(ExpressionValue left, ExpressionValue right)
    {
        var leftJson = left.AsJson();
        var rightJson = right.AsJson();
        return new ExpressionValue(leftJson.Equals(rightJson));
    }
    static ExpressionValue EqualsNodeAndPrimtive(ExpressionValue first, ExpressionValue second)
    {
        var json = first.AsJson();
        return EqualsPrimitive(EvaluateJson(json), second);
    }
    static ExpressionValue LessThan(ExpressionValue leftValue, ExpressionValue rightValue)
    {
        var left = leftValue;
        var right = rightValue;
        if(left.ValueType == ValueType.Node)
            left = EvaluateJson(left.AsJson());
        if(right.ValueType == ValueType.Node)
            right = EvaluateJson(right.AsJson());
        if(left.ValueType != ValueType.PrimitiveValue || right.ValueType != ValueType.PrimitiveValue) // ???
            return ExpressionValue.Nothing;
        switch(left.PrimitiveKind)
        {
            case PrimitiveKind.String:
                if(right.PrimitiveKind == PrimitiveKind.String)
                    return new(string.Compare(left.AsString(), right.AsString(), StringComparison.Ordinal) < 0);
                return ExpressionValue.Nothing;
            case PrimitiveKind.Integer:
                switch(right.PrimitiveKind)
                {
                    case PrimitiveKind.Integer:
                        return new(left.AsLong() < right.AsLong());
                    case PrimitiveKind.Float:
                        return new(left.AsLong() < right.AsDouble());
                }
                break;
            case PrimitiveKind.Float:
                switch(right.PrimitiveKind)
                {
                    case PrimitiveKind.Integer:
                        return new(left.AsDouble() < right.AsLong());
                    case PrimitiveKind.Float:
                        return new(left.AsDouble() < right.AsDouble());
                }
                break;
        }
        return ExpressionValue.Nothing;
    }
    static ExpressionValue EqualsPrimitive(ExpressionValue left, ExpressionValue right)
    {
        switch(left.PrimitiveKind)
        {
            case PrimitiveKind.Integer:
                return right.PrimitiveKind switch
                {
                    PrimitiveKind.Integer => new(left.AsLong() == right.AsLong()),
                    PrimitiveKind.Float => new(left.AsLong() == right.AsDouble()),
                    _ => ExpressionValue.Nothing,
                };
            case PrimitiveKind.Boolean:
                return right.PrimitiveKind switch
                {
                    PrimitiveKind.Boolean => new(left.AsBoolean() == right.AsBoolean()),
                    _ => ExpressionValue.Nothing,
                };
            case PrimitiveKind.Float:
                return right.PrimitiveKind switch
                {
                    PrimitiveKind.Integer => new(left.AsDouble() == right.AsLong()),
                    PrimitiveKind.Float => new(left.AsDouble() == right.AsDouble()),
                    _ => ExpressionValue.Nothing,
                };
            case PrimitiveKind.String:
                return right.PrimitiveKind switch
                {
                    PrimitiveKind.String => new(left.AsString() == right.AsString()),
                    _ => ExpressionValue.Nothing
                };
            case PrimitiveKind.Null:
                return new(right.PrimitiveKind == PrimitiveKind.Null);
        }
        return ExpressionValue.Nothing;
    }
}