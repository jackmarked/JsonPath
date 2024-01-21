using System.Diagnostics.CodeAnalysis;

namespace JetDevel.JsonPath.CodeAnalysis;

sealed partial class Parser
{
    readonly Lexer lexer;
    Token token;
    Token nextToken;
    public Parser(Lexer lexer)
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
    public JsonPathQuerySyntax ParseQuery()
    {
        ReadToken();
        if(token.Kind == SyntaxKind.DollarMarkToken)
        {
            var segments = Segments();
            Expect(SyntaxKind.EndOfFile);
            return new JsonPathQuerySyntax(segments.AsReadOnly());
        }
        return null!;
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
        switch(nextToken.Kind)
        {
            case SyntaxKind.OpenBracketToken:
            case SyntaxKind.DotToken:
                segment = ChildSegment();
                break;
            case SyntaxKind.DotDotToken:
                segment = DescendantSegment();
                break;
        }
        // "[" | "." | ".."

        return segment != null;
        // segment             = child-segment / descendant-segment
    }
    BaseChildSegmentSyntax ChildSegment()
    {
        if(nextToken.Kind == SyntaxKind.OpenBracketToken)
            return BracketedSelection();
        Expect(SyntaxKind.DotToken);
        if(nextToken.Kind == SyntaxKind.AsteriskToken)
            return new ChildSegmentSyntax(WildcardSelector());
        return new ChildSegmentSyntax(MemberNameShorthand());
        /*
        child-segment = bracketed-selection /
                        ("." (wildcard-selector / member-name-shorthand)) */
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
        List<SelectorSyntax> selectors = [Selector()];
        while(Skip(SyntaxKind.CommaToken))
            selectors.Add(Selector());
        return selectors;
    }
    SelectorSyntax Selector()
    {
        return nextToken.Kind switch
        {
            SyntaxKind.StringLiteralToken => NameSelector(),
            SyntaxKind.AsteriskToken => WildcardSelector(),
            SyntaxKind.IntegerNumberLiteral or SyntaxKind.ColonToken => SliceOrIndexSelector(),
            SyntaxKind.QuestionMarkToken => FlterSelector(),
            _ => throw new InvalidOperationException($"Unexpected token kind: '{nextToken.Kind}'."),
        };


        /*

        selector            = name-selector  /       " or  '
                              wildcard-selector /    *
                              slice-selector /       [start S] ":" S [end S] [":" [S step ]]
                              index-selector /       index-selector      = int
                              filter-selector     = "?" S logical-expr

        int       = "0" / (["-"] DIGIT1 *DIGIT)      ; - optional
        DIGIT1    = %x31-39                    ; 1-9 non-zero digit

         */
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
        if(Skip(SyntaxKind.IntegerNumberLiteral) || Skip(SyntaxKind.ColonToken))
        {
            if(token.Kind == SyntaxKind.ColonToken)
                seconColon = token;
            else
                end = token;
            if(seconColon.HasValue)
            {
                if(Skip(SyntaxKind.IntegerNumberLiteral))
                    step = token;
            }
            else
            {
                if(Skip(SyntaxKind.ColonToken))
                    seconColon = token;
            }
        }
        if(seconColon.HasValue && Skip(SyntaxKind.IntegerNumberLiteral))
            step = token;
        return new SliceSelectorSyntax(start, firstColon, end, seconColon, step);
        // slice-selector /       [start S] ":" S [end S] [":" [S step ]]
        /*
 slice-selector /       [start S] ":" S [end S] [":" [S step ]]
 index-selector /       index-selector      = int
*/
    }
    private FlterSelectorSyntax FlterSelector()
    {
        Expect(SyntaxKind.QuestionMarkToken);
        var expression = LogicalExpression();
        return new FlterSelectorSyntax(expression);
        //filter-selector     = "?" S logical-expr
    }

    NameSelectorSyntax NameSelector()
    {
        Expect(SyntaxKind.StringLiteralToken);
        return new NameSelectorSyntax(SyntaxFacts.GetStringLiteralValue(token.Text));
    }
    MemberNameShorthandSelectorSyntax MemberNameShorthand()
    {
        Expect(SyntaxKind.MemberNameToken);
        return new MemberNameShorthandSelectorSyntax(token.Text);
    }
    DescendantSegmentSyntax DescendantSegment()
    {
        Expect(SyntaxKind.DotDotToken);
        return nextToken.Kind switch
        {
            SyntaxKind.OpenBracketToken => new DescendantSegmentSyntax(BracketedSelection()),
            SyntaxKind.AsteriskToken => new DescendantSegmentSyntax(WildcardSelector()),
            SyntaxKind.MemberNameToken => new DescendantSegmentSyntax(MemberNameShorthand()),
            _ => throw new InvalidOperationException($"Expected selector but was {nextToken.Kind}."),
        };
        /*
descendant-segment  = ".." (bracketed-selection /
          wildcard-selector /
          member-name-shorthand)
*/
    }
    void Expect(SyntaxKind tokenKind)
    {
        if(nextToken.Kind != tokenKind)
            throw new InvalidOperationException($"Expected {tokenKind} but was {nextToken.Kind}");
        ReadToken();
    }
    bool Skip(SyntaxKind tokenKind)
    {
        if(nextToken.Kind != tokenKind)
            return false;
        ReadToken();
        return true;
    }
}