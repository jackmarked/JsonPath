using System.Buffers;

namespace JetDevel.JsonPath.CodeAnalysis;

partial class Lexer
{
    static class KnownCodes
    {
        public static readonly SearchValues<byte> BlankSpaces = SearchValues.Create("\u0020\u0009\u000A\u000D"u8);
        public static readonly SearchValues<byte> HexDigits = SearchValues.Create("0123456789ABCDEFabcdef"u8);
        public static readonly SearchValues<byte> HexDigitsWithoutD = SearchValues.Create("0123456789ABCEFabcef"u8);
        public static readonly SearchValues<byte> Digits = SearchValues.Create("0123456789"u8);
        public static readonly SearchValues<byte> DigitsWitout0 = SearchValues.Create("123456789"u8);
        public static readonly SearchValues<byte> Alpha = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"u8);


        /// <summary>
        /// Dollar sign ('$'). 
        /// </summary>
        public const byte RootIdentifier = 0x24; // (byte)'$'; // U+0024	$	36	044	Dollar sign	0005
        /// <summary>
        /// Slash ('/').
        /// </summary>
        public const byte Slash = 0x2F; // "/"  / ; / slash (solidus) U+002F
        /// <summary>
        /// Back slash ('\').
        /// </summary>
        public const byte BackSlash = 0x5C; // \
        /// <summary>
        /// Single quote ("'").
        /// </summary>
        public const byte SingleQuote = 0x27; // '
        /// <summary>
        /// Double quote ('"').
        /// </summary>
        public const byte DoubleQuote = 0x22; // "
        /// <summary>
        /// Latin lowercase letter b ('b').
        /// </summary>
        public const byte b = 0x62; // %x62 / ; b BS backspace U+0008
        /// <summary>
        /// Latin lowercase letter f ('f').
        /// </summary>
        public const byte f = 0x66; // %x66 / ; f FF form feed U+000C
        /// <summary>
        /// Latin lowercase letter n ('n').
        /// </summary>
        public const byte n = 0x6E; // %x6E / ; n LF line feed U+000A
        /// <summary>
        /// Latin lowercase letter r ('r').
        /// </summary>
        public const byte r = 0x72; // %x72 / ; r CR carriage return U+000D
        /// <summary>
        /// Latin lowercase letter t ('t').
        /// </summary>
        public const byte t = 0x74; // %x74 / ; t HT horizontal tab U+0009
        /// <summary>
        /// Latin lowercase letter u ('u').
        /// </summary>
        public const byte u = 0x75; // %x75 ;  uXXXX      U+XXXX
        /// <summary>
        /// Latin uppercase letter A ('A').
        /// </summary>
        public const byte A = (byte)'A';
        /// <summary>
        /// Latin lowercase letter a ('a').
        /// </summary>
        public const byte a = (byte)'a';
        /// <summary>
        /// Latin uppercase letter B ('B').
        /// </summary>
        public const byte B = (byte)'B';
        /// <summary>
        /// Latin uppercase letter C ('C').
        /// </summary>
        public const byte C = (byte)'C';
        /// <summary>
        /// Latin uppercase letter D ('D').
        /// </summary>
        public const byte D = (byte)'D';
        /// <summary>
        /// Latin lowercase letter d ('d').
        /// </summary>
        public const byte d = (byte)'d';
        /// <summary>
        /// Latin uppercase letter E ('E').
        /// </summary>
        public const byte E = (byte)'E';
        /// <summary>
        /// Latin lowercase letter e ('e').
        /// </summary>
        public const byte e = (byte)'e';
        /// <summary>
        /// Latin uppercase letter F ('F').
        /// </summary>
        public const byte F = (byte)'F';
        /// <summary>
        /// Dot symbol ('.').
        /// </summary>
        public const byte Dot = (byte)'.';
    }
}