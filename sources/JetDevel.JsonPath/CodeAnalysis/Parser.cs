using System.Diagnostics.CodeAnalysis;

namespace JetDevel.JsonPath.CodeAnalysis;

sealed partial class Parser
{
    readonly Lexer lexer;
    Token token;
    Token nextToken;
    List<string>? errors = null;
    void AddError(string message)
    {
        errors ??= [];
        errors.Add(message);
    }
    void AddErrorAndReadToken(string message)
    {
        AddError(message);
        ReadToken();
    }
    internal Parser(Lexer lexer)
    {
        this.lexer = lexer;
    }
    Token ReadToken()
    {
        token = lexer.GetNextToken();
        nextToken = lexer.LookAhead();
        return token;
    }
    bool TryReadToken(SyntaxKind kind)
    {
        if(nextToken.Kind != kind)
            return false;
        ReadToken();
        return true;
    }
    public ParserResult ParseQuery()
    {
        ReadToken();
        if(token.Kind != SyntaxKind.DollarMarkToken)
            return null!;

        var segments = Segments();
        Expect(SyntaxKind.EndOfFile);
        if(errors != null)
            return new (errors.AsReadOnly());
        return new(new JsonPathQuerySyntax(segments.AsReadOnly()));
    }
    List<SegmentSyntax> Segments()
    {
        var result = new List<SegmentSyntax>();
        while(TryParseSegment(out var segment))
            result.Add(segment);
        return result;
    }
    bool TryParseSegment([NotNullWhen(true)] out SegmentSyntax? segment)
    {
        segment = null;
        switch(nextToken.Kind)// "[" | "." | ".."
        {
            case SyntaxKind.OpenBracketToken:
            case SyntaxKind.DotToken:
                segment = ChildSegment();
                break;
            case SyntaxKind.DotDotToken:
                segment = DescendantSegment();
                break;
        }
        return segment != null;
    }
    BaseChildSegmentSyntax ChildSegment()
    {
        if(nextToken.Kind == SyntaxKind.OpenBracketToken)
            return BracketedSelection();
        Expect(SyntaxKind.DotToken);
        if(nextToken.Kind == SyntaxKind.AsteriskToken)
            return new ChildSegmentSyntax(WildcardSelector());
        return new ChildSegmentSyntax(MemberNameShorthand());
    }
    WildcardSelectorSyntax WildcardSelector()
    {
        Expect(SyntaxKind.AsteriskToken);
        return new WildcardSelectorSyntax();
    }
    BracketedSelectionSegmentSyntax BracketedSelection()
    {
        Expect(SyntaxKind.OpenBracketToken);
        var selectors = Selectors();
        Expect(SyntaxKind.CloseBracketToken);
        return new BracketedSelectionSegmentSyntax(selectors);
    }
    List<SelectorSyntax> Selectors()
    {
        List<SelectorSyntax> selectors = [];
        do
        {
            var selector = Selector();
            if(selector == null)
                return selectors;
            selectors.Add(selector);
        } while(TryReadToken(SyntaxKind.CommaToken));
        return selectors;
    }
    SelectorSyntax? Selector()
    {
        switch(nextToken.Kind)
        {
            case SyntaxKind.StringLiteralToken:
                return new NameSelectorSyntax(SyntaxFacts.GetStringLiteralValue(ReadToken().Text));
            case SyntaxKind.AsteriskToken:
                ReadToken();
                return new WildcardSelectorSyntax();
            case SyntaxKind.IntegerNumberLiteral or SyntaxKind.ColonToken:
                return SliceOrIndexSelector();
            case SyntaxKind.QuestionMarkToken:
                ReadToken();
                return new FilterSelectorSyntax(LogicalOrExpression());
            default:
                AddErrorAndReadToken($"Unexpected token kind: '{nextToken.Kind}'.");
                return null;
        }
    }
    SelectorSyntax SliceOrIndexSelector()
    {
        var integerOrColonToken = nextToken;
        ReadToken();
        if((integerOrColonToken.Kind != SyntaxKind.ColonToken) && (integerOrColonToken.Kind != SyntaxKind.IntegerNumberLiteral
                || nextToken.Kind != SyntaxKind.ColonToken))
            return new IndexSelectorSyntax(integerOrColonToken.Text); // index-selector      = int

        Token? start = integerOrColonToken.Kind == SyntaxKind.IntegerNumberLiteral ? integerOrColonToken : null;
        Token firstColon = integerOrColonToken.Kind == SyntaxKind.ColonToken ? integerOrColonToken : nextToken;
        Token? end = null;
        Token? seconColon = null;
        Token? step = null;
        if(start.HasValue)
        {
            ReadToken();
        }
        if(TryReadToken(SyntaxKind.IntegerNumberLiteral) || TryReadToken(SyntaxKind.ColonToken))
        {
            if(token.Kind == SyntaxKind.ColonToken)
                seconColon = token;
            else
                end = token;
            if(seconColon.HasValue)
            {
                if(TryReadToken(SyntaxKind.IntegerNumberLiteral))
                    step = token;
            }
            else
            {
                if(TryReadToken(SyntaxKind.ColonToken))
                    seconColon = token;
            }
        }
        if(seconColon.HasValue && TryReadToken(SyntaxKind.IntegerNumberLiteral))
            step = token;
        return new SliceSelectorSyntax(start, firstColon, end, seconColon, step);
        // slice-selector /       [start S] ":" S [end S] [":" [S step ]]
        /*
 slice-selector /       [start S] ":" S [end S] [":" [S step ]]
 index-selector /       index-selector      = int
*/
    }
    MemberNameShorthandSelectorSyntax MemberNameShorthand()
    {
        Expect(SyntaxKind.MemberNameToken);
        return new MemberNameShorthandSelectorSyntax(token.Text);
    }
    DescendantSegmentSyntax? DescendantSegment()
    {
        Expect(SyntaxKind.DotDotToken);
        switch(nextToken.Kind)
        {
            case SyntaxKind.OpenBracketToken:
                return new DescendantSegmentSyntax(BracketedSelection());
            case SyntaxKind.AsteriskToken:
                return new DescendantSegmentSyntax(WildcardSelector());
            case SyntaxKind.MemberNameToken:
                return new DescendantSegmentSyntax(MemberNameShorthand());
            default:
                AddErrorAndReadToken($"Expected segment but was {nextToken.Kind}.");
                return null;
        }
        /*
descendant-segment  = ".." (bracketed-selection /
          wildcard-selector /
          member-name-shorthand)
*/
    }
    void Expect(SyntaxKind tokenKind)
    {
        if(nextToken.Kind != tokenKind)
            AddError($"Expected {tokenKind} but was {nextToken.Kind}.");
        ReadToken();
    }
}