using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace JetDevel.JsonPath.CodeAnalysis;

public readonly struct Token
{
    public Token(SyntaxKind kind, ReadOnlySpan<byte> utf8Text = default)
    {
        Kind = kind;
        Text = SyntaxFacts.GetText(kind);
        if(!string.IsNullOrEmpty(Text))
            return;
        var length = utf8Text.Length;
        var chars = length > 80 ? new char[length] : stackalloc char[length];
        Utf8.ToUtf16(utf8Text, chars, out var read, out var written);
        Text = new string(chars[..written]);
    }
    public Token(SyntaxKind kind, ReadOnlySpan<int> utf32Text)
    {
        Kind = kind;
        Text = SyntaxFacts.GetText(kind);
        if(string.IsNullOrEmpty(Text))
            Text = Encoding.UTF32.GetString(MemoryMarshal.Cast<int, byte>(utf32Text));
    }
    public SyntaxKind Kind { get; }
    public string Text { get; }
    public override readonly string ToString() => @$"Kind: {Kind}, Text: ""{Text}""";
}