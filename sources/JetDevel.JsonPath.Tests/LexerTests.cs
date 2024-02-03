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
        var lexer = new Lexer(new Utf8BytesUnicodeCharacterReader(utf8Bytes.ToArray()));
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
        // Arrange.
        UnicodeCharacterReader value = null;

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
    public void GetTokens_CallWithEmptyReader_ReturnsEndOfFile()
    {
        // Arrange.
        var value = new Utf8BytesUnicodeCharacterReader([]);
        var lexer = new Lexer(value);

        // Act.
        var token = lexer.GetNextToken();

        // Assert.
        Assert.That(token.Kind, Is.EqualTo(SyntaxKind.EndOfFile));
    }
    [Test]
    public void GetTokens_CallWithDollar_ReturnsRootIdentifierToken()
    {
        // Arrange.
        var value = "$";
        SyntaxKind[] expectedKinds = [SyntaxKind.DollarMarkToken];

        // Act.
        var kinds = GetTokenKinds(value);

        // Assert.
        Assert.That(kinds, Is.EquivalentTo(expectedKinds));
    }
    [Test]
    public void GetTokens_CallWithDollarDotAsterisk_ReturnsValidTokens()
    {
        // Arrange.
        var source = "$.*";
        SyntaxKind[] expectedKinds = [SyntaxKind.DollarMarkToken, SyntaxKind.DotToken, SyntaxKind.AsteriskToken];

        // Assert.
        AssertTokenKinds(source, expectedKinds);
    }
    [Test]
    public void GetTokens_CallWithBraketed_ReturnsValidTokens()
    {
        // Arrange.
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
        // Arrange.
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
        // Arrange.
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
        // Arrange.
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
    public void GetTokens_CallWithSingleQuotedStringLiterals_ReturnsValidTokens()
    {
        // Arrange.
        var source = "$['ab']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithDoubleQuotedStringLiteral_ReturnsValidTokens()
    {
        // Arrange.
        var source = @" ""ab"" ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.StringLiteralToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStringLiteralsWithEscapable_ReturnsValidTokens()
    {
        // Arrange.
        var source = """$['a\b\f\n\r\t\/\'']""";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStringLiteralsWithHexEscapable_ReturnsValidTokens()
    {
        // Arrange.
        var source = "$['a\\uA123']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStringLiteralsWithSingleQuoteEscaped_ReturnsValidTokens()
    {
        // Arrange.
        var source = "$['a\\'']";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken,
            SyntaxKind.OpenBracketToken,
            SyntaxKind.StringLiteralToken,
            SyntaxKind.CloseBracketToken);
    }
    [Test]
    public void GetTokens_CallWithSingleQuotedStringLiteralsWithDoubleQuote_ReturnsValidTokens()
    {
        // Arrange.
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
        // Arrange.
        var source = new byte[] { 0xEF, 0xBB, 0xBF, (byte)'$' };

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.DollarMarkToken);
    }
    [Test]
    public void GetTokens_CallWithCompOperatorTokens_ReturnsValidTokens()
    {
        // Arrange.
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
    public void GetTokens_CallWithLogicalOperatorsText_ReturnsValidTokens()
    {
        // Arrange.
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
        // Arrange.
        var source = " 123456 ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.IntegerNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithZeroText_ReturnsValidTokens()
    {
        // Arrange.
        var source = " 0 ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.IntegerNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithNegativeInteger_ReturnsValidTokens()
    {
        // Arrange.
        var source = " -7 ";

        // Assert.
        AssertTokenKinds(source,
            SyntaxKind.IntegerNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithNegativeFloat_ReturnsFlatTokens()
    {
        // Arrange.
        var source = " -0.7123E+54";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithPositiveFloat_ReturnsFlatTokens()
    {
        // Arrange.
        var source = " 0.9824e-38";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithNegativeZero_ReturnsUnknown()
    {
        // Arrange.
        var source = " -0 ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test]
    public void GetTokens_CallWithFractionNumberOnly_ReturnsParen()
    {
        // Arrange.
        var source = " -0.76876 ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithParen_ReturnsValidTokens()
    {
        // Arrange.
        var source = " ( ) ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken);
    }
    [Test]
    public void GetTokens_CallWithExponentNumberOnly_ReturnsFloatNumberLiteral()
    {
        // Arrange.
        var source = " 122E6 ";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.FloatNumberLiteral);
    }
    [Test]
    public void GetTokens_CallWithWrongSingleQuotedStringLiteral_ReturnsUnknown()
    {
        // Arrange.
        var source = """ '\' """;

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test]
    public void GetTokens_CallWithWrongDoubleQuotedStringLiteral_ReturnsUnknown()
    {
        // Arrange.
        var source = """ "\" """;

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test]
    public void GetTokens_CallWithWrongDoubleEscapedStringLiteral_ReturnsUnknown()
    {
        // Arrange.
        var source = """ "\ud""";

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown);
    }
    [Test/*, Ignore("todo")*/]
    public void GetTokens_CallWithWrongDoubleEscapedStringLiteral_ReturnsUnknown1()
    {
        // Arrange.
        var source = """ #  # """;

        // Assert.
        AssertTokenKinds(source, SyntaxKind.Unknown, SyntaxKind.Unknown);
    }
}