using JetDevel.JsonPath.CodeAnalysis;
using JetDevel.JsonPath.CodeAnalysis.Expressions;

namespace JetDevel.JsonPath.Tests;

sealed class SyntaxFactoryTests
{
    [Test]
    public void Parse_RootIdentifier_ReturnsValidRootIdentifier()
    {
        JsonPathQuerySyntax result = SyntaxFactory.Parse("$");

        Assert.That(result, Is.Not.Null);
    }
    [Test]
    public void Parse_RootIdentifierWithDotAsterisk_ReturnsValidSelector()
    {
        JsonPathQuerySyntax result = SyntaxFactory.Parse("$.*");
        var childSegment = result.Segments[0] as ChildSegmentSyntax;
        var selector = childSegment.Selector as WildcardSelectorSyntax;
        Assert.That(selector, Is.Not.Null);
    }
    [Test]
    public void Parse_RootIdentifierWithDotName_ReturnsValidSelector()
    {
        JsonPathQuerySyntax result = SyntaxFactory.Parse("$.abs");
        var childSegment = result.Segments[0] as ChildSegmentSyntax;
        var selector = childSegment!.Selector as MemberNameShorthandSelectorSyntax;
        Assert.That(selector!.MemberName, Is.EqualTo("abs"));
    }
    [Test]
    public void Parse_RootIdentifierWithBracketedSelectorIndex_ReturnsValidSelector()
    {
        JsonPathQuerySyntax result = SyntaxFactory.Parse("$[123489]");
        var childSegment = result.Segments[0] as BracketedSelectionSegmentSyntax;
        var selector = childSegment.Selectors[0] as IndexSelectorSyntax;
        Assert.That(selector.Index, Is.EqualTo(123489));
    }
    [Test]
    public void Parse_RootIdentifierWithBracketedSelectorSlice_ReturnsValidSelector()
    {
        var result = SyntaxFactory.Parse("$[2:4:1]");
        var childSegment = result.Segments[0] as BracketedSelectionSegmentSyntax;
        var selector = childSegment.Selectors[0] as SliceSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(selector.Start, Is.EqualTo(2));
            Assert.That(selector.End, Is.EqualTo(4));
            Assert.That(selector.Step, Is.EqualTo(1));
        });
    }
    [Test]
    public void Parse_BracketedSelectorWithMultiplyIndex_ReturnsValidSelectors()
    {
        var result = SyntaxFactory.Parse("$[2,     4    ,      7]");
        var childSegment = result.Segments[0] as BracketedSelectionSegmentSyntax;
        var selector1 = childSegment.Selectors[0] as IndexSelectorSyntax;
        var selector2 = childSegment.Selectors[1] as IndexSelectorSyntax;
        var selector3 = childSegment.Selectors[2] as IndexSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(selector1.Index, Is.EqualTo(2));
            Assert.That(selector2.Index, Is.EqualTo(4));
            Assert.That(selector3.Index, Is.EqualTo(7));
        });
    }

    [Test]
    public void Parse_BracketedSelectorWithNameSelector_ReturnsValidSelectors()
    {
        var a = "🙏";
        Console.WriteLine(a.Length);
        var result = SyntaxFactory.Parse("$['🙏']");
        var childSegment = result.Segments[0] as BracketedSelectionSegmentSyntax;
        var selector1 = childSegment.Selectors[0] as NameSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(selector1.Name, Is.EqualTo(a));

        });
    }
    [Test]
    public void Parse_BracketedSelectorWithSurogateNameSelector_ReturnsValidSelectors()
    {
        var expectedName = "🤔";
        Console.WriteLine(expectedName.Length);
        var result = SyntaxFactory.Parse("$['\\uD83E\\uDD14']");
        var childSegment = result.Segments[0] as BracketedSelectionSegmentSyntax;
        var selector1 = childSegment.Selectors[0] as NameSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(selector1.Name, Is.EqualTo(expectedName));

        });
    }
    [Test]
    public void Parse_BracketedSelectorWithNameSelectorUnicode_ReturnsValidSelectors()
    {
        var a = "🙏";
        Console.WriteLine(a.Length);
        var result = SyntaxFactory.Parse("$.🙏");
        var childSegment = result.Segments[0] as ChildSegmentSyntax;
        var selector1 = childSegment.Selector as MemberNameShorthandSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(selector1.MemberName, Is.EqualTo(a));

        });
    }
    [Test]
    public void Parse_ComplexSegments_ReturnsValidSelector()
    {
        var result = SyntaxFactory.Parse("$.store.book[0].title");
        //var childSegment = result.Segments[0] as BracketedSelectionSegmentSyntax;
        //var selector = childSegment.Selectors[0] as SliceSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(result.Segments, Has.Count.EqualTo(4));
        });
    }
    [Test]
    public void Parse_DescendantSegmentWithMemberName_ReturnsValidSelector()
    {
        var result = SyntaxFactory.Parse("$..store");
        var descendantSegment = result.Segments[0] as DescendantSegmentSyntax;
        var selector = descendantSegment.Selector as MemberNameShorthandSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(selector.MemberName, Is.EqualTo("store"));
        });
    }
    [Test]
    public void Parse_DescendantSegmentWithBracket_ReturnsValidSelector()
    {
        var result = SyntaxFactory.Parse("$..['store']");
        var descendantSegment = result.Segments[0] as DescendantSegmentSyntax;
        var bracketedSelection = descendantSegment.SelectionSegmentSyntax;
        Assert.Multiple(() =>
        {
            Assert.That((bracketedSelection.Selectors[0] as NameSelectorSyntax).Name, Is.EqualTo("store"));
        });
    }
    [Test]
    public void Parse_DescendantSegmentWithWildcard_ReturnsValidSelector()
    {
        var result = SyntaxFactory.Parse("$..*");
        var descendantSegment = result.Segments[0] as DescendantSegmentSyntax;
        var bracketedSelection = descendantSegment.SelectionSegmentSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(descendantSegment.Selector as WildcardSelectorSyntax, Is.Not.Null);
        });
    }
    [Test]
    public void Parse_SearchSelecor_ReturnsValidSelector()
    {
        var result = SyntaxFactory.Parse("$.a[?@.b]");
        var descendantSegment = result.Segments[1] as BracketedSelectionSegmentSyntax;
        var filterSelector = descendantSegment.Selectors[0] as FlterSelectorSyntax;
        Assert.Multiple(() =>
        {
            Assert.That(filterSelector.Expression, Is.Not.Null);
        });
    }
    [Test]
    public void Parse_SearchSelecorWithCompasionEquals_ReturnsValid()
    {
        var result = SyntaxFactory.Parse("$.a[?!(@.b == 'kilo' || length(@.c, 7, 7 <= 3) == 8 && @.d == false)  ]");
        var segment = result.Segments[1] as BracketedSelectionSegmentSyntax;
        var selector = segment.Selectors[0] as FlterSelectorSyntax;
        var expression = ((selector.Expression as LogcalNotExpressionSyntax).Expression as ParenthesizedExpressionSyntax).Expression;
        Assert.Multiple(() =>
        {
            Assert.That(expression.Kind, Is.EqualTo(SyntaxKind.LogicalOrExpression));
        });
    }
    // $.a[?@.b]
    // $.store.book[0].title
}