namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class FunctionExpressionSyntax: ExpressionSyntax
{
    internal FunctionExpressionSyntax(string name, IReadOnlyList<ExpressionSyntax> arguments)
    {
        Name = name;
        Arguments = arguments;
    }
    public string Name { get; }
    public IReadOnlyList<ExpressionSyntax> Arguments { get; }
    public override SyntaxKind Kind => SyntaxKind.FunctionExpression;
    public override string ToString() => Name + "(" + string.Join(", ", Arguments) + ")";
}