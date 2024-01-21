using System.Buffers;
using System.Text;

namespace JetDevel.JsonPath.CodeAnalysis;

public sealed partial class Lexer
{
    readonly byte[] source;
    readonly static SearchValues<byte> blankSpaces = SearchValues.Create("\u0020\u0009\u000A\u000D"u8);

    readonly static SearchValues<byte> digits1 = SearchValues.Create("123456789"u8);
    readonly static SearchValues<byte> alpha = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"u8);
    Token nextToken;
    int startPosition;
    int currentPosition;
    int maxPosition;
    int sourceLength;
    int codePoint;
    int symbolLength;
#if DEBUG
    public string Symbol => char.ConvertFromUtf32(codePoint);
#endif
    public Lexer(string source) : this(Encoding.UTF8.GetBytes(source ?? throw new ArgumentNullException(nameof(source))))
    {
    }
    public Lexer(ReadOnlySpan<byte> utf8bytes) : this(utf8bytes.ToArray())
    {
    }
    static Utf8SmbolKind[] symbolLengthTable = new Utf8SmbolKind[256];
    static Lexer()
    {
        for(int i = 0; i < symbolLengthTable.Length; i++)
            symbolLengthTable[i] = GetSymbolLengthKindInternal((byte)i);
    }
    internal Lexer(byte[] source)
    {
        ArgumentNullException.ThrowIfNull(source);
        this.source = source;
        sourceLength = source.Length;
        maxPosition = sourceLength - 1;
        AddChar();
        if(StartWithBom())
        {
            nextToken = new Token(SyntaxKind.ByteOrderMark, source.AsSpan()[..3]);
            AddChar();
        }
        else
            nextToken = Scan();
    }

    bool StartWithBom() =>
        sourceLength > 2 && source[0] == 0xEF && source[1] == 0xBB && source[2] == 0xBF;

    void AddChar()
    {
        currentPosition += symbolLength;
        symbolLength = 1;
        if(IsEof())
            return;
        var octet = source[currentPosition];
        if(octet < 0x80)
            codePoint = octet;
        else
            codePoint = GetMultiOctetSymbol(octet);
    }
    int GetMultiOctetSymbol(byte firstOctet)
    {
        var kind = GetSymbolLengthKind(firstOctet);
        if(kind == Utf8SmbolKind.Unexpectd || kind == Utf8SmbolKind.OneOctet)
            return firstOctet;
        var secondOctet = LookNextOctet(1);
        if(!IsValidNotFistSymbolOctet(secondOctet))
            return firstOctet;
        symbolLength = (int)kind;
        if(kind == Utf8SmbolKind.TwoOctets)
            return GetUnicodeSymbolCode(firstOctet, secondOctet);
        var thirdOctet = LookNextOctet(2);
        if(!IsValidNotFistSymbolOctet(thirdOctet))
            return firstOctet;
        if(kind == Utf8SmbolKind.ThreeOctets)
            return GetUnicodeSymbolCode(firstOctet, secondOctet, thirdOctet);
        var forthOctet = LookNextOctet(3);
        if(!IsValidNotFistSymbolOctet(forthOctet))
            return firstOctet;
        return GetUnicodeSymbolCode(firstOctet, secondOctet, thirdOctet, forthOctet);
    }
    void AddChars(int count)
    {
        currentPosition += count;
        if(IsNotEof())
            codePoint = source[currentPosition];
    }
    byte LookNextOctet(int index)
    {
        var sourceIndex = currentPosition + index;
        if(sourceIndex < sourceLength)
            return source[sourceIndex];
        else
            throw new InvalidOperationException("Unexpected end of file.");
    }
    bool IsNotEof()
    {
        return currentPosition < sourceLength;
    }
    bool IsEof()
    {
        return currentPosition > maxPosition;
    }
    Token CreateToken(SyntaxKind kind)
    {
        var token = new Token(kind, source.AsSpan(startPosition, currentPosition - startPosition));
        startPosition = currentPosition;
        return token;
    }
    void SkipWhiteSpaces()
    {
        while(TryRead(blankSpaces))
            startPosition = currentPosition;
    }
    public Token GetNextToken()
    {
        var result = nextToken;
        nextToken = Scan();
        return result;
    }
    bool TryRead(int value)
    {
        if(IsEof() || codePoint != value)
            return false;
        AddChar();
        return true;
    }
    bool TryRead(SearchValues<byte> values)
    {
        if(IsEof() || codePoint > 0x7f || !values.Contains((byte)codePoint))
            return false;
        AddChar();
        return true;
    }
    bool TryRead(Func<int, bool> predicate)
    {
        if(IsEof() || !predicate(codePoint))
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
        if(IsEof())
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

        while(IsNotEof())
            if(!(Unescaped() || TryRead(KnownOctets.DoubleQuote) || EscapeSingleQuoteOrEscapable()))
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
        while(IsNotEof())
            if(!(Unescaped() || TryRead(KnownOctets.SingleQuote) || EscapeDoubleQuoteOrEscapable()))
                break;

        if(TryRead(KnownOctets.DoubleQuote))
            return CreateToken(SyntaxKind.StringLiteralToken);
        return CreateToken(SyntaxKind.Unknown);

        /*
        double-quoted       = unescaped /
                              %x27      /                    ; '
                              ESC %x22  /                    ; \"
                              ESC escapable
         */
    }
    bool EscapeDoubleQuoteOrEscapable()
    {
        if(!TryRead(KnownOctets.BackSlash))
            return false;
        if(TryRead(KnownOctets.DoubleQuote))
            return true;
        return Escapeble();
    }
    bool EscapeSingleQuoteOrEscapable()
    {
        if(!TryRead(KnownOctets.BackSlash))
            return false;
        if(TryRead(KnownOctets.SingleQuote))
            return true;
        return Escapeble();
    }
    bool Escapeble()
    {
        if(IsEof())
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

    private bool HexChar()
    {
        return Surrogate() || NonSurrogate();
        /*
hexchar             = non-surrogate /
                  (high-surrogate "\" %x75 low-surrogate)
non-surrogate       = ((DIGIT / "A"/"B"/"C" / "E"/"F") 3HEXDIG) /
                   ("D" %x30-37 2HEXDIG )
high-surrogate      = "D" ("8"/"9"/"A"/"B") 2HEXDIG
low-surrogate       = "D" ("C"/"D"/"E"/"F") 2HEXDIG

HEXDIG              = DIGIT / "A" / "B" / "C" / "D" / "E" / "F"
*/
    }

    private bool NonSurrogate()
    {
        if(codePoint == KnownOctets.D || codePoint == 'd') // ("D" %x30-37 2HEXDIG )
        {
            var next = LookNextOctet(1);
            if(next < 0x30 || next > 0x37)
                return false;
            next = LookNextOctet(2);
            if(!KnownOctets.HexDigits.Contains(next))
                return false;
            next = LookNextOctet(3);
            if(KnownOctets.HexDigits.Contains(next))
            {
                AddChars(3);
                return true;
            }
            return false;

        }
        if(codePoint < 0x80 && KnownOctets.HexDigitsWithoutD.Contains((byte)codePoint)) // ((DIGIT / "A"/"B"/"C" / "E"/"F") 3HEXDIG)
        {
            var next = LookNextOctet(1);
            if(!KnownOctets.HexDigits.Contains(next))
                return false;
            next = LookNextOctet(2);
            if(!KnownOctets.HexDigits.Contains(next))
                return false;
            next = LookNextOctet(3);
            if(KnownOctets.HexDigits.Contains(next))
            {
                AddChars(3);
                return true;
            }
        }
        /*
non-surrogate       =  ("D" %x30-37 2HEXDIG ) /
                   ((DIGIT / "A"/"B"/"C" / "E"/"F") 3HEXDIG)

         */
        return false;
    }

    private bool Surrogate()
    {
        if(codePoint != KnownOctets.D)
            return false;

        var next = LookNextOctet(1);
        if(!(next is (byte)'8' or (byte)'9' or KnownOctets.A or KnownOctets.B))
            return false;
        if(!KnownOctets.HexDigits.Contains(LookNextOctet(2)))
            return false;
        if(!KnownOctets.HexDigits.Contains(LookNextOctet(3)))
            return false;
        if(LookNextOctet(4) != KnownOctets.BackSlash)
            return false;
        if(LookNextOctet(5) != KnownOctets.u)
            return false;
        if(LookNextOctet(6) != KnownOctets.D)
            return false;
        if(!(LookNextOctet(7) is KnownOctets.C or KnownOctets.D or KnownOctets.E or KnownOctets.F))
            return false;
        if(!KnownOctets.HexDigits.Contains(LookNextOctet(8)))
            return false;
        if(!KnownOctets.HexDigits.Contains(LookNextOctet(9)))
            return false;
        AddChars(10);
        return true;
        /*
surrogate           = high-surrogate "\" %x75 low-surrogate
high-surrogate      = "D" ("8"/"9"/"A"/"B") 2HEXDIG
low-surrogate       = "D" ("C"/"D"/"E"/"F") 2HEXDIG
HEXDIG              = DIGIT / "A" / "B" / "C" / "D" / "E" / "F"
         */
    }

    static Utf8SmbolKind GetSymbolLengthKind(byte octet)
    {
        return symbolLengthTable[octet];
    }
    static Utf8SmbolKind GetSymbolLengthKindInternal(byte octet)
    {
        if(IsOneOctetSymbol(octet))
            return Utf8SmbolKind.OneOctet;
        if(IsTwoOctetFrstSymbol(octet))
            return Utf8SmbolKind.TwoOctets;
        if(IsThreeOctetFrstSymbol(octet))
            return Utf8SmbolKind.ThreeOctets;
        if(IsForeOctetFrstSymbol(octet))
            return Utf8SmbolKind.ForeOctets;
        return Utf8SmbolKind.Unexpectd;
    }
    static bool IsOneOctetSymbol(byte octet)
    {
        byte mask = 0b10000000;
        return (mask & octet) == 0;
    }
    static bool IsTwoOctetFrstSymbol(byte octet)
    {
        byte mask = 0b1110_0000;
        byte result = 0b1100_0000; // 110xxxxx
        return (mask & octet) == result;
    }
    static bool IsThreeOctetFrstSymbol(byte octet)
    {
        byte mask = 0b1111_0000;
        byte result = 0b1110_0000; // 1110xxxx
        return (mask & octet) == result;
    }
    static bool IsForeOctetFrstSymbol(byte octet)
    {
        byte mask = 0b1111_1000;
        byte result = 0b1111_0000; // 11110xxx
        return (mask & octet) == result;
    }
    bool IsValidNotFistSymbolOctet(byte octet)
    {
        byte mask = 0b1100_0000;
        byte result = 0b1000_0000; // 10xxxxxx
        return (mask & octet) == result;
    }

    private int GetUnicodeSymbolCode(int firstOctet, int secondOctet, int thirdOctet, int forthOctet)
    {
        var result =
            ((firstOctet & 0b0000_0111) << 18) |
            ((secondOctet & 0b0011_1111) << 12) |
            ((thirdOctet & 0b0011_1111) << 6) |
            (forthOctet & 0b0011_1111);
        return result;
        // 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
    }

    private int GetUnicodeSymbolCode(int firstOctet, int secondOctet, int thirdOctet)
    {

        var result =
            ((firstOctet & 0b0000_1111) << 12) |
            ((secondOctet & 0b0011_1111) << 6) |
            (thirdOctet & 0b0011_1111);
        return result;
        // 1110xxxx 10xxxxxx 10xxxxxx
    }

    private int GetUnicodeSymbolCode(int firstOctet, int secondOctet)
    {
        int result = firstOctet & 0b0001_1111;
        result <<= 6;
        result += (secondOctet & 0b0011_1111);
        return result;
        // 	110xxxxx 10xxxxxx
    }

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