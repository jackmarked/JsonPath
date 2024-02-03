namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class BracketedSelectionSegmentSyntax: BaseChildSegmentSyntax
{
    internal BracketedSelectionSegmentSyntax(IReadOnlyList<SelectorSyntax> selectors) =>
        Selectors = selectors;

    public IReadOnlyList<SelectorSyntax> Selectors { get; }
    public override string ToString() =>
        "[" + string.Join(" ", Selectors) + "]";
    public override SyntaxKind Kind => SyntaxKind.BracketedSelection;
}