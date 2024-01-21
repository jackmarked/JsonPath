using System.Diagnostics.CodeAnalysis;

namespace JetDevel.JsonPath;

public readonly struct UnicodeCharacter(in int codePoint)
{
    public int CodePoint { get; } = codePoint;
    public string Symbol => char.ConvertFromUtf32(CodePoint);
    public static readonly UnicodeCharacter Invalid = new(-1);
    public static readonly UnicodeCharacter EndOfStream = new(int.MinValue);
    public static bool operator !=(UnicodeCharacter left, UnicodeCharacter right) => !left.EqualsInternal(right);
    public static bool operator ==(UnicodeCharacter left, UnicodeCharacter right) => left.EqualsInternal(right);
    public override int GetHashCode() => HashCode.Combine(CodePoint);
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is UnicodeCharacter chrarcter && EqualsInternal(chrarcter);
    bool EqualsInternal(UnicodeCharacter other) => CodePoint == other.CodePoint;
    public static implicit operator UnicodeCharacter(int codePoint) => new(codePoint);
    public override string ToString() => Symbol;
}