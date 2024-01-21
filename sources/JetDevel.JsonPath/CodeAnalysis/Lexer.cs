using System.Buffers;
using System.Runtime.InteropServices;

namespace JetDevel.JsonPath.CodeAnalysis;

public sealed partial class Lexer
{
    readonly UnicodeCharacterReader source;
    readonly static SearchValues<byte> blankSpaces = SearchValues.Create("\u0020\u0009\u000A\u000D"u8);

    readonly static SearchValues<byte> digits1 = SearchValues.Create("123456789"u8);
    readonly static SearchValues<byte> alpha = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"u8);
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
        ReadAll(blankSpaces);
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
            if(!ReadAny(KnownOctets.Digits))
            {
                token = CreateToken(SyntaxKind.Unknown);
                return true;
            }
            hasFraction = true;
        }
        var hasExponent = codePoint == 'e' || codePoint == 'E';
        if(!hasExponent)
        {
            if(hasFraction)
                token = CreateToken(SyntaxKind.FloatNumberLiteral);
            return hasFraction;
        }
        AddChar();
        _ = TryRead('-') || TryRead('+');
        if(ReadAny(KnownOctets.Digits))
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
                ReadAll(KnownOctets.Digits);
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
                if(!TryRead(digits1))
                    return CreateToken(SyntaxKind.Unknown);
                ReadAll(KnownOctets.Digits);
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
            case KnownOctets.RootIdentifier:
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
            case KnownOctets.SingleQuote:
                return SingleQuotedStringLiteral();
            case KnownOctets.DoubleQuote:
                return DoubleQuotedStringLiteral();
            case KnownOctets.Dot:
                AddChar();
                if(TryRead(KnownOctets.Dot))
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


    /*
name-selector       = string-literal

string-literal      = %x22 *double-quoted %x22 /     ; "string"
         %x27 *single-quoted %x27       ; 'string'



ESC                 = %x5C                           ; \  backslash

unescaped           = %x20-21 /                      ; see RFC 8259
            ; omit 0x22 "
         %x23-26 /
            ; omit 0x27 '
         %x28-5B /
            ; omit 0x5C \
         %x5D-D7FF /   ; skip surrogate code points
         %xE000-10FFFF

escapable           = %x62 / ; b BS backspace U+0008
         %x66 / ; f FF form feed U+000C
         %x6E / ; n LF line feed U+000A
         %x72 / ; r CR carriage return U+000D
         %x74 / ; t HT horizontal tab U+0009
         "/"  / ; / slash (solidus) U+002F
         "\"  / ; \ backslash (reverse solidus) U+005C
         (%x75 hexchar) ;  uXXXX      U+XXXX

hexchar             = non-surrogate /
         (high-surrogate "\" %x75 low-surrogate)
non-surrogate       = ((DIGIT / "A"/"B"/"C" / "E"/"F") 3HEXDIG) /
          ("D" %x30-37 2HEXDIG )
high-surrogate      = "D" ("8"/"9"/"A"/"B") 2HEXDIG
low-surrogate       = "D" ("C"/"D"/"E"/"F") 2HEXDIG

HEXDIG              = DIGIT / "A" / "B" / "C" / "D" / "E" / "F"
*/
    void Expect(byte expectedOctet)
    {
        if(codePoint != expectedOctet)
            throw new InvalidOperationException($"Expected {expectedOctet} but was {codePoint}.");
        AddChar();
    }
    Token SingleQuotedStringLiteral()
    {
        Expect(KnownOctets.SingleQuote);
        bool failed = false;
        while(!isEndOfStream)
            if(!(Unescaped() || TryRead(KnownOctets.DoubleQuote) || EscapeSingleQuoteOrEscapable(out failed)) || failed)
                break;
        if(TryRead(KnownOctets.SingleQuote))
            return CreateToken(SyntaxKind.StringLiteralToken);
        return CreateToken(SyntaxKind.Unknown);
        /*
        single-quoted       = unescaped /
                              %x22      /                    ; "
                              ESC %x27  /                    ; \'
                              ESC escapable
         */
    }
    Token DoubleQuotedStringLiteral()
    {
        Expect(KnownOctets.DoubleQuote);
        bool failed = false;
        while(!isEndOfStream)
            if(!(Unescaped() || TryRead(KnownOctets.SingleQuote) || EscapeDoubleQuoteOrEscapable(out failed)) || failed)
                break;

        if(TryRead(KnownOctets.DoubleQuote))
            return CreateToken(SyntaxKind.StringLiteralToken);
        return CreateToken(SyntaxKind.Unknown);
    }
    bool EscapeDoubleQuoteOrEscapable(out bool failed)
    {
        failed = false;
        if(!TryRead(KnownOctets.BackSlash))
            return false;
        if(TryRead(KnownOctets.DoubleQuote))
            return true;
        failed = !Escapeble();
        return true;
    }
    bool EscapeSingleQuoteOrEscapable(out bool failed)
    {
        failed = false;
        if(!TryRead(KnownOctets.BackSlash))
            return false;
        if(TryRead(KnownOctets.SingleQuote))
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
            case KnownOctets.b:         // %x62 / ; b BS backspace U+0008
            case KnownOctets.f:         // %x66 / ; f FF form feed U+000C
            case KnownOctets.n:         // %x6E / ; n LF line feed U+000A
            case KnownOctets.r:         // %x72 / ; r CR carriage return U+000D
            case KnownOctets.t:         // %x74 / ; t HT horizontal tab U+0009
            case KnownOctets.Slash:     // "/"  / ; / slash (solidus) U+002F
            case KnownOctets.BackSlash: // "\"  / ; \ backslash (reverse solidus) U+005C
                AddChar();
                return true;
            case KnownOctets.u:         // (%x75 hexchar) ;  uXXXX      U+XXXX
                AddChar();
                return HexChar();
        }
        return false;
    }
    /// <summary>
    /// hexchar = non-surrogate / (high-surrogate "\" %x75 low-surrogate)
    /// </summary>
    /// <returns></returns>
    private bool HexChar()
    {
        var nonSurrogate = TryReadSequence([
            s1 => s1 < 0x80 && KnownOctets.HexDigitsWithoutD.Contains((byte)s1),
            IsHexDigit,
            IsHexDigit,
            IsHexDigit], out var readed);
        if(nonSurrogate)
            return true;
        if(readed)
            return false;

        if(codePoint is not (KnownOctets.D or KnownOctets.d))
            return false;
        AddChar();
        return codePoint switch
        {
            '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' =>TryReadSequence([
                IsHexDigit,
                IsHexDigit], out _),
            '8' or '9' or KnownOctets.A or KnownOctets.a or KnownOctets.B or KnownOctets.b => TryReadSequence([
                IsHexDigit,
                IsHexDigit,
                s6 => s6 is KnownOctets.BackSlash,
                s7 => s7 is KnownOctets.u,
                s8 => s8 is KnownOctets.D or 'd',
                s9 => s9 is KnownOctets.C or KnownOctets.D or KnownOctets.E or KnownOctets.F or 'c' or 'd' or 'e' or 'f',
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
    bool IsHexDigit(int codePoint) => codePoint < 0x80 && KnownOctets.HexDigits.Contains((byte)codePoint);
    /// <summary>
    /// non-surrogate       = ((DIGIT / "A"/"B"/"C" / "E"/"F") 3HEXDIG) / ("D" %x30-37 2HEXDIG )
    /// </summary>
    /// <returns></returns>


    /// <summary>
    /// high-surrogate      = "D" ("8"/"9"/"A"/"B") 2HEXDIG
    /// low-surrogate       = "D" ("C"/"D"/"E"/"F") 2HEXDIG
    /// </summary>
    /// <returns></returns>


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
        }
        return false;
        /*
unescaped           = %x20-21 /                      ; see RFC 8259
                         ; omit 0x22 "
                      %x23-26 /
                         ; omit 0x27 '
                      %x28-5B /
                         ; omit 0x5C \
                      %x5D-D7FF /   ; skip surrogate code points
                      %xE000-10FFFF
         */
    }
    /*

int                 = "0" /
         (["-"] DIGIT1 *DIGIT)      ; - optional
           DIGIT1              = %x31-39                    ; 1-9 non-zero digit

*/
    bool IsNameFirst(int ch)
    {
        if(ch < 0x80)
        {
            if(alpha.Contains((byte)ch))
                return true;
            return ch == '_';
        }
        if(/*codePoint >= 0x80 &&*/ codePoint <= 0xD7FF)
            return true;
        return codePoint >= 0xE000 && codePoint <= 0x10FFFF;
    }

    bool IsNameChar(int ch)
    {
        return IsNameFirst(ch) || (ch < 0x80 && KnownOctets.Digits.Contains((byte)ch));
    }
    /*
    member-name-shorthand = name-first *name-char
    name-first          = ALPHA /
                          "_"   /
                          %x80-D7FF /   ; skip surrogate code points
                          %xE000-10FFFF
    name-char           = DIGIT / name-first

    DIGIT               = %x30-39              ; 0-9
    ALPHA               = %x41-5A / %x61-7A    ; A-Z / a-z
             */
    internal Token LookAhead()
    {
        return nextToken;
    }
}