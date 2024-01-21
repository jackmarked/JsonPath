namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class SingularQueryExpressionSyntax: QueryExpressionSyntax
{
    internal SingularQueryExpressionSyntax(QueryType queryType, IReadOnlyList<SegmentSyntax> segments) : base(queryType, segments)
    {
    }
    public override SyntaxKind Kind => SyntaxKind.SingularQueryExpression;
}