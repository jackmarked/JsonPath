namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public abstract class LiteralExpressionSyntax<TValue>: ExpressionSyntax
{
    protected private LiteralExpressionSyntax(TValue value)
    {
        Value = value;
    }
    public TValue Value { get; }
    public override string ToString()
    {
        return (Value is null ? "null" : Value.ToString()) ?? string.Empty;
    }
}