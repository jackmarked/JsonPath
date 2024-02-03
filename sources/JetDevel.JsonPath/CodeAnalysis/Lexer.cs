using System.Buffers;
using System.Runtime.InteropServices;

namespace JetDevel.JsonPath.CodeAnalysis;

public sealed partial class Lexer
{
    readonly UnicodeCharacterReader source;
    Token nextToken;
    int codePoint;
    bool isEndOfStream;
    List<int> buffer = [];
#if DEBUG
    public string Symbol => char.ConvertFromUtf32(codePoint);
#endif
    public Lexer(UnicodeCharacterReader source)
    {
        ArgumentNullException.ThrowIfNull(source);
        this.source = source;
        isEndOfStream = !source.TryReadNext(out var chrarcter);
        codePoint = chrarcter.CodePoint;
        nextToken = Scan();
    }
    void AddChar()
    {
        if(isEndOfStream)
            return;
        buffer.Add(codePoint);
        isEndOfStream = !source.TryReadNext(out var chrarcter);
        codePoint = chrarcter.CodePoint;
    }
    Token CreateToken(SyntaxKind kind)
    {
        var token = new Token(kind, CollectionsMarshal.AsSpan(buffer));
        buffer.Clear();
        return token;
    }
    void SkipWhiteSpaces()
    {
        ReadAll(KnownCodes.BlankSpaces);
        buffer.Clear();
    }
    public Token GetNextToken()
    {
        var result = nextToken;
        nextToken = Scan();
        return result;
    }
    bool TryRead(int value)
    {
        if(isEndOfStream || codePoint != value)
            return false;
        AddChar();
        return true;
    }
    bool TryRead(SearchValues<byte> values)
    {
        if(isEndOfStream || codePoint > 0x7f || !values.Contains((byte)codePoint))
            return false;
        AddChar();
        return true;
    }
    bool TryRead(Func<int, bool> predicate)
    {
        if(isEndOfStream || !predicate(codePoint))
            return false;
        AddChar();
        return true;
    }
    bool ReadAny(SearchValues<byte> values)
    {
        if(!TryRead(values))
            return false;
        ReadAll(values);
        return true;
    }
    void ReadAll(SearchValues<byte> values)
    {
        while(TryRead(values))
            if(!TryRead(values))
                break;
    }
    void ReadAll(Func<int, bool> predicate)
    {
        while(TryRead(predicate))
            if(!TryRead(predicate))
                break;
    }
    bool TryReadDecimal(out Token token)
    {
        token = default;
        bool hasFraction = false;
        if(TryRead('.'))
        {
            if(!ReadAny(KnownCodes.Digits))
            {
                token = CreateToken(SyntaxKind.Unknown);
                return true;
            }
            hasFraction = true;
        }
        var hasExponent = codePoint is KnownCodes.e or KnownCodes.E;
        if(!hasExponent)
        {
            if(hasFraction)
                token = CreateToken(SyntaxKind.FloatNumberLiteral);
            return hasFraction;
        }
        AddChar();
        _ = TryRead('-') || TryRead('+');
        if(ReadAny(KnownCodes.Digits))
            token = CreateToken(SyntaxKind.FloatNumberLiteral);
        else
            token = CreateToken(SyntaxKind.Unknown);
        return true;
    }
    private Token Scan()
    {
        SkipWhiteSpaces();
        if(isEndOfStream)
            return new Token(SyntaxKind.EndOfFile);
        switch(codePoint)
        {
            case '0':
                AddChar();
                if(TryReadDecimal(out var decimalToken0))
                    return decimalToken0;
                return CreateToken(SyntaxKind.IntegerNumberLiteral);
            case '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                AddChar();
                ReadAll(KnownCodes.Digits);
                if(TryReadDecimal(out var decimalToken))
                    return decimalToken;
                return CreateToken(SyntaxKind.IntegerNumberLiteral);
            case '-':
                AddChar();
                if(TryRead('0'))
                    if(TryReadDecimal(out var negativeDecimalToken))
                        return negativeDecimalToken;
                    else
                        return CreateToken(SyntaxKind.Unknown);
                if(!TryRead(KnownCodes.DigitsWithout0))
                    return CreateToken(SyntaxKind.Unknown);
                ReadAll(KnownCodes.Digits);
                return CreateToken(SyntaxKind.IntegerNumberLiteral);
            case '|':
                AddChar();
                if(TryRead('|'))
                    return CreateToken(SyntaxKind.BarBarToken);
                return CreateToken(SyntaxKind.Unknown);
            case '&':
                AddChar();
                if(TryRead('&'))
                    return CreateToken(SyntaxKind.AmpersandAmpersandToken);
                return CreateToken(SyntaxKind.Unknown);
            case '=':
                AddChar();
                if(TryRead('='))
                    return CreateToken(SyntaxKind.EqualsEqualsToken);
                return CreateToken(SyntaxKind.Unknown);
            case '>':
                AddChar();
                if(TryRead('='))
                    return CreateToken(SyntaxKind.GreaterEqualsToken);
                return CreateToken(SyntaxKind.GreaterToken);
            case '<':
                AddChar();
                if(TryRead('='))
                    return CreateToken(SyntaxKind.LessEqualsToken);
                return CreateToken(SyntaxKind.LessToken);
            case '!':
                AddChar();
                if(TryRead('='))
                    return CreateToken(SyntaxKind.ExclamationEqualsToken);
                return CreateToken(SyntaxKind.ExclamationToken);
            case '(':
                return AddSymbolAndCreateToken(SyntaxKind.OpenParenToken);
            case ')':
                return AddSymbolAndCreateToken(SyntaxKind.CloseParenToken);
            case KnownCodes.RootIdentifier:
                return AddSymbolAndCreateToken(SyntaxKind.DollarMarkToken);
            case ':':
                return AddSymbolAndCreateToken(SyntaxKind.ColonToken);
            case ',':
                return AddSymbolAndCreateToken(SyntaxKind.CommaToken);
            case '[':
                return AddSymbolAndCreateToken(SyntaxKind.OpenBracketToken);
            case ']':
                return AddSymbolAndCreateToken(SyntaxKind.CloseBracketToken);
            case '*':
                return AddSymbolAndCreateToken(SyntaxKind.AsteriskToken);
            case '?':
                return AddSymbolAndCreateToken(SyntaxKind.QuestionMarkToken);
            case '@':
                return AddSymbolAndCreateToken(SyntaxKind.AtToken);
            case KnownCodes.SingleQuote:
                return StringLiteral(KnownCodes.SingleQuote, KnownCodes.DoubleQuote);
            case KnownCodes.DoubleQuote:
                return StringLiteral(KnownCodes.DoubleQuote, KnownCodes.SingleQuote);
            case KnownCodes.Dot:
                AddChar();
                if(TryRead(KnownCodes.Dot))
                    return CreateToken(SyntaxKind.DotDotToken);
                return CreateToken(SyntaxKind.DotToken);
        }
        if(TryRead(IsNameFirst))
        {
            ReadAll(IsNameChar);
            return CreateToken(SyntaxKind.MemberNameToken);
        }
        return AddSymbolAndCreateToken(SyntaxKind.Unknown);
    }
    Token AddSymbolAndCreateToken(SyntaxKind kind)
    {
        AddChar();
        return CreateToken(kind);
    }
    Token StringLiteral(int quoteCode, int unscapedQuoteCode)
    {
        AddChar();
        bool failed = false;
        while(!isEndOfStream)
            if(!(Unescaped() || TryRead(unscapedQuoteCode) || EscapeQuoteOrEscapable(quoteCode, out failed)) || failed)
                break;

        if(TryRead(quoteCode))
            return CreateToken(SyntaxKind.StringLiteralToken);
        return CreateToken(SyntaxKind.Unknown);
    }
    bool EscapeQuoteOrEscapable(int qouteCode, out bool failed)
    {
        failed = false;
        if(!TryRead(KnownCodes.BackSlash))
            return false;
        if(TryRead(qouteCode))
            return true;
        failed = !Escapeble();
        return true;
    }
    bool Escapeble()
    {
        if(isEndOfStream)
            return false;
        switch(codePoint)
        {
            case KnownCodes.b:         // %x62 / ; b BS backspace U+0008
            case KnownCodes.f:         // %x66 / ; f FF form feed U+000C
            case KnownCodes.n:         // %x6E / ; n LF line feed U+000A
            case KnownCodes.r:         // %x72 / ; r CR carriage return U+000D
            case KnownCodes.t:         // %x74 / ; t HT horizontal tab U+0009
            case KnownCodes.Slash:     // "/"  / ; / slash (solidus) U+002F
            case KnownCodes.BackSlash: // "\"  / ; \ backslash (reverse solidus) U+005C
                AddChar();
                return true;
            case KnownCodes.u:         // (%x75 hexchar) ;  uXXXX      U+XXXX
                AddChar();
                return HexChar();
        }
        return false;
    }
    private bool HexChar()
    {
        var nonSurrogate = TryReadSequence([
            s1 => s1 < 0x80 && KnownCodes.HexDigitsWithoutD.Contains((byte)s1),
            IsHexDigit,
            IsHexDigit,
            IsHexDigit], out var readed);
        if(nonSurrogate)
            return true;
        if(readed)
            return false;

        if(codePoint is not (KnownCodes.D or KnownCodes.d))
            return false;
        AddChar();
        return codePoint switch
        {
            '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' => TryReadSequence([
                IsHexDigit,
                IsHexDigit], out _),
            '8' or '9' or KnownCodes.A or KnownCodes.a or KnownCodes.B or KnownCodes.b => TryReadSequence([
                IsHexDigit,
                IsHexDigit,
                s6 => s6 is KnownCodes.BackSlash,
                s7 => s7 is KnownCodes.u,
                s8 => s8 is KnownCodes.D or 'd',
                s9 => s9 is KnownCodes.C or KnownCodes.D or KnownCodes.E or KnownCodes.F or 'c' or 'd' or 'e' or 'f',
                IsHexDigit,
                IsHexDigit], out _),
            _ => false,
        };
    }
    bool TryReadSequence(ReadOnlySpan<Predicate<int>> predicates, out bool readAny)
    {
        readAny = false;
        for(int i = 0; i < predicates.Length; i++)
        {
            if(isEndOfStream)
                return false;
            if(!predicates[i](codePoint))
                break;
            AddChar();
            readAny = true;
        }
        return true;
    }
    bool IsHexDigit(int codePoint) => codePoint < 0x80 && KnownCodes.HexDigits.Contains((byte)codePoint);


    bool Unescaped()
    {
        switch(codePoint)
        {
            case 0x20:
            case 0x21:
            case 0x23:
            case 0x24:
            case 0x25:
            case 0x26:
            case >= 0x28 and <= 0x5b:
            case >= 0x5D and <= 0xD7FF:
            case >= 0xE000 and <= 0x10FFFF:
                AddChar();
                return true;
            default: return false;
        }
    }
    bool IsNameFirst(int ch)
    {
        if(ch < 0x80)
        {
            if(KnownCodes.Alpha.Contains((byte)ch))
                return true;
            return ch == '_';
        }
        if(codePoint <= 0xD7FF)
            return true;
        return codePoint >= 0xE000 && codePoint <= 0x10FFFF;
    }

    bool IsNameChar(int ch)
    {
        return IsNameFirst(ch) || (ch < 0x80 && KnownCodes.Digits.Contains((byte)ch));
    }
    internal Token LookAhead()
    {
        return nextToken;
    }
}