namespace JetDevel.JsonPath.CodeAnalysis.Expressions;

public sealed class FilterQueryExpressionSyntax: QueryExpressionSyntax
{
    internal FilterQueryExpressionSyntax(QueryType queryType, IReadOnlyList<SegmentSyntax> segments)
        : base(queryType, segments) { }

    public override SyntaxKind Kind => SyntaxKind.FilterQueryExpression;
}