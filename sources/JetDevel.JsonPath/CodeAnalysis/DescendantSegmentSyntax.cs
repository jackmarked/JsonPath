namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class DescendantSegmentSyntax: SegmentSyntax
{
    internal DescendantSegmentSyntax(BracketedSelectionSegmentSyntax bracketedSelectionSegment)
    {
        SelectionSegmentSyntax = bracketedSelectionSegment;
    }
    internal DescendantSegmentSyntax(SelectorSyntax selector)
    {
        Selector = selector;
    }
    public SelectorSyntax? Selector { get; }
    public BracketedSelectionSegmentSyntax? SelectionSegmentSyntax { get; }
    public override SyntaxKind Kind => SyntaxKind.Unknown;
    public override string ToString() => ".." + Selector;
}