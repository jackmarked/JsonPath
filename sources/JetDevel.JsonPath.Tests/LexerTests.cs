using System.Text;
using JetDevel.JsonPath.CodeAnalysis;

namespace JetDevel.JsonPath.Tests;

sealed class LexerTests
{
    static List<Token> GetTokens(string source, bool withEof = false)
    {
        var utf8Bytes = Encoding.UTF8.GetBytes(source);
        return GetTokens(utf8Bytes, withEof);
    }
    static List<Token> GetTokens(ReadOnlySpan<byte> utf8Bytes, bool withEof = false)
    {
        var lexer = new Lexer(utf8Bytes);
        var result = new List<Token>();
        Token token;
        do
        {
            token = lexer.GetNextToken();
            result.Add(token);
        }
        while(token.Kind != SyntaxKind.EndOfFile);
        if(withEof)
            return result;
        else
            result.RemoveAt(result.Count - 1);
        return result;
    }

    static List<SyntaxKind> GetTokenKinds(string source, bool withEof = false)
    {
        var result = GetTokens(source, withEof);
        return result.Select(s => s.Kind).ToList();
    }
    static List<SyntaxKind> GetTokenKinds(ReadOnlySpan<byte> source, bool withEof = false)
    {
        var result = GetTokens(source, withEof);
        return result.Select(s => s.Kind).ToList();
    }
    static void AssertTokenKinds(string source, params SyntaxKind[] expectedKinds)
    {
        // Act.
        var syntaxKinds = GetTokenKinds(source);

        // Assert.
        Assert.That(syntaxKinds, Is.EquivalentTo(expectedKinds));
    }
    static void AssertTokenKinds(ReadOnlySpan<byte> source, params SyntaxKind[] expectedKinds)
    {
        // Act.
        var syntaxKinds = GetTokenKinds(source);

        // Assert.
        Assert.That(syntaxKinds, Is.EquivalentTo(expectedKinds));
    }

    [Test]
    public void Constructor_CallWithNull_ThrowsArgumentNullExeption()
    {
        // Arange.
        string value = null;

        // Act.
        Lexer action() => new(value);

        // Assert.
        Assert.That((Func<Lexer>)action, Throws.ArgumentNullException);
    }
    [Test]
    public void Parse_BracketedSelectorWithNameSelectorUnicode_ReturnsValidSelectors()
    {

        var result = GetTokens("$.🙏");
        Assert.Multiple(() =>
        {
            Assert.That(result[^1].Text, Is.EqualTo("🙏"));
        });
    }
    [Test]
    public void GetTokens_CallWithEmptyString_ReturnsEndOfFile()
    {
        // Arange.
        var value = string.Empty;
        var lexer = new Lexer(value);

        // Act.
        var token = lexer.GetNextToken();

        // Assert.
        Assert.That(token.Kind, Is.EqualTo(SyntaxKind.EndOfFile));
    }
    [Test]
    public void GetTokens_CallWithDollar_ReturnsRootIdentifierToken()
    {
        // Arange.
        var value = "$";
        var lexer = new Lexer(value);
        SyntaxKind[] expectedKinds = [SyntaxKind.DollarMarkToken];

        // Act.
        var kinds = GetTokenKinds(value);

        // Assert.
        Assert.That(kinds, Is.EquivalentTo(expectedKinds));
    }
    [Test]
    public void GetTokens_CallWithDollarDotAsterisk_ReturnsValidTokens()
    {
        // Arange.
        var source = "$.*";
        SyntaxKind[] expectedKinds = [SyntaxKind.DollarMarkToken, SyntaxKind.DotToken, SyntaxKind.AsteriskToken];

        // Assert.
        AssertTokenKinds(source, expectedKinds);
    }
    [Test]
    public void GetTokens_CallWithBraketed_ReturnsValidTokens()
    {
        // Arange.
        var source = "$[123489]";

        // Assert.
        AssertTokenKinds(source, [SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.CloseBracketToken]);
    }
    [Test]
    public void GetTokens_CallWithMultiIndex_ReturnsValidTokens()
    {
        // Arange.
        var source = "$[1, 2, 8]";

        // Assert.
        AssertTokenKinds(source, [SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.CommaToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.CommaToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.CloseBracketToken]);
    }
    [Test]
    public void GetTokens_CallWithMemberName_ReturnsValidTokens()
    {
        // Arange.
        var source = "$.abab";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.DotToken,
            SyntaxKind.MemberNameToken);
    }
    [Test]
    public void GetTokens_CallWithSlice_ReturnsValidTokens()
    {
        // Arange.
        var source = "$[1:2:3]";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.ColonToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.ColonToken,
            SyntaxKind.IntegerNumberLiteral,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStirngLitersl_ReturnsValidTokens()
    {
        // Arange.
        var source = "$['ab']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithDoubleQuotedStirngLiteral_ReturnsValidTokens()
    {
        // Arange.
        var source = @" ""ab"" ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.StringLiteralToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStirngLiterslWithEscapable_ReturnsValidTokens()
    {
        // Arange.
        var source = """$['a\b\f\n\r\t\/\'']""";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStirngLiterslWithHexEscapable_ReturnsValidTokens()
    {
        // Arange.
        var source = "$['a\\uA123']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStirngLiterslWithSingleQuoteEscaped_ReturnsValidTokens()
    {
        // Arange.
        var source = "$['a\\'']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStirngLiterslWithDoubleQuote_ReturnsValidTokens()
    {
        // Arange.
        var source = @"$['a""']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithBomAndRootIdentifier_ReturnsValidTokens()
    {
        /*
         
Для указания, что файл или поток содержит символы Юникода, в начале файла или потока может быть вставлен маркер последовательности байтов (англ. Byte order mark, BOM), который в случае кодирования в UTF-8 принимает форму трёх байтов: EF BB BF16.

                  1-й байт     2-й байт     3-й байт
Двоичный код          1110 1111    1011 1011    1011 1111
Шестнадцатеричный код        EF           BB           BF
         */
        // Arange.
        var source = new byte[] { 0xEF, 0xBB, 0xBF, (byte)'$' };

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.ByteOrderMark,
            SyntaxKind.DollarMarkToken);
    }
    [Test]
    public void GetTokens_CallWithCompOperatorTokens_ReturnsValidTokens()
    {
        // Arange.
        var source = "! < > == != <= >= ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.ExclamationToken,
            SyntaxKind.LessToken,
            SyntaxKind.GreaterToken,
            SyntaxKind.EqualsEqualsToken,
            SyntaxKind.ExclamationEqualsToken,
            SyntaxKind.LessEqualsToken,
            SyntaxKind.GreaterEqualsToken);
    }
    [Test]
    public void GetTokens_CallWithLogcalOperatorsText_ReturnsValidTokens()
    {
        // Arange.
        var source = " ! || && ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.ExclamationToken,
            SyntaxKind.BarBarToken,
            SyntaxKind.AmpersandAmpersandToken);
    }
    [Test]
    public void GetTokens_CallWithIntegerText_ReturnsValidTokens()
    {
        // Arange.
        var source = " 123456 ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.IntegerNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithZeroText_ReturnsValidTokens()
    {
        // Arange.
        var source = " 0 ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.IntegerNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithNegativeInteger_ReturnsValidTokens()
    {
        // Arange.
        var source = " -7 ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.IntegerNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithNegativeFloat_ReturnsFlatTokens()
    {
        // Arange.
        var source = " -0.7123E+54";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithPositiveFloat_ReturnsFlatTokens()
    {
        // Arange.
        var source = " 0.9824e-38";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithNegativeZero_ReturnsUnknown()
    {
        // Arange.
        var source = " -0 ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test]
    public void GetTokens_CallWithFractionNumberOnly_ReturnsParen()
    {
        // Arange.
        var source = " -0.76876 ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithParen_ReturnsValidTokens()
    {
        // Arange.
        var source = " ( ) ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken);
    }
    [Test]
    public void GetTokens_CallWithExponentNumberOnly_ReturnsFloatNumberLiteral()
    {
        // Arange.
        var source = " 122E6 ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test/*, Ignore("todo")*/]
    public void GetTokens_CallWithWrongSingleQuotedStringLiteral_ReturnsUnknown()
    {
        // Arange.
        var source = """ '\' """;

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test/*, Ignore("todo")*/]
    public void GetTokens_CallWithWrongDoubleQuotedStringLiteral_ReturnsUnknown()
    {
        // Arange.
        var source = """ "\" """;

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test/*, Ignore("todo")*/]
    public void GetTokens_CallWithWrongDoubleEscapedStringLiteral_ReturnsUnknown()
    {
        // Arange.
        var source = """ "\ud""";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test/*, Ignore("todo")*/]
    public void GetTokens_CallWithWrongDoubleEscapedStringLiteral_ReturnsUnknown1()
    {
        // Arange.
        var source = """##""";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown, SyntaxKind.Unknown);
    }
}