namespace JetDevel.JsonPath.CodeAnalysis;
public static class SyntaxFacts
{
    internal static string GetText(SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.DollarMarkToken => "$",
            SyntaxKind.QuestionMarkToken => "?",
            SyntaxKind.AtToken => "@",
            SyntaxKind.AsteriskToken => "*",
            SyntaxKind.DotToken => ".",
            SyntaxKind.DotDotToken => "..",
            SyntaxKind.ExclamationToken => "!",
            SyntaxKind.CommaToken => ",",
            SyntaxKind.OpenParenToken => "(",
            SyntaxKind.CloseParenToken => ")",
            SyntaxKind.OpenBracketToken => "[",
            SyntaxKind.CloseBracketToken => "]",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.BarBarToken => "||",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.GreaterEqualsToken => ">=",
            SyntaxKind.LessEqualsToken => "<=",
            _ => string.Empty,
        };
    }
    internal static string GetStringLiteralValue(string text)
    {
        if(string.IsNullOrEmpty(text) || text.Length < 2)
            return string.Empty;
        ReadOnlySpan<char> sourceSpan = text.AsSpan()[1..^1];
        return Unescape(sourceSpan);
    }

    static string Unescape(ReadOnlySpan<char> sourceSpan)
    {
        var span = sourceSpan.Length > 80 ? new char[sourceSpan.Length] : stackalloc char[sourceSpan.Length];
        var resultLength = 0;
        for(int i = 0; i < sourceSpan.Length; i++)
        {
            var ch = sourceSpan[i];
            var resultChar = ch;
            if(ch == '\\')
            {
                i++;
                var nextChar = sourceSpan[i];
                switch(nextChar)
                {
                    case '\'':
                    case '\\':
                    case '"':
                    case '/':
                        resultChar = nextChar;
                        break;
                    case 'b':
                        resultChar = '\x0008';
                        break;
                    case 'f':
                        resultChar = '\x000C';
                        break;
                    case 'n':
                        resultChar = '\x000A';
                        break;
                    case 'r':
                        resultChar = '\x000D';
                        break;
                    case 't':
                        resultChar = '\x0009';
                        break;
                    case 'u':
                        resultChar = (char)HexToDecimal(sourceSpan.Slice(i + 1, 4));
                        i += 4;
                        break;
                }
            }
            span[resultLength++] = resultChar;
        }
        return new string(span[..resultLength]);
    }
    static int HexToDecimal(ReadOnlySpan<char> chars)
    {
        var result = 0;
        foreach(var currentChar in chars)
            result = result << 4 | HexToDecimalFast(currentChar);
        return result;
    }
    static readonly sbyte[] hexToDecimalMap = new sbyte['g'];
    static SyntaxFacts()
    {
        for(int i = 0; i < hexToDecimalMap.Length; i++)
            hexToDecimalMap[i] = (sbyte)HexToDecimalInitialize((char)i);
    }
    static int HexToDecimalFast(char hex) =>
        hex < 'g' ? hexToDecimalMap[hex] : -1;
    static int HexToDecimalInitialize(char hex)
    {
        if(char.IsBetween(hex, '0', '9'))
            return hex - '0';
        if(char.IsBetween(hex, 'a', 'f'))
            return hex - 'a' + 10;
        if(char.IsBetween(hex, 'A', 'F'))
            return hex - 'A' + 10;
        return -1;
        //return hex switch
        //{//
        //    '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => hex - '0',
        //    'a' or 'A' => 10,
        //    'b' or 'B' => 11,
        //    'c' or 'C' => 12,
        //    'd' or 'D' => 13,
        //    'e' or 'E' => 14,
        //    'f' or 'F' => 15,
        //    _ => -1
        //};//
    }
    internal static SyntaxKind GetBinaryExpressionKind(Token operatorToken)
    {
        return operatorToken.Kind switch
        {
            SyntaxKind.BarBarToken => SyntaxKind.LogicalOrExpression,
            SyntaxKind.AmpersandAmpersandToken => SyntaxKind.LogicalAndExpression,
            SyntaxKind.EqualsEqualsToken => SyntaxKind.EqualsExpression,
            SyntaxKind.ExclamationEqualsToken => SyntaxKind.NotEqualsExpression,
            SyntaxKind.GreaterToken => SyntaxKind.GreaterThanExpression,
            SyntaxKind.LessToken => SyntaxKind.LessThanExpression,
            SyntaxKind.GreaterEqualsToken => SyntaxKind.GreaterThanOrEqualsExpression,
            SyntaxKind.LessEqualsToken => SyntaxKind.LessThanOrEqualsExpression,
            _ => SyntaxKind.Unknown,
        };
    }
}