namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class JsonPathQuerySyntax: JsonPathSyntaxNode
{
    internal JsonPathQuerySyntax(IReadOnlyList<SegmentSyntax> segments) =>
        Segments = segments;

    public IReadOnlyList<SegmentSyntax> Segments { get; }
    public override SyntaxKind Kind => SyntaxKind.JsonPathQuery;
    public override string ToString() => "$" + string.Concat(Segments);
}