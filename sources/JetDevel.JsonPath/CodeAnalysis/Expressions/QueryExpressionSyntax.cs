namespace JetDevel.JsonPath.CodeAnalysis.Expressions
{
    public abstract class QueryExpressionSyntax: ExpressionSyntax
    {
        private protected QueryExpressionSyntax(QueryType queryType, IReadOnlyList<SegmentSyntax> segments)
        {
            QueryType = queryType;
            Segments = segments;
        }
        public QueryType QueryType { get; }
        public IReadOnlyList<SegmentSyntax> Segments { get; }
        public override string ToString()
        {
            return QueryType == QueryType.RootNode ? "$" : "@" + string.Join(", ", Segments.Select(s => s.ToString()));
        }
    }
    public enum QueryType: byte
    {
        RootNode = 0,
        CurentNode = 1
    }
}
