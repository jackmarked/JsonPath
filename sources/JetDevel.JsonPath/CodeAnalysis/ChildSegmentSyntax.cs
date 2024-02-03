namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class ChildSegmentSyntax: BaseChildSegmentSyntax
{
    internal ChildSegmentSyntax(SelectorSyntax selector) =>
        Selector = selector;
    public SelectorSyntax Selector { get; }
    public override string ToString() => "." + Selector;
    public override SyntaxKind Kind => SyntaxKind.ChildSegment;
}