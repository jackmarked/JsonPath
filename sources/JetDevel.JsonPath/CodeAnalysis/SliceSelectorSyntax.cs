namespace JetDevel.JsonPath.CodeAnalysis;

public sealed class SliceSelectorSyntax: SelectorSyntax
{
    internal SliceSelectorSyntax(Token? start, Token firstColon, Token? end, Token? seconColon, Token? step)
    {
        if(start != null && long.TryParse(start.Value.Text, out var startValue))
            Start = startValue;
        if(end != null && long.TryParse(end.Value.Text, out var endValue))
            End = endValue;
        if(step != null && long.TryParse(step.Value.Text, out var stepValue))
            Step = stepValue;
    }
    public long? Start { get; }
    public long? End { get; }
    public long? Step { get; }
    public override SyntaxKind Kind => SyntaxKind.SliceSelector;
}