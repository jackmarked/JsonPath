namespace JetDevel.JsonPath;
public sealed class Utf8BytesUnicodeCharacterReader: UnicodeCharacterReader
{
    readonly byte[] bytes;
    int offset;
    readonly int maxOffset;
    static readonly Utf8SmbolKind[] symbolLengthTable = new Utf8SmbolKind[256];
    static Utf8BytesUnicodeCharacterReader()
    {
        for(int i = 0; i < symbolLengthTable.Length; i++)
            symbolLengthTable[i] = GetSymbolLengthKindInternal((byte)i);
    }
    public Utf8BytesUnicodeCharacterReader(byte[] bytes)
    {
        this.bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        this.offset = 0;
        maxOffset = bytes.Length - 1;
        SkipByteOrderMark();
    }
    void SkipByteOrderMark()
    {
        const int byteOrderMarkLength = 3;
        if(maxOffset > 1 && bytes.AsSpan(0, byteOrderMarkLength) is [0xEF, 0xBB, 0xBF])
            offset = byteOrderMarkLength;
    }
    public override bool TryReadNext(out UnicodeCharacter character)
    {
        character = UnicodeCharacter.EndOfStream;
        if(!TryReadNextOctet(out var firstOctet))
            return false;
        if(firstOctet < 0x80)
        {
            character = firstOctet;
            return true;
        }
        character = GetMultiOctetSymbol(firstOctet);
        return character != UnicodeCharacter.EndOfStream;
    }
    UnicodeCharacter GetMultiOctetSymbol(byte firstOctet)
    {
        var kind = GetSymbolLengthKind(firstOctet);
        if(kind == Utf8SmbolKind.Unknown)
            return UnicodeCharacter.Invalid;
        if(kind == Utf8SmbolKind.OneOctet)
            return firstOctet;

        if(!TryReadNextOctet(out var secondOctet))
            return UnicodeCharacter.EndOfStream;
        if(!IsValidNotFistSymbolOctet(secondOctet))
            return UnicodeCharacter.Invalid;
        if(kind == Utf8SmbolKind.TwoOctets)
            return GetUnicodeSymbolCode(firstOctet, secondOctet);

        if(!TryReadNextOctet(out var thirdOctet))
            return UnicodeCharacter.EndOfStream;
        if(!IsValidNotFistSymbolOctet(thirdOctet))
            return UnicodeCharacter.Invalid;
        if(kind == Utf8SmbolKind.ThreeOctets)
            return GetUnicodeSymbolCode(firstOctet, secondOctet, thirdOctet);

        if(!TryReadNextOctet(out var forthOctet))
            return UnicodeCharacter.EndOfStream;
        if(!IsValidNotFistSymbolOctet(forthOctet))
            return UnicodeCharacter.Invalid;
        return GetUnicodeSymbolCode(firstOctet, secondOctet, thirdOctet, forthOctet);
    }
    static int GetUnicodeSymbolCode(int firstOctet, int secondOctet, int thirdOctet, int forthOctet)
    {
        var result = // 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
            ((firstOctet & 0b0000_0111) << 18) |
            ((secondOctet & 0b0011_1111) << 12) |
            ((thirdOctet & 0b0011_1111) << 6) |
            (forthOctet & 0b0011_1111);
        return result;
    }
    static int GetUnicodeSymbolCode(int firstOctet, int secondOctet, int thirdOctet)
    {
        var result = // 1110xxxx 10xxxxxx 10xxxxxx
            ((firstOctet & 0b0000_1111) << 12) |
            ((secondOctet & 0b0011_1111) << 6) |
            (thirdOctet & 0b0011_1111);
        return result;
    }
    static int GetUnicodeSymbolCode(int firstOctet, int secondOctet)
    {
        var result = // 110xxxxx 10xxxxxx
            ((firstOctet & 0b0001_1111) << 6) |
            (secondOctet & 0b0011_1111);
        return result;
    }
    bool TryReadNextOctet(out byte octet)
    {
        octet = byte.MaxValue;
        if(offset > maxOffset)
            return false;
        octet = bytes[offset++];
        return true;
    }
    static Utf8SmbolKind GetSymbolLengthKind(byte octet)
    {
        return symbolLengthTable[octet];
    }
    enum Utf8SmbolKind
    {
        Unknown = 0,
        OneOctet = 1,
        TwoOctets = 2,
        ThreeOctets = 3,
        ForeOctets = 4
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
        return Utf8SmbolKind.Unknown;
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
    static bool IsValidNotFistSymbolOctet(byte octet)
    {
        byte mask = 0b1100_0000;
        byte result = 0b1000_0000; // 10xxxxxx
        return (mask & octet) == result;
    }
}