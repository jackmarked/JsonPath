using JetDevel.JsonPath.CodeAnalysis.Expressions;
namespace JetDevel.JsonPath.CodeAnalysis;

partial class Parser
{
    ExpressionSyntax LogicalOrExpression()
    { // logical-or-expr     = logical-and-expr *(S "||" S logical-and-expr)
        var left = LogicalAndExpression();
        while(TryReadToken(SyntaxKind.BarBarToken))
        {
            var operatorToken = token;
            var right = LogicalAndExpression();
            left = new BinaryExpressionSyntax(left, right, operatorToken);
        }
        return left;
    }
    ExpressionSyntax LogicalAndExpression()
    { // logical-and-expr    = basic-expr *(S "&&" S basic-expr)
        var left = BasicExpression();
        while(TryReadToken(SyntaxKind.AmpersandAmpersandToken))
        {
            var operatorToken = token;
            var right = BasicExpression();
            left = new BinaryExpressionSyntax(left, right, operatorToken);
        }
        return left;
    }
    ExpressionSyntax BasicExpression()
    {  // basic-expr          = paren-expr / test-expr / comparison-expr
        if(nextToken.Kind == SyntaxKind.ExclamationToken)
            return LogicalNotExpression();
        ExpressionSyntax? left = null;
        List<SegmentSyntax>? segments = null;
        QueryType queryType = QueryType.CurentNode;
        if(nextToken.Kind == SyntaxKind.MemberNameToken && nextToken.Text is not ("null" or "true" or "false"))
        {
            left = FunctionExpression();
        }
        else if(nextToken.Kind == SyntaxKind.OpenParenToken)
        {
            left = ParenthesizedExpression();
        }
        else if(nextToken.Kind == SyntaxKind.DollarMarkToken)
        {
            Expect(SyntaxKind.DollarMarkToken);
            segments = Segments();
            queryType = QueryType.RootNode;
        }
        else if(nextToken.Kind == SyntaxKind.AtToken)
        {
            Expect(SyntaxKind.AtToken);
            segments = Segments();
            queryType = QueryType.CurentNode;
        }
        if(segments != null)
        {
            if(IsComparsion(nextToken))
                left = new SingularQueryExpressionSyntax(queryType, segments);
            else
                left = new FilterQueryExpressionSyntax(queryType, segments);
        }
        left ??= Literal();
        if(!IsComparsion(nextToken))
            return left;
        var operatorToken = nextToken;
        Expect(IsComparsion);
        var right = Comparable();
        return new BinaryExpressionSyntax(left, right!, operatorToken);
    }
    ExpressionSyntax? Comparable()
    {
        var lieral = Literal(true);
        if(lieral != null)
            return lieral;
        if(nextToken.Kind == SyntaxKind.MemberNameToken)
            return FunctionExpression();
        if(nextToken.Kind == SyntaxKind.DollarMarkToken)
        {
            Expect(SyntaxKind.DollarMarkToken);
            return new SingularQueryExpressionSyntax(QueryType.RootNode, Segments());
        }
        if(nextToken.Kind == SyntaxKind.AtToken)
        {
            Expect(SyntaxKind.AtToken);
            return new SingularQueryExpressionSyntax(QueryType.CurentNode, Segments());
        }
        AddErrorAndReadToken("Invalid comparable.");
        return null;
    }
    ExpressionSyntax Literal(bool canReturnNull = false)
    {
        switch(nextToken.Kind)
        {
            case SyntaxKind.FloatNumberLiteral:
                return new FloatNumberLiteralSyntax(ReadToken());
            case SyntaxKind.IntegerNumberLiteral:
                return new IntegerNumberLiteralSyntax(ReadToken());
            case SyntaxKind.StringLiteralToken:
                return new StringLiteralSyntax(ReadToken());
            case SyntaxKind.MemberNameToken:
                switch(nextToken.Text)
                {
                    case "true":
                        ReadToken();
                        return new BooleanLiteralSyntax(true);
                    case "false":
                        ReadToken();
                        return new BooleanLiteralSyntax(false);
                    case "null":
                        ReadToken();
                        return new NullLiteralSyntax();
                }
                break;
        }
        if(canReturnNull)
            return null!;
        AddErrorAndReadToken("Invalid literal.");
        return null!;
    }
    void Expect(Func<Token, bool> predicate)
    {
        if(!predicate(nextToken))
            AddError($"Unexpected token '{nextToken.Text}'.");
        ReadToken();
    }
    private bool IsComparsion(Token token)
    {
        return token.Kind switch
        {
            SyntaxKind.EqualsEqualsToken or SyntaxKind.ExclamationEqualsToken
            or SyntaxKind.LessEqualsToken or SyntaxKind.GreaterEqualsToken
            or SyntaxKind.GreaterToken or SyntaxKind.LessToken => true,
            _ => false,
        };
    }

    LogcalNotExpressionSyntax LogicalNotExpression()
    {
        Expect(SyntaxKind.ExclamationToken);
        if(nextToken.Kind == SyntaxKind.OpenParenToken)
            return new(ParenthesizedExpression());
        if(nextToken.Kind == SyntaxKind.MemberNameToken)
            return new(FunctionExpression());
        if(nextToken.Kind == SyntaxKind.DollarMarkToken)
        {
            Expect(SyntaxKind.DollarMarkToken);
            return new(new FilterQueryExpressionSyntax(QueryType.RootNode, Segments()));
        }
        Expect(SyntaxKind.AtToken);
        return new(new FilterQueryExpressionSyntax(QueryType.CurentNode, Segments()));
    }
    ParenthesizedExpressionSyntax ParenthesizedExpression()
    {
        Expect(SyntaxKind.OpenParenToken);
        var expression = LogicalOrExpression();
        Expect(SyntaxKind.CloseParenToken);
        return new(expression);
    }
    FunctionExpressionSyntax FunctionExpression()
    {
        Expect(SyntaxKind.MemberNameToken);
        var nameToken = token;
        Expect(SyntaxKind.OpenParenToken);
        IReadOnlyList<ExpressionSyntax>? arguments = null;
        if(nextToken.Kind != SyntaxKind.CloseParenToken)
            arguments = Arguments();
        Expect(SyntaxKind.CloseParenToken);
        return new(nameToken.Text, arguments ?? []);
    }
    private IReadOnlyList<ExpressionSyntax> Arguments()
    {
        var result = new List<ExpressionSyntax>();
        do
            result.Add(Argument());
        while(TryReadToken(SyntaxKind.CommaToken));
        return result.AsReadOnly();
    }
    ExpressionSyntax Argument()
    {
        var left = Literal(true);
        if(left == null)
            return LogicalOrExpression();
        if(!IsComparsion(nextToken))
            return left;
        var operatorToken = nextToken;
        Expect(IsComparsion);
        var right = Comparable();
        return new BinaryExpressionSyntax(left, right!, operatorToken);
    }
}