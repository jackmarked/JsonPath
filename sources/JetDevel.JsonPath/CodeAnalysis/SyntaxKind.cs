namespace JetDevel.JsonPath.CodeAnalysis;

public enum SyntaxKind: ulong
{
    Unknown = 0,

    /// <summary>
    /// "$".
    /// </summary>
    DollarMarkToken = 1,

    /// <summary>
    /// "@".
    /// </summary>
    AtToken = 2,

    /// <summary>
    /// "(".
    /// </summary>
    OpenBracketToken = 3,

    /// <summary>
    /// ")".
    /// </summary>
    CloseBracketToken = 4,

    /// <summary>
    /// ".".
    /// </summary>
    DotToken = 5,

    /// <summary>
    /// "..".
    /// </summary>
    DotDotToken = 6,

    /// <summary>
    /// "*".
    /// </summary>
    AsteriskToken = 7,

    /// <summary>
    /// "member".
    /// </summary>
    MemberNameToken = 8,

    /// <summary>
    /// ",".
    /// </summary>
    CommaToken = 9,

    /// <summary>
    /// "'text'" or ""text"".
    /// </summary>
    StringLiteralToken = 10,

    /// <summary>
    /// "1234".
    /// </summary>
    IntegerNumberLiteral = 11,

    /// <summary>
    /// "123.124e10".
    /// </summary>
    FloatNumberLiteral = 12,

    /// <summary>
    /// "?".
    /// </summary>
    QuestionMarkToken = 13,

    /// <summary>
    /// ":".
    /// </summary>
    ColonToken = 14,

    /// <summary>
    /// "||".
    /// </summary>
    BarBarToken = 15,

    /// <summary>
    /// "&amp;&amp;".
    /// </summary>
    AmpersandAmpersandToken = 16,

    /// <summary>
    /// "!".
    /// </summary>
    ExclamationToken = 17,

    /// <summary>
    /// "==".
    /// </summary>
    EqualsEqualsToken = 18,

    /// <summary>
    /// "!=".
    /// </summary>
    ExclamationEqualsToken = 19,

    /// <summary>
    /// "<".
    /// </summary>
    LessToken = 20,

    /// <summary>
    /// ">".
    /// </summary>
    GreaterToken = 21,

    /// <summary>
    /// "<=".
    /// </summary>
    LessEqualsToken = 22,

    /// <summary>
    /// ">=".
    /// </summary>
    GreaterEqualsToken = 23,

    /// <summary>
    /// "(".
    /// </summary>
    OpenParenToken = 24,

    /// <summary>
    /// ")".
    /// </summary>
    CloseParenToken = 25,
    ByteOrderMark = 26,
    EndOfFile = 27,


    JsonPathQuery = 28,
    LogcalNotExpression = 29,
    NullLiteralExpression = 30,
    EqualsExpression = 31,
    ParenthesizedExpression = 32,
    LessThanExpression = 33,
    LessThanOrEqualsExpression = 34,
    GreaterThanOrEqualsExpression = 35,
    GreaterThanExpression = 36,
    NotEqualsExpression = 37,
    LogicalAndExpression = 38,
    LogicalOrExpression = 39,
    FunctionExpression = 40,
    FilterQueryExpression = 41,
    SingularQueryExpression = 42
}