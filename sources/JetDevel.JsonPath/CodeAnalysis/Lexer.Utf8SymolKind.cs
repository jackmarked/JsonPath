namespace JetDevel.JsonPath.CodeAnalysis;

partial class Lexer
{
    enum Utf8SmbolKind
    {
        Unexpectd = 0,
        OneOctet = 1,
        TwoOctets = 2,
        ThreeOctets = 3,
        ForeOctets = 4
    }
}