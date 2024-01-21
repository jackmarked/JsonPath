namespace JetDevel.JsonPath.Tests;
/// <summary>
/// https://cburgmer.github.io/json-path-comparison/results/array_slice_on_exact_match.html
/// </summary>
sealed class JsonPathComparisonTests: JsonPathQueryTestFixture
{
    [Test]
    public void ArraySlice()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[1:3]", @"[""second"", ""third""]");
    }
    [Test]
    public void ArraySliceOnExactMatch()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[0:5]", @"[""first"", ""second"", ""third"", ""forth"", ""fifth""]");
    }
    [Test]
    public void ArraySliceOnNonOverlappingArray()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, "$[7:10]", "[]");
    }
    [Test]
    public void ArraySliceOnObject()
    {
        var source = """
{":": 42, "more": "string", "a": 1, "b": 2, "c": 3, "1:3": "nice"}
""";
        AssertQueryResultIsEmpty(source, "$[1:3]");
    }
    [Test]
    public void ArraySliceOnPartiallyOverlappingArray()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, "$[1:10]", @"[""second"", ""third""]");
    }
    [Test]
    public void ArraySliceWithLargeNumberForEnd()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[2:113667776004]", @"[""third"",""forth"",""fifth""]");
    }
    [Test]
    public void ArraySliceWithLargeNumberForEndAndNegativeStep()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[2:-113667776004:-1]", @"[""third"",""second"",""first""]");
    }
    [Test]
    public void ArraySliceWithLargeNumberForStart()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[-113667776004:2]", @"[""first"",""second""]");
    }
    [Test]
    public void ArraySliceWithLargeNumberForStartEndNegativeStep()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[113667776004:2:-1]", @"[""fifth"",""forth""]");
    }
    [Test]
    public void ArraySliceWithNegativeStartAndEndAndRangeOfMinus1()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[-4:-5]", @"[]");
    }
    [Test]
    public void ArraySliceWithNegativeStartAndEndAndRangeOf0()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[-4:-4]", @"[]");
    }
    [Test]
    public void ArraySliceWithNegativeStarAndEndAndRangeOf1()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[-4:-3]", @"[4]");
    }
    [Test]
    public void ArraySliceWithNegativeStartAndPositiveEndAndRangeOfMinus1()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[-4:1]", @"[]");
    }
    [Test]
    public void ArraySliceWithNegativeStartAndPositiveEndAndRangeOf0()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[-4:2]", @"[]");
    }
    [Test]
    public void ArraySliceWithNegativeStartAndPositiveEndAndRangeOf1()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[-4:3]", @"[4]");
    }
    [Test]
    public void ArraySliceWithNegativeStep()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[3:0:-2]", @"[""forth"",""second""]");
    }
    [Test]
    public void ArraySliceWithNegativeStepOnPartiallyOverlappingArray()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[7:3:-1]", @"[""fifth""]");
    }
    [Test]
    public void ArraySliceWithNegativeStepAndStartGreaterThanEnd()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[0:3:-2]", @"[]");
    }
    [Test]
    public void ArraySliceWithNegativeStepOnly()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[::-2]", @"[""fifth"", ""third"", ""first""]");
    }
    [Test]
    public void ArraySliceWithOpenEnd()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[1:]", @"[""second"", ""third"", ""forth"", ""fifth""]");
    }
    [Test]
    public void ArraySliceWithOpenStart()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[:2]", @"[""first"", ""second""]");
    }
    [Test]
    public void ArraySliceWithOpenStartAndEnd()
    {
        var source = """
["first", "second"]
""";
        AssertQueryResult(source, "$[:]", @"[""first"", ""second""]");
    }
    [Test]
    public void ArraySliceWithOpenStartAndEndOnObject()
    {
        var source = """
{":": 42, "more": "string"}
""";
        AssertQueryResult(source, "$[:]", @"[]");
    }
    [Test]
    public void ArraySliceWithOpenStartAndEndAndStepEmpty()
    {
        var source = """
["first","second"]
""";
        AssertQueryResult(source, "$[::]", @"[""first"",""second""]");
    }
    [Test]
    public void ArraySliceWithPositiveStartAndNegativeEndAndRangeOfMinus1()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[3:-4]", @"[]");
    }
    [Test]
    public void ArraySliceWithPositiveStartAndNegativeEndAndRangeOf0()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[3:-3]", @"[]");
    }
    [Test]
    public void ArraySliceWithPositiveStartAndNegativeEndAndRangeOf1()
    {
        var source = """
[2, "a", 4, 5, 100, "nice"]
""";
        AssertQueryResult(source, "$[3:-2]", @"[5]");
    }
    [Test]
    public void ArraySliceWithRangeOfMinus1()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[2:1]", @"[]");
    }
    [Test]
    public void ArraySliceWithRangeOf0()
    {
        var source = """
["first", "second"]
""";
        AssertQueryResult(source, "$[0:0]", @"[]");
    }
    [Test]
    public void ArraySliceWithRangeOf1()
    {
        var source = """
["first", "second"]
""";
        AssertQueryResult(source, "$[0:1]", @"[""first""]");
    }
    [Test]
    public void ArraySliceWithStartMinus1AndOpenEnd()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, "$[-1:]", @"[""third""]");
    }
    [Test]
    public void ArraySliceWithStartMinus2AndOpenEnd()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, "$[-2:]", @"[""second"",""third""]");
    }
    [Test]
    public void ArraySliceWithStartLargeNegativeNumberAndOpenEndOnShortArray()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, "$[-4:]", @"[""first"", ""second"", ""third""]");
    }
    [Test]
    public void ArraySliceWithStep0()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[0:3:0]", @"[]");
    }
    [Test]
    public void ArraySliceWithStep1()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[0:3:1]", @"[""first"",""second"",""third""]");
    }
    [Test, Ignore("TODO")]
    public void ArraySliceWithStepAndLeadingZeros() // ???
    {
        var source = """
[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25]
""";
        AssertQueryResult(source, "$[010:024:010]", @"[10,20]");
    }
    [Test]
    public void ArraySliceWithStepButEndNotAligned()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[0:4:2]", @"[""first"",""third""]");
    }
    [Test]
    public void ArraySliceWithStepEmpty()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[1:3:]", @"[""second"",""third""]");
    }
    [Test]
    public void ArraySliceWithStepOnly()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[::2]", @"[""first"",""third"",""fifth""]");
    }
    [Test]
    public void BracketNotation()
    {
        var source = """
{
  "key": "value"
}
""";
        AssertQueryResult(source, "$['key']", @"[""value""]");
    }
    [Test]
    public void BracketNotationOnObjectWithoutKey()
    {
        var source = """
{
  "key": "value"
}
""";
        AssertQueryResult(source, "$['missing']", @"[]");
    }
    [Test]  // Bracket notation after recursive descent
    public void BracketNotationAfterRecursiveDescent()
    {
        var source = """
[
    "first",
    {
        "key": [
            "first nested",
            {
                "more": [
                    {
                        "nested": ["deepest", "second"]
                    },
                    ["more", "values"]
                ]
            }
        ]
    }
]
""";
        AssertQueryResult(source, "$..[0]", @"[""deepest"",""first nested"",""first"",""more"",{""nested"":[""deepest"",""second""]}]", true);
    }
    [Test]
    public void BracketNotationWithNFCPathOnNFDKey()
    {
        var source = """
{"ü": 42}
""";
        AssertQueryResult(source, "$['ü']", @"[]");
    }
    [Test]
    public void BracketNotationWithDot()
    {
        var source = """
{
    "one": {"key": "value"},
    "two": {"some": "more", "key": "other value"},
    "two.some": "42"
}
""";
        AssertQueryResult(source, "$['two.some']", @"[""42""]");
    }
    [Test]
    public void BracketNotationWithDoubleQuotes()
    {
        var source = """
{
  "key": "value"
}
""";
        AssertQueryResult(source, @"$[""key""]", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithEmptyPath()
    {
        var source = """
{"": 42, "''": 123, "\"\"": 222}
""";
        AssertInvalidQuery(source, @"$[]", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithEmptyString()
    {
        var source = """
{"": 42, "''": 123, "\"\"": 222}
""";
        AssertQueryResult(source, @"$['']", @"[42]");
    }
    [Test]
    public void BracketNotationWithEmptyStringDoubledQuoted()
    {
        var source = """
{"": 42, "''": 123, "\"\"": 222}
""";
        AssertQueryResult(source, @"$[""""]", @"[42]");
    }
    [Test]
    public void BracketNotationWithNegativeNumberOnShortArray()
    {
        var source = """
["one element"]
""";
        AssertQueryResult(source, @"$[-2]", @"[]");
    }
    [Test]
    public void BracketNotationWithNumber()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, @"$[2]", @"[""third""]");
    }
    [Test]
    public void BracketNotationWithNumberOnObject()
    {
        var source = """
{ "0": "value" }
""";
        AssertQueryResult(source, @"$[0]", @"[]");
    }
    [Test]
    public void BracketNotationWithNumberOnShortArray()
    {
        var source = """
["one element"]
""";
        AssertQueryResult(source, @"$[1]", @"[]");
    }
    [Test]
    public void BracketNotationWithNumberOnString()
    {
        var source = """
"Hello World"
""";
        AssertQueryResult(source, "$[0]", @"[]");
    }
    [Test]
    public void BracketNotationWithNumberAfterDotNotationWithWildcardOnNestedArraysWithDifferentLength()
    {
        var source = """
[[1], [2,3]]
""";
        AssertQueryResult(source, "$.*[1]", @"[3]");
    }
    [Test]
    public void BracketNotationWithNumberMinus1()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, "$[-1]", @"[""third""]");
    }
    [Test]
    public void BracketNotationWithNumberMinus1OnEmptyArray()
    {
        var source = """
[]
""";
        AssertQueryResult(source, "$[-1]", @"[]");
    }
    [Test]
    public void BracketNotationWithNumber0()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, "$[0]", @"[""first""]");
    }
    [Test]
    public void BracketNotationWithQuotedArraySliceLiteral()
    {
        var source = """
{
    ":": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, "$[':']", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedClosingBracketLiteral()
    {
        var source = """
{"]": 42}
""";
        AssertQueryResult(source, "$[']']", @"[42]");
    }
    [Test]
    public void BracketNotationWithQuotedCurrentObjectLiteral()
    {
        var source = """
{
    "@": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, "$['@']", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedDotLiteral()
    {
        var source = """
{
    ".": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, "$['.']", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedDotWildcard()
    {
        var source = """
{"key": 42, ".*": 1, "": 10}
""";
        AssertQueryResult(source, "$['.*']", @"[1]");
    }
    [Test]
    public void BracketNotationWithQuotedDoubleQuoteLiteral()
    {
        var source = """
{
    "\"": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, """$['"']""", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedSingleQuoteLiteral()
    {
        var source = """
{
    "'": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, """$['\'']""", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedEscapedBackslash()
    {
        var source = """
{"\\":"value"}
""";
        AssertQueryResult(source, """$['\\']""", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedEscapedSingleQuoted()
    {
        var source = """
{"'":"value"}
""";
        AssertQueryResult(source, """$['\'']""", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedNumberOnObject()
    {
        var source = """
{"0":"value"}
""";
        AssertQueryResult(source, """$['0']""", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedRootLiteral()
    {
        var source = """
{
    "$": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, """$['$']""", @"[""value""]");
    }
    [Test]
    public void BracketNotationWithQuotedStringAndUnescapedSingleQuote()
    {
        var source = """
{"single'quote":"value"}
""";
        AssertInvalidQuery(source, """$['single'quote']""", "[]");
    }
    [Test]
    public void BracketNotationWithQuotedUnionLiteral()
    {
        var source = """
{
    ",": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, """$[',']""", """["value"]""");
    }
    [Test]
    public void BracketNotationWithQuotedWildcardLiteral()
    {
        var source = """
{
    "*": "value",
    "another": "entry"
}
""";
        AssertQueryResult(source, """$['*']""", """["value"]""");
    }
    [Test]
    public void BracketNotationWithQuotedWildcardLiteralOnObjectWithoutKey()
    {
        var source = """
{
    "another": "entry"
}
""";
        AssertQueryResult(source, """$['*']""", """[]""");
    }
    [Test]
    public void BracketNotationWithSpaces()
    {
        var source = """
{" a": 1, "a": 2, " a ": 3, "a ": 4, " 'a' ": 5, " 'a": 6, "a' ": 7, " \"a\" ": 8, "\"a\"": 9}
""";
        AssertQueryResult(source, """$[ 'a' ]""", """[2]""");
    }
    [Test]
    public void BracketNotationWithStringIncludingDotWildcard()
    {
        var source = """
{"nice": 42, "ni.*": 1, "mice": 100}
""";
        AssertQueryResult(source, """$['ni.*']""", """[1]""");
    }
    [Test]
    public void BracketNotationWithTwoLiteralsSeparatedByDot()
    {
        var source = """
{
    "one": {"key": "value"},
    "two": {"some": "more", "key": "other value"},
    "two.some": "42",
    "two'.'some": "43"
}
""";
        AssertInvalidQuery(source, """$['two'.'some']""", """[]""");
    }
    [Test]
    public void BracketNotationWithTwoLiteralsSeparatedByDotWithoutQuotes()
    {
        var source = """
{
    "one": {"key": "value"},
    "two": {"some": "more", "key": "other value"},
    "two.some": "42"
}
""";
        AssertInvalidQuery(source, """$[two.some]""", """[]""");
    }
    [Test]
    public void BracketNotationWithWildcardOnArray()
    {
        var source = """
[
    "string",
    42,
    {
        "key": "value"
    },
    [0, 1]
]
""";
        AssertQueryResult(source, """$[*]""", """["string",42,{"key": "value"},[0, 1]]""");
    }
    [Test]
    public void BracketNotationWithWildcardOnEmptyArray()
    {
        var source = """
[
]
""";
        AssertQueryResult(source, """$[*]""", """[]""");
    }
    [Test]
    public void BracketNotationWithWildcardOnEmptyObject()
    {
        var source = """
{}
""";
        AssertQueryResult(source, """$[*]""", """[]""");
    }
    [Test]
    public void BracketNotationWithWildcardOnNullValueArray()
    {
        var source = """
[
    40,
    null,
    42
]
""";
        AssertQueryResult(source, """$[*]""", """[40,null,42]""");
    }
    [Test]
    public void BracketNotationWithWildcardOnObject()
    {
        var source = """
{
    "some": "string",
    "int": 42,
    "object": {
        "key": "value"
    },
    "array": [0, 1]
}
""";
        AssertQueryResult(source, """$[*]""", """["string",42,[0,1],{"key": "value"}]""", true);
    }
    [Test]
    public void BracketNotationWithWildcardAfterDotNotationAfterBracketNotationWithWildcard()
    {
        var source = """
[{"bar": [42]}]
""";
        AssertQueryResult(source, """$[*].bar[*]""", """[42]""");
    }
    [Test]
    public void BracketNotationWithWildcardAfterRecursiveDescent()
    {
        var source = """
{
    "key": "value",
    "another key": {
        "complex": "string",
        "primitives": [0, 1]
    }
}
""";
        AssertQueryResult(source, """$..[*]""", """
            [
              "string",
              "value",
              0,
              1,
              [0,1],
              {
                "complex": "string",
                "primitives": [0,1]
              }
            ]
            """, true);
    }
    [Test]
    public void BracketNotationWithoutQuotes()
    {
        var source = """
{ "key": "value" }
""";
        AssertInvalidQuery(source, """$[key]""", """[]""");
    }
    [Test]
    public void CurrentWithDotNotation()
    {
        var source = """
{"a": 1}
""";
        AssertInvalidQuery(source, """@.a""", """[]""");
    }
    [Test]
    public void DotBracketNotation()
    {
        var source = """
{
  "key": "value",
  "other": {"key": [{"key": 42}]}
}
""";
        AssertInvalidQuery(source, """$.['key']""", """["value"]""");
    }
    [Test]
    public void DotBracketNotationWithDoubleQuotes()
    {
        var source = """
{
  "key": "value",
  "other": {"key": [{"key": 42}]}
}
""";
        AssertInvalidQuery(source, """$.["key"]""", """["value"]""");
    }
    [Test]
    public void DotBracketNotationWithoutQuotes()
    {
        var source = """
{
  "key": "value",
  "other": {"key": [{"key": 42}]}
}
""";
        AssertInvalidQuery(source, """$.[key]""", """["value"]""");
    }
    [Test]
    public void DotBracketNotationWithoutQuotes1()
    {
        var source = """
{
  "key": "value",
  "other": {"key": [{"key": 42}]}
}
""";
        AssertInvalidQuery(source, """$.["key"]""", """["value"]""");
    }
    [Test]
    public void DotNotation()
    {
        var source = """
{
  "key": "value"
}
""";
        AssertQueryResult(source, """$.key""", """["value"]""");
    }
    [Test]
    public void DotNotationOnArray()
    {
        var source = """
[0, 1]
""";
        AssertQueryResultIsEmpty(source, """$.key""");
    }
    [Test]
    public void DotNotationOnArrayValue()
    {
        var source = """
{
    "key": ["first", "second"]
}
""";
        AssertQueryResult(source, """$.key""", """[["first","second"]]""");
    }
    [Test]
    public void DotNotationOnArrayWithContainingObjectMatchingKey()
    {
        var source = """
[{"id": 2}]
""";
        AssertQueryResult(source, """$.id""", """[]""");
    }
    [Test]
    public void DotNotationOnEmptyObjectValue()
    {
        var source = """
{
  "key": {}
}
""";
        AssertQueryResult(source, """$.key""", """[{}]""");
    }
    [Test]
    public void DotNotationOnNullValue()
    {
        var source = """
{
  "key": null
}
""";
        AssertQueryResult(source, """$.key""", """[null]""");
    }
    [Test]
    public void DotNotationOnObjectWithoutKey()
    {
        var source = """
{"key": "value"}
""";
        AssertQueryResult(source, """$.missing""", """[]""");
    }
    [Test]
    public void DotNotationAfterArraySlice()
    {
        var source = """
[{"key": "ey"}, {"key": "bee"}, {"key": "see"}]
""";
        AssertQueryResult(source, """$[0:2].key""", """["ey","bee"]""");
    }
    [Test]
    public void DotNotationaAfterBracketNotationAfterRecursiveDescent()
    {
        var source = """
{
  "k": [{"key": "some value"}, {"key": 42}],
  "kk": [[{"key": 100}, {"key": 200}, {"key": 300}], [{"key": 400}, {"key": 500}, {"key": 600}]],
  "key": [0, 1]
}
""";
        AssertQueryResult(source, """$..[1].key""", """[200,42,500]""", true);
    }
    [Test]
    public void DotNotationaAfterBracketNotationWithWildcard()
    {
        var source = """
[{"a": 1},{"a": 1}]
""";
        AssertQueryResult(source, """$[*].a""", """[1,1]""");
    }
    [Test]
    public void DotNotationaAfterBracketNotationWithWildcardOnOneMatching()
    {
        var source = """
[{"a": 1}]
""";
        AssertQueryResult(source, """$[*].a""", """[1]""");
    }
    [Test]
    public void DotNotationaAfterBracketNotationWithWildcardOnSomeMatching()
    {
        var source = """
[{"a": 1},{"b": 1}]
""";
        AssertQueryResult(source, """$[*].a""", """[1]""");
    }
    [Test]
    public void DotNotationaAfterFilterExpression()
    {
        var source = """
[{"id": 42, "name": "forty-two"}, {"id": 1, "name": "one"}]
""";
        AssertQueryResult(source, """$[?(@.id==42)].name""", """["forty-two"]""");
    }
    [Test]
    public void DotNotationaAfterRecursiveDescent()
    {
        var source = """
{
    "object": {
        "key": "value",
        "array": [
            {"key": "something"},
            {"key": {"key": "russian dolls"}}
        ]
    },
    "key": "top"
}
""";
        AssertQueryResult(source, """$..key""", """["russian dolls","something","top","value",{"key": "russian dolls"}]""", true);
    }
    [Test]
    public void DotNotationaAfterRecursiveDescentAfterDotNotation()
    {
        var source = """
{
  "store": {
    "book": [
      {
        "category": "reference",
        "author": "Nigel Rees",
        "title": "Sayings of the Century",
        "price": 8.95
      },
      {
        "category": "fiction",
        "author": "Evelyn Waugh",
        "title": "Sword of Honour",
        "price": 12.99
      },
      {
        "category": "fiction",
        "author": "Herman Melville",
        "title": "Moby Dick",
        "isbn": "0-553-21311-3",
        "price": 8.99
      },
      {
        "category": "fiction",
        "author": "J. R. R. Tolkien",
        "title": "The Lord of the Rings",
        "isbn": "0-395-19395-8",
        "price": 22.99
      }
    ],
    "bicycle": {
      "color": "red",
      "price": 19.95
    }
  }
}
""";
        AssertQueryResult(source, """$.store..price""", """[12.99,19.95,22.99,8.95,8.99]""", true);
    }
    [Test]
    public void DotNotationaAfterRecursiveDescentWithExtraDot()
    {
        var source = """
{
    "object": {
        "key": "value",
        "array": [
            {"key": "something"},
            {"key": {"key": "russian dolls"}}
        ]
    },
    "key": "top"
}
""";
        AssertInvalidQuery(source, """$...key""", """[]""");
    }
    [Test]
    public void DotNotationaAfterUnion()
    {
        var source = """
[{"key": "ey"}, {"key": "bee"}, {"key": "see"}]
""";
        AssertQueryResult(source, """$[0,2].key""", """["ey","see"]""");
    }
    [Test]
    public void DotNotationaAfterUnionWithKeys()
    {
        var source = """
{
    "one": {"key": "value"},
    "two": {"k": "v"},
    "three": {"some": "more", "key": "other value"}
}
""";
        AssertQueryResult(source, """$['one','three'].key""", """["value","other value"]""");
    }
    [Test, Ignore("TODO")]
    public void DotNotationaWithDash()
    {
        var source = """
{
  "key": 42,
  "key-": 43,
  "-": 44,
  "dash": 45,
  "-dash": 46,
  "": 47,
  "key-dash": "value",
  "something": "else"
}
""";
        AssertQueryResult(source, """$.key-dash""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithDoubleQuotes()
    {
        var source = """
{
  "key": "value",
  "\"key\"": 42
}
""";
        AssertInvalidQuery(source, """$."key" """, """[]""");
    }
    [Test]
    public void DotNotationaWithDoubleQuotesAfterRecursiveDescent()
    {
        var source = """
{
  "object": {
    "key": "value",
    "\"key\"": 100,
    "array": [
      {"key": "something", "\"key\"": 0},
      {"key": {"key": "russian dolls"}, "\"key\"": {"\"key\"": 99}}
    ]
  },
  "key": "top",
  "\"key\"": 42
}
""";
        AssertInvalidQuery(source, """$.."key" """, """[]""");
    }
    [Test]
    public void DotNotationaWithEmptyPath()
    {
        var source = """
{"key": 42, "": 9001, "''": "nice"}
""";
        AssertInvalidQuery(source, """$.""", """[]""");
    }
    [Test]
    public void DotNotationaWithKeyNamedIn()
    {
        var source = """
{ "in": "value" }
""";
        AssertQueryResult(source, """$.in""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithKeyNamedLength()
    {
        var source = """
{ "length": "value" }
""";
        AssertQueryResult(source, """$.length""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithKeyNamedLengthOnArray()
    {
        var source = """
[4, 5, 6]
""";
        AssertQueryResult(source, """$.length""", """[]""");
    }
    [Test]
    public void DotNotationaWithKeyNamedNull()
    {
        var source = """
{
  "null": "value"
}
""";
        AssertQueryResult(source, """$.null""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithKeyNamedTrue()
    {
        var source = """
{
  "true": "value"
}
""";
        AssertQueryResult(source, """$.true""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithKeyRootLiteral()
    {
        var source = """
{
  "$": "value"
}
""";
        AssertInvalidQuery(source, """$.$""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithNonAsciiKey()
    {
        var source = """
{
  "屬性": "value"
}
""";
        AssertQueryResult(source, """$.屬性""", """["value"]""");
    }
    [Test]
    public void DotNotationaWithNumber()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertInvalidQuery(source, """$.2""", """["value"]""");
    }
    [Test, Ignore("TODO")]
    public void DotNotationaWithNumberOnObject()
    {
        var source = """
{"a": "first", "2": "second", "b": "third"}
""";
        AssertQueryResult(source, """$.2""", """["second"]""");
    }
    [Test]
    public void DotNotationaWithNumberMinus1()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertInvalidQuery(source, """$.-1""", """[]""");
    }
    [Test]
    public void DotNotationaWithSingleQuotes()
    {
        var source = """
{
  "key": "value",
  "'key'": 42
}
""";
        AssertInvalidQuery(source, """$.'key'""", """[]""");
    }
    [Test]
    public void DotNotationaWithSingleQuotesAfterRecursiveDescent()
    {
        var source = """
{
  "object": {
    "key": "value",
    "'key'": 100,
    "array": [
      {"key": "something", "'key'": 0},
      {"key": {"key": "russian dolls"}, "'key'": {"'key'": 99}}
    ]
  },
  "key": "top",
  "'key'": 42
}
""";
        AssertInvalidQuery(source, """$.'key'""", """[]""");
    }
    [Test]
    public void DotNotationaWithSingleQuotesAndDot()
    {
        var source = """
{"some.key": 42, "some": {"key": "value"}, "'some.key'": 43}
""";
        AssertInvalidQuery(source, """$.'some.key'""", """[]""");
    }
    [Test]
    public void DotNotationaWithSpacePaddedKey()
    {
        var source = """
{" a": 1, "a": 2, " a ": 3, "": 4}
""";
        AssertQueryResult(source, """$. a""", """[2]""");
    }
    [Test]
    public void DotNotationaWithWildcardOnArray()
    {
        var source = """
[
    "string",
    42,
    {
        "key": "value"
    },
    [0, 1]
]
""";
        AssertQueryResult(source, """$.*""", """["string",42,{"key": "value"},[0,1]]""");
    }
    [Test]
    public void DotNotationaWithWildcardOnEmptyArray()
    {
        var source = """
[]
""";
        AssertQueryResult(source, """$.*""", """[]""");
    }
    [Test]
    public void DotNotationaWithWildcardOnEmptyObject()
    {
        var source = """
{}
""";
        AssertQueryResult(source, """$.*""", """[]""");
    }
    [Test]
    public void DotNotationaWithWildcardOnObject()
    {
        var source = """
{
    "some": "string",
    "int": 42,
    "object": {
        "key": "value"
    },
    "array": [0, 1]
}
""";
        AssertQueryResult(source, """$.*""", """["string",42,[0,1],{"key": "value"}]""", true);
    }
    [Test]
    public void DotNotationaWithWildcardAfterDotNotationAfterDotNotationWithWildcard()
    {
        var source = """
[{"bar": [42]}]
""";
        AssertQueryResult(source, """$.*.bar.*""", """[42]""");
    }
    [Test]
    public void DotNotationWithWildcardAfterDotNotationWithWildcardOnNestedArrays()
    {
        var source = """
[[1, 2, 3], [4, 5, 6]]
""";
        AssertQueryResult(source, """$.*.*""", """[1,2,3,4,5,6]""");
    }
    [Test]
    public void DotNotationWithWildcardAfterRecursiveDescent()
    {
        var source = """
{
    "key": "value",
    "another key": {
        "complex": "string",
        "primitives": [0, 1]
    }
}
""";
        AssertQueryResult(source, """$..*""", """["string","value",0,1,[0,1],{"complex":"string","primitives": [0,1]}]""", true);
    }
    [Test]
    public void DotNotationWithWildcardAfterRecursiveDescentOnNullValueArray()
    {
        var source = """
[
    40,
    null,
    42
]
""";
        AssertQueryResult(source, """$..*""", """[40,42,null]""", true);
    }
    [Test]
    public void DotNotationWithWildcardAfterRecursiveDescentOnScalar()
    {
        var source = """42""";
        AssertQueryResult(source, """$..*""", """[]""");
    }
    [Test]
    public void DotNotationWithoutDot()
    {
        var source = """
{"a": 1, "$a": 2}
""";
        AssertInvalidQuery(source, """$a""", """[]""");
    }
    [Test]
    public void DotNotationWithoutRoot()
    {
        var source = """
{
  "key": "value"
}
""";
        AssertInvalidQuery(source, """.key""", """[]""");
    }
    [Test]
    public void DotNotationWithoutRootAndDot()
    {
        var source = """
{
  "key": "value"
}
""";
        AssertInvalidQuery(source, """key""", """[]""");
    }
    [Test]
    public void Empty()
    {
        var source = """
{"a": 42, "": 21}
""";
        AssertInvalidQuery(source, string.Empty, """[]""");
    }
    [Test]
    public void FilterExpressionOnObject()
    {
        var source = """
{"key": 42, "another": {"key": 1}}
""";
        AssertQueryResult(source, """$[?(@.key)]""", """[{"key": 1}]""");
    }
    [Test] //TODO
    public void FilterExpressionAfterDotNotationWithWildcardAfterRecursiveDescent()
    {
        var source = """
[
    {
        "complext": {
            "one": [
                {"name": "first",  "id": 1},
                {"name": "next",   "id": 2},
                {"name": "another","id": 3},
                {"name": "more",   "id": 4}
            ],
            "more": {"name": "next to last","id": 5}
        }
    },
    {"name": "last","id": 6}
]
""";
        AssertQueryResult(source, """$..*[?(@.id>2)]""",
"""
[{"id": 3,"name": "another"},{"id": 4,"name": "more"},{"id": 5,"name": "next to last"}]
""", true);
    }
    [Test]
    public void FilterExpressionAfterRecursiveDescent()
    {
        var source = """
{
  "id": 2,
  "more": [
    { "id": 2, "name": "a" },
    {"more": {"id": 2, "name": "b"}},
    {"id": {"id": 2, "name": "c"}},
    [{"id": 2, "name": "d"}]
  ]
}
""";
        AssertQueryResult(source, """$..[?(@.id==2)]""",
            """[{ "id": 2, "name": "a" },{ "id": 2, "name": "b" },{ "id": 2, "name": "c" },{ "id": 2, "name": "d" }]""", true);
    }
    [Test]
    public void FilterExpressionWithAddition()
    {
        var source = """
[{"key": 60}, {"key": 50}, {"key": 10}, {"key": -50}, {"key+50": 100}]
""";
        AssertInvalidQuery(source, """$[?(@.key+50==100)]""",
            """[{ "id": 2, "name": "a" },{ "id": 2, "name": "b" },{ "id": 2, "name": "c" },{ "id": 2, "name": "d" }]""");
    }
    [Test]
    public void FilterExpressionWithBooleanAndOperator()
    {
        var source = """
[{"key": 42},{"key": 43},{"key": 44}]
""";
        AssertQueryResult(source, """$[?(@.key>42 && @.key<44)]""", """[{"key": 43}]""");
    }
    [Test]
    public void FilterExpressionWithBooleanAndOperatorAndValueFalse()
    {
        var source = """
[
  {"key": 1},
  {"key": 3},
  {"key": "nice"},
  {"key": true},
  {"key": null},
  {"key": false},
  {"key": {}},
  {"key": []},
  {"key": -1},
  {"key": 0},
  {"key": ""}
]
""";
        AssertQueryResult(source, """$[?(@.key>0 && false)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithBooleanAndOperatorAndValueTrue()
    {
        var source = """
[
  {"key": 1},
  {"key": 3},
  {"key": "nice"},
  {"key": true},
  {"key": null},
  {"key": false},
  {"key": {}},
  {"key": []},
  {"key": -1},
  {"key": 0},
  {"key": ""}
]
""";
        AssertQueryResult(source, """$[?(@.key>0 && true)]""", """[{"key": 1},{"key": 3}]""");
    }
    [Test]
    public void FilterExpressionWithBooleanOrOperator()
    {
        var source = """
[
    {"key": 42},
    {"key": 43},
    {"key": 44}
]
""";
        AssertQueryResult(source, """$[?(@.key>43 || @.key<43)]""", """[{"key": 42},{"key": 44}]""");
    }
    [Test]
    public void FilterExpressionWithBooleanOrOperatorAndValueFalse()
    {
        var source = """
[
  {"key": 1},
  {"key": 3},
  {"key": "nice"},
  {"key": true},
  {"key": null},
  {"key": false},
  {"key": {}},
  {"key": []},
  {"key": -1},
  {"key": 0},
  {"key": ""}
]
""";
        AssertQueryResult(source, """$[?(@.key>0 || false)]""", """[{"key": 1},{"key": 3}]""");
    }
    [Test]
    public void FilterExpressionWithBracketNotation()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@['key']==42)]""", """[{"key": 42}]""");
    }
    [Test]
    public void FilterExpressionWithBracketNotationWithMinus1()
    {
        var source = """
[[2, 3], ["a"], [0, 2], [2]]
""";
        AssertQueryResult(source, """$[?(@[-1]==2)]""", """[[0, 2], [2]]""", true);
    }
    [Test]
    public void FilterExpressionWithBracketNotationWithNumber()
    {
        var source = """
[["a", "b"], ["x", "y"]]
""";
        AssertQueryResult(source, """$[?(@[1]=='b')]""", """[["a", "b"]]""");
    }
    [Test]
    public void FilterExpressionWithBracketNotationWithNumberOnObject()
    {
        var source = """
{"1": ["a", "b"], "2": ["x", "y"]}
""";
        AssertQueryResult(source, """$[?(@[1]=='b')]""", """[["a", "b"]]""");
    }
    [Test]
    public void FilterExpressionWithCurrentObject()
    {
        var source = """
["some value",null,"value",0,1,-1,"",[],{},false,true]
""";
        AssertQueryResult(source, """$[?(@)]""", source);
    }
    [Test]
    public void FilterExpressionWithDifferentGroupedOperators()
    {
        var source = """
[{"a": true},{"a": true,"b": true},{"a": true,"b": true,"c": true},{"b": true,"c": true},{"a": true,"c": true},{"c": true},{"b": true}]
""";
        AssertQueryResult(source, """$[?(@.a && (@.b || @.c))]""",
            """[{"a": true,"b": true},{"a": true,"b": true,"c": true},{"a": true,"c": true}]""", true);
    }
    [Test]
    public void FilterExpressionWithDifferentUngroupedOperators()
    {
        var source = """
[{"a": true},{"a": true,"b": true},{"a": true,"b": true,"c": true},{"b": true,"c": true},{"a": true,"c": true},{"c": true},{"b": true}]
""";
        AssertQueryResult(source, """$[?(@.a && @.b || @.c)]""",
            """[{"a":true,"b":true},{"a":true,"b":true,"c":true},{"b":true,"c":true},{"a":true,"c":true},{"c":true}]""", true);
    }
    [Test]
    public void FilterExpressionWithDivision()
    {
        var source = """
[{"a": true},{"a": true,"b": true},{"a": true,"b": true,"c": true},{"b": true,"c": true},{"a": true,"c": true},{"c": true},{"b": true}]
""";
        AssertInvalidQuery(source, """$[?(@.key/10==5)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithDotNotationWithDash()
    {
        var source = """
[
   {
     "key-dash": "value"
   }
]
""";
        AssertInvalidQuery(source, """$[?(@.key-dash == 'value')]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithDotNotationWithNumber()
    {
        var source = """
[{"a": "first", "2": "second", "b": "third"}]   
""";
        AssertInvalidQuery(source, """$[?(@.2 == 'second')]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithDotNotationWithNumberOnArray()
    {
        var source = """
[["first", "second", "third", "forth", "fifth"]]  
""";
        AssertInvalidQuery(source, """$[?(@.2 == 'third')]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEquals()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "some"},
    {"key": "42"},
    {"key": null},
    {"key": 420},
    {"key": ""},
    {"key": {}},
    {"key": []},
    {"key": [42]},
    {"key": {"key": 42}},
    {"key": {"some": 42}},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key==42)]""", """[{"key": 42}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsOnArrayOfNumbers()
    {
        var source = """
[
    0,
    42,
    -1,
    41,
    43,
    42.0001,
    41.9999,
    null,
    100
]
""";
        AssertQueryResult(source, """$[?(@==42)]""", """[42]""");
    }
    [Test]
    public void FilterExpressionWithEqualsOnArrayWithoutMatch()
    {
        var source = """
[
[{"key": 42}]
]
""";
        AssertQueryResult(source, """$[?(@.key==43)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsOnObject()
    {
        var source = """
{
    "a": {"key": 0},
    "b": {"key": 42},
    "c": {"key": -1},
    "d": {"key": 41},
    "e": {"key": 43},
    "f": {"key": 42.0001},
    "g": {"key": 41.9999},
    "h": {"key": 100},
    "i": {"some": "value"}
}
""";
        AssertQueryResult(source, """$[?(@.key==42)]""", """[{"key": 42}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsOnObjectWithKeyMatchingQuery()
    {
        var source = """
{"id": 2}
""";
        AssertQueryResult(source, """$[?(@.id==2)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsArray()
    {
        var source = """
[
  {"d": ["v1","v2"]},
  {"d": ["a","b"]},
  {"d": "v1"},
  {"d": "v2"},
  {"d": {}},
  {"d": []},
  {"d": null},
  {"d": -1},
  {"d": 0},
  {"d": 1},
  {"d": "['v1','v2']"},
  {"d": "['v1', 'v2']"},
  {"d": "v1,v2"},
  {"d": "[\"v1\", \"v2\"]"},
  {"d": "[\"v1\",\"v2\"]"}
]
""";
        AssertInvalidQuery(source, """$[?(@.d==["v1","v2"])]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsArrayForArraySliceWithRange1()
    {
        var source = """
[[1, 2, 3], [1], [2, 3], 1, 2]
""";
        AssertInvalidQuery(source, """$[?(@[0:1]==[1])]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsArrayForDotNotationWithStar()
    {
        var source = """
[[1,2], [2,3], [1], [2], [1, 2, 3], 1, 2, 3]
""";
        AssertInvalidQuery(source, """$[?(@.*==[1,2])]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsArrayOrEqualsTrue()
    {
        var source = """
[
  {"d": ["v1", "v2"] },
  {"d": ["a", "b"] },
  {"d" : true}
]
""";
        AssertInvalidQuery(source, """$[?(@.d==["v1","v2"] || (@.d == true))]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsArrayWithSingleQuotes()
    {
        var source = """
[
  {"d": ["v1","v2"]},
  {"d": ["a","b"]},
  {"d": "v1"},
  {"d": "v2"},
  {"d": {}},
  {"d": []},
  {"d": null},
  {"d": -1},
  {"d": 0},
  {"d": 1},
  {"d": "['v1','v2']"},
  {"d": "['v1', 'v2']"},
  {"d": "v1,v2"},
  {"d": "[\"v1\", \"v2\"]"},
  {"d": "[\"v1\",\"v2\"]"}
]
""";
        AssertInvalidQuery(source, """$[?(@.d==['v1','v2'])]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsBooleanExpressionValue()
    {
        var source = """
[{"key": 42}, {"key": 43}, {"key": 44}]
""";
        AssertQueryResult(source, """$[?((@.key<44)==false)]""", """[{"key": 44}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsFalse()
    {
        var source = """
[
  { "some": "some value" },
  { "key": true },
  { "key": false },
  { "key": null },
  { "key": "value" },
  { "key": "" },
  { "key": 0 },
  { "key": 1 },
  { "key": -1 },
  { "key": 42 },
  { "key": {} },
  { "key": [] }
]
""";
        AssertQueryResult(source, """$[?(@.key==false)]""", """[{ "key": false }]""");
    }
    [Test]
    public void FilterExpressionWithEqualsNull()
    {
        var source = """
[
  { "some": "some value" },
  { "key": true },
  { "key": false },
  { "key": null },
  { "key": "value" },
  { "key": "" },
  { "key": 0 },
  { "key": 1 },
  { "key": -1 },
  { "key": 42 },
  { "key": {} },
  { "key": [] }
]
""";
        AssertQueryResult(source, """$[?(@.key==null)]""", """[{ "key": null }]""");
    }
    [Test, Ignore("TODO: Detect not singular selectors")]
    public void FilterExpressionWithEqualsNumberForArraySliceWithRange1()
    {
        var source = """
[[1, 2, 3], [1], [2, 3], 1, 2]
""";
        AssertQueryResult(source, """$[?(@[0:1]==1)]""", """[]""");
    }
    [Test, Ignore("TODO: Detect not singular selectors")]
    public void FilterExpressionWithEqualsNumberForBracketNotationWithStar()
    {
        var source = """
[[1,2], [2,3], [1], [2], [1, 2, 3], 1, 2, 3]
""";
        AssertQueryResult(source, """$[?(@[*]==2)]""", """[]""");
    }
    [Test, Ignore("TODO: Detect not singular selectors")]
    public void FilterExpressionWithEqualsNumberForDotNotationWithStar()
    {
        var source = """
[[1,2], [2,3], [1], [2], [1, 2, 3], 1, 2, 3]
""";
        AssertQueryResult(source, """$[?(@.*==2)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsNumberWithFraction()
    {
        var source = """
[{"key": -12.3}, {"key": -0.123}, {"key": -12}, {"key": 12.3}, {"key": 2}, {"key": "-0.123e2"}]
""";
        AssertQueryResult(source, """$[?(@.key==-0.123e2)]""", """[{"key": -12.3}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsNumberWithLeadingZeros()
    {
        var source = """
[{"key": "010"}, {"key": "10"}, {"key": 10}, {"key": 0}, {"key": 8}]
""";
        AssertInvalidQuery(source, """$[?(@.key==010)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsObject()
    {
        var source = """
[ {"d": {"k": "v"}},
  {"d": {"a": "b"}},
  {"d": "k"},
  {"d": "v"},
  {"d": {}},
  {"d": []},
  {"d": null},
  {"d": -1},
  {"d": 0},
  {"d": 1},
  {"d": "[object Object]"},
  {"d": "{\"k\": \"v\"}"},
  {"d": "{\"k\":\"v\"}"},
  "v"
]
""";
        AssertInvalidQuery(source, """$[?(@.d=={"k":"v"})]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithEqualsString()
    {
        var source = """
[
    {"key": "some"},
    {"key": "value"},
    {"key": null},
    {"key": 0},
    {"key": 1},
    {"key": -1},
    {"key": ""},
    {"key": {}},
    {"key": []},
    {"key": "valuemore"},
    {"key": "morevalue"},
    {"key": ["value"]},
    {"key": {"some": "value"}},
    {"key": {"key": "value"}},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key=="value")]""", """[{"key": "value"}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsStringInNfc()
    {
        var source = """
[
  {"key": "something"},
  {"key": "Mot\u00f6rhead"},
  {"key": "mot\u00f6rhead"},
  {"key": "Motorhead"},
  {"key": "Motoo\u0308rhead"},
  {"key": "motoo\u0308rhead"}
]
""";
        AssertQueryResult(source, """$[?(@.key=="Motörhead")]""", """[{"key": "Mot\u00f6rhead"}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsStringWithCurrentObjectLiteral()
    {
        var source = """
[
    {"key": "some"},
    {"key": "value"},
    {"key": "hi@example.com"}
]
""";
        AssertQueryResult(source, """$[?(@.key=="hi@example.com")]""", """[{"key": "hi@example.com"}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsStringWithDotLiteral()
    {
        var source = """
[
    {"key": "some"},
    {"key": "value"},
    {"key": "some.value"}
]
""";
        AssertQueryResult(source, """$[?(@.key=="some.value")]""", """[{"key": "some.value"}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsStringWithSingleQuotes()
    {
        var source = """
[
    {"key": "some"},
    {"key": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key=='value')]""", """[{"key": "value"}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsStringWithUnicodeCharacterEscape()
    {
        var source = """
[
  {"key": "something"},
  {"key": "Mot\u00f6rhead"},
  {"key": "mot\u00f6rhead"},
  {"key": "Motorhead"},
  {"key": "Motoo\u0308rhead"},
  {"key": "motoo\u0308rhead"}
]
""";
        AssertQueryResult(source, """$[?(@.key=="Mot\u00f6rhead")]""", """[{"key": "Mot\u00f6rhead"}]""");
    }
    [Test, Ignore("TODO: detect singular query.")]
    public void FilterExpressionWithEqualsTrue()
    {
        var source = """
[
  {"some": "some value"},
  {"key": true},
  {"key": false},
  {"key": null},
  {"key": "value"},
  {"key": ""},
  {"key": 0},
  {"key": 1},
  {"key": -1},
  {"key": 42},
  {"key": {}},
  {"key": []}
]
""";
        AssertQueryResult(source, """$[?(@.key==true)]""", """[{"key": true}]""");
    }
    [Test, Ignore("TODO: detect singular query.")]
    public void FilterExpressionWithEqualsWithPathAndPath()
    {
        var source = """
[
  {"key1": 10, "key2": 10},
  {"key1": 42, "key2": 50},
  {"key1": 10},
  {"key2": 10},
  {},
  {"key1": null, "key2": null},
  {"key1": null},
  {"key2": null},
  {"key1": 0, "key2": 0},
  {"key1": 0},
  {"key2": 0},
  {"key1": -1, "key2": -1},
  {"key1": "", "key2": ""},
  {"key1": false, "key2": false},
  {"key1": false},
  {"key2": false},
  {"key1": true, "key2": true},
  {"key1": [], "key2": []},
  {"key1": {}, "key2": {}},
  {"key1": {"a": 1, "b": 2}, "key2": {"b": 2, "a": 1}}
]
""";
        AssertQueryResult(source, """$[?(@.key1==@.key2)]""", """[{"key": true}]""");
    }
    [Test]
    public void FilterExpressionWithEqualsWithRootReference()
    {
        var source = """
{"value": 42, "items": [{"key": 10}, {"key": 42}, {"key": 50}]}
""";
        AssertQueryResult(source, """$.items[?(@.key==$.value)]""", """[{"key": 42}]""");
    }
    [Test]
    public void FilterExpressionWithGreaterThan()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "value"},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key>42)]""", """[{"key": 43},{"key": 42.0001},{"key": 100}]""");
    }
    [Test]
    public void FilterExpressionWithGreaterThanOrEqual()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "value"},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key>=42)]""", """[{"key": 42},{"key": 43},{"key": 42.0001},{"key": 100}]""");
    }
    [Test]
    public void FilterExpressionWithGreaterThanString()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "alpha"},
    {"key": "ALPHA"},
    {"key": "value"},
    {"key": "VALUE"},
    {"some": "value"},
    {"some": "VALUE"}
]
""";
        AssertQueryResult(source, """$[?(@.key>"VALUE")]""", """[{"key": "alpha"},{"key": "value"}]""");
    }
    [Test]
    public void FilterExpressionWithInArrayOfValues()
    {
        var source = """
[{"d": 1}, {"d": 2}, {"d": 1}, {"d": 3}, {"d": 4}]
""";
        AssertInvalidQuery(source, """$[?(@.d in [2, 3])]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithInCurrentObject()
    {
        var source = """
[{"d": [1, 2, 3]}, {"d": [2]}, {"d": [1]}, {"d": [3, 4]}, {"d": [4, 2]}]
""";
        AssertInvalidQuery(source, """$[?(2 in @.d)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithLengthFreeFunction()
    {
        var source = """
[
  [1,2,3,4,5],
  [1,2,3,4],
  [1,2,3]
]
""";
        AssertQueryResult(source, """$[?(length(@) == 4)]""", """[[1,2,3,4]]""");
    }
    [Test]
    public void FilterExpressionWithLengthFunction()
    {
        var source = """
[
  [1,2,3,4,5],
  [1,2,3,4],
  [1,2,3]
]
""";
        AssertInvalidQuery(source, """$[?(@.length() == 4)]""", """[[1,2,3,4]]""");
    }
    [Test]
    public void FilterExpressionWithLengthPoperty()
    {
        var source = """
[
  [1,2,3,4,5],
  [1,2,3,4],
  [1,2,3]
]
""";
        AssertQueryResult(source, """$[?(@.length == 4)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithLessThan()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "value"},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key<42)]""", """[{"key": 0},{"key": -1},{"key": 41},{"key": 41.9999}]""");
    }
    [Test]
    public void FilterExpressionWithLessThanOrEqual()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "value"},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key<=42)]""", """[{"key": 0},{"key": 42},{"key": -1},{"key": 41},{"key": 41.9999}]""");
    }
    [Test]
    public void FilterExpressionWithLocalDotKeyAndNullInData()
    {
        var source = """
[
    {"key": 0},
    {"key": "value"},
    null,
    {"key": 42},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key=='value')]""", """[{"key": "value"}]""");
    }
    [Test]
    public void FilterExpressionWithMultiplication()
    {
        var source = """
[{"key": 60}, {"key": 50}, {"key": 10}, {"key": -50}, {"key*2": 100}]
""";
        AssertInvalidQuery(source, """$[?(@.key*2==100)]""", """[{"key": "value"}]""");
    }
    [Test, Ignore("TODO")]
    public void FilterExpressionWithNegationAndEquals()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "value"},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(!(@.key==42))]""", """
            [
                {"key": 0},
                {"key": -1},
                {"key": 41},
                {"key": 43},
                {"key": 42.0001},
                {"key": 41.9999},
                {"key": 100},
                {"key": "43"},
                {"key": "42"},
                {"key": "41"},
                {"key": "value"},
                {"some": "value"}
            ]
            """);
    }
    [Test]
    public void FilterExpressionWithNegationAndEqualsArrayOrEqualsTrue()
    {
        var source = """
[
  {"d": ["v1", "v2"] },
  {"d": ["a", "b"] },
  {"d" : true}
]
""";
        AssertInvalidQuery(source, """$[?(!(@.d==["v1","v2"]) || (@.d == true))]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithNegationAndLessThan()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "43"},
    {"key": "42"},
    {"key": "41"},
    {"key": "value"},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(!(@.key<42))]""", """[{"key": 42},{"key": 43},{"key": 42.0001}, {"key": 100}]""");
    }
    [Test]
    public void FilterExpressionWithNegationAndWithoutValue()
    {
        var source = """
[
  {"some": "some value"},
  {"key": true},
  {"key": false},
  {"key": null},
  {"key": "value"},
  {"key": ""},
  {"key": 0},
  {"key": 1},
  {"key": -1},
  {"key": 42},
  {"key": {}},
  {"key": []}
]
""";
        AssertQueryResult(source, """$[?(!@.key)]""", """[{"some": "some value"}]""");
    }
    [Test]
    public void FilterExpressionWithNonSingularExistenceTest()
    {
        var source = """
[
    {"a": 0},
    {"a": "x"},
    {"a": false},
    {"a": true},
    {"a": null},
    {"a": []},
    {"a": [1]},
    {"a": [1, 2]},
    {"a": {}},
    {"a": {"x": "y"}},
    {"a": {"x": "y", "w": "z"}}
]
""";
        AssertQueryResult(source, """$[?(@.a.*)]""", """[{"a": [1]},{"a": [1, 2]},{"a": {"x": "y"}}, {"a": {"x": "y", "w": "z"}}]""");
    }
    [Test, Ignore("TODO")]
    public void FilterExpressionWithNotEquals()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "some"},
    {"key": "42"},
    {"key": null},
    {"key": 420},
    {"key": ""},
    {"key": {}},
    {"key": []},
    {"key": [42]},
    {"key": {"key": 42}},
    {"key": {"some": 42}},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?(@.key!=42)]""", """
            [
              {
                "key": 0
              },
              {
                "key": -1
              },
              {
                "key": 1
              },
              {
                "key": 41
              },
              {
                "key": 43
              },
              {
                "key": 42.0001
              },
              {
                "key": 41.9999
              },
              {
                "key": 100
              },
              {
                "key": "some"
              },
              {
                "key": "42"
              },
              {
                "key": null
              },
              {
                "key": 420
              },
              {
                "key": ""
              },
              {
                "key": {}
              },
              {
                "key": []
              },
              {
                "key": [
                  42
                ]
              },
              {
                "key": {
                  "key": 42
                }
              },
              {
                "key": {
                  "some": 42
                }
              },
              {
                "some": "value"
              }
            ]
            """);
    }
    [Test]
    public void FilterExpressionWithNotEqualsArrayOrEqualsTrue()
    {
        var source = """
[
  {"d": ["v1", "v2"] },
  {"d": ["a", "b"] },
  {"d" : true}
]
""";
        AssertInvalidQuery(source, """$[?((@.d!=["v1","v2"]) || (@.d == true))]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithParentAxisOperator()
    {
        var source = """
[
    {
      "title": "Sayings of the Century",
      "bookmarks": [{
          "page": 40
      }]
    },
    {
      "title": "Sword of Honour",
      "bookmarks": [
        {
            "page": 35
        },
        {
            "page": 45
        }
      ]
    },
    {
      "title": "Moby Dick",
      "bookmarks": [
        {
            "page": 3035
        },
        {
            "page": 45
        }
      ]
    }
]
""";
        AssertInvalidQuery(source, """$[*].bookmarks[?(@.page == 45)]^^^""", """[]""");
    }
    [Test]
    public void FilterExpressionWithRegularExpression()
    {
        var source = """
[
  {"name": "hullo world"},
  {"name": "hello world"},
  {"name": "yes hello world"},
  {"name": "HELLO WORLD"},
  {"name": "good bye"}
]
""";
        AssertInvalidQuery(source, """$[?(@.name=~/hello.*/)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithRegularExpressionFromMember()
    {
        var source = """
[
  {"name": "hullo world"},
  {"name": "hello world"},
  {"name": "yes hello world"},
  {"name": "HELLO WORLD"},
  {"name": "good bye"},
  {"pattern": "hello.*"}
]
""";
        AssertInvalidQuery(source, """$[?(@.name=~/@.pattern/)]""", """[]""");
    }
    [Test, Ignore("TODO")]
    public void FilterExpressionWithSetWiseComparisonToScalar()
    {
        var source = """
[[1,2],[3,4],[5,6]]
""";
        AssertQueryResult(source, """$[?(@[*]>=4)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithSetWiseComparisonToSet()
    {
        var source = """
{"x":[[1,2],[3,4],[5,6]],"y":[3,4,5]}
""";
        AssertQueryResult(source, """$.x[?(@[*]>=$.y[*])]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithSingleEqual()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "some"},
    {"key": "42"},
    {"key": null},
    {"key": 420},
    {"key": ""},
    {"key": {}},
    {"key": []},
    {"key": [42]},
    {"key": {"key": 42}},
    {"key": {"some": 42}},
    {"some": "value"}
]
""";
        AssertInvalidQuery(source, """$[?(@.key=42)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithSubfilter()
    {
        var source = """
[
    {"a": [{"price": 1}, {"price": 3}]},
    {"a": [{"price": 11}]},
    {"a": [{"price": 8}, {"price": 12}, {"price": 3}]},
    {"a": []}
]
""";
        AssertQueryResult(source, """$[?(@.a[?(@.price>10)])]""", """[{"a": [{"price": 11}]},{"a": [{"price": 8}, {"price": 12}, {"price": 3}]}]""");
    }
    [Test]
    public void FilterExpressionWithSubpaths()
    {
        var source = """
[
  {"a": {"b": 3}},
  {"a": {"b": 2}}
]
""";
        AssertQueryResult(source, """$[?(@.a.b==3)]""", """[{"a": {"b": 3}}]""");
    }
    [Test]
    public void FilterExpressionWithSubpathsDeeplyNested()
    {
        var source = """
[{"a": {"b": {"c": 3}}}, {"a": 3}, {"c": 3}, {"a": {"b": {"c": 2}}}]
""";
        AssertQueryResult(source, """$[?(@.a.b.c==3)]""", """[{"a": {"b": {"c": 3}}}]""");
    }
    [Test]
    public void FilterExpressionWithSubtraction()
    {
        var source = """
[{"key": 60}, {"key": 50}, {"key": 10}, {"key": -50}, {"key-50": -100}]
""";
        AssertInvalidQuery(source, """$[?(@.key-50==-100)]""", """[{"a": {"b": {"c": 3}}}]""");
    }
    [Test]
    public void FilterExpressionWithTautologicalComparison()
    {
        var source = """
[1, 3, "nice", true, null, false, {}, [], -1, 0, ""]
""";
        AssertQueryResult(source, """$[?(1==1)]""", source);
    }
    [Test]
    public void FilterExpressionWithTripleEqual()
    {
        var source = """
[{"key": 60}, {"key": 42}, {"key": 10}, {"key": -50}, {"key-50": -100}]
""";
        AssertInvalidQuery(source, """$[?(@.key===42)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithValue()
    {
        var source = """
[
  {"some": "some value"},
  {"key": true},
  {"key": false},
  {"key": null},
  {"key": "value"},
  {"key": ""},
  {"key": 0},
  {"key": 1},
  {"key": -1},
  {"key": 42},
  {"key": {}},
  {"key": []}
]
""";
        AssertQueryResult(source, """$[?(@.key)]""", """
            [ {"key": true},
              {"key": false},
              {"key": null},
              {"key": "value"},
              {"key": ""},
              {"key": 0},
              {"key": 1},
              {"key": -1},
              {"key": 42},
              {"key": {}},
              {"key": []}]
            """);
    }
    [Test]
    public void FilterExpressionWithValueAfterDotNotationWithWildcardOnArrayOfObjects()
    {
        var source = """
[
  {
    "some": "some value"
  },
  {
    "key": "value"
  }
]
""";
        AssertQueryResult(source, """$.*[?(@.key)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithValueAfterRecursiveDescent()
    {
        var source = """
{"id": 2, "more": [{"id": 2}, {"more": {"id": 2}}, {"id": {"id": 2}}, [{"id": 2}]]}
""";
        AssertQueryResult(source, """$..[?(@.id)]""", """[{"id":2},{"id":2},{"id":{"id":2}},{"id":2},{"id":2}]""");
    }
    [Test]
    public void FilterExpressionWithValueFalse()
    {
        var source = """
[1, 3, "nice", true, null, false, {}, [], -1, 0, ""]
""";
        AssertQueryResult(source, """$[?(false)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithValueFromRecursiveDescent()
    {
        var source = """
[{"key": [{"child": 1}, {"child": 2}]}, {"key": [{"child": 2}]}, {"key": [{}]}, {"key": [{"something": 42}]}, {}]
""";
        AssertQueryResult(source, """$[?(@..child)]""", """[{"key":[{"child":1},{"child":2}]},{"key":[{"child":2}]}]""");
    }
    [Test]
    public void FilterExpressionWithValueNull()
    {
        var source = """
[1, 3, "nice", true, null, false, {}, [], -1, 0, ""]
""";
        AssertQueryResult(source, """$[?(null)]""", """[]""");
    }
    [Test]
    public void FilterExpressionWithValueTrue()
    {
        var source = """
[1, 3, "nice", true, null, false, {}, [], -1, 0, ""]
""";
        AssertQueryResult(source, """$[?(true)]""", source);
    }
    [Test]
    public void FilterExpressionWithoutParens()
    {
        var source = """
[
    {"key": 0},
    {"key": 42},
    {"key": -1},
    {"key": 1},
    {"key": 41},
    {"key": 43},
    {"key": 42.0001},
    {"key": 41.9999},
    {"key": 100},
    {"key": "some"},
    {"key": "42"},
    {"key": null},
    {"key": 420},
    {"key": ""},
    {"key": {}},
    {"key": []},
    {"key": [42]},
    {"key": {"key": 42}},
    {"key": {"some": 42}},
    {"some": "value"}
]
""";
        AssertQueryResult(source, """$[?@.key==42]""", """[{"key": 42}]""");
    }
    [Test]
    public void FilterExpressionWithoutValue()
    {
        var source = """
[
  {"some": "some value"},
  {"key": true},
  {"key": false},
  {"key": null},
  {"key": "value"},
  {"key": ""},
  {"key": 0},
  {"key": 1},
  {"key": -1},
  {"key": 42},
  {"key": {}},
  {"key": []}
]
""";
        AssertQueryResult(source, """$[?(@.key)]""", """
            [  {"key": true},
              {"key": false},
              {"key": null},
              {"key": "value"},
              {"key": ""},
              {"key": 0},
              {"key": 1},
              {"key": -1},
              {"key": 42},
              {"key": {}},
              {"key": []}]
            """);
    }
    [Test]
    public void FunctionSum()
    {
        var source = """
{"data": [1,2,3,4]}
""";
        AssertInvalidQuery(source, """$.data.sum()""", """[]""");
    }
    [Test]
    public void ParensNotation()
    {
        var source = """
{"key": 1, "some": 2, "more": 3}
""";
        AssertInvalidQuery(source, """$(key,more)""", """[]""");
    }
    [Test]
    public void RecursiveDescent()
    {
        var source = """
[{"a": {"b": "c"}}, [0, 1]]
""";
        AssertInvalidQuery(source, """$..""", """[]""");
    }
    [Test]
    public void RecursiveDescentOnNestedArrays()
    {
        var source = """
[[0], [1]]
""";
        AssertQueryResult(source, """$..*""", """[[0],[1],0,1]""", true);
    }
    [Test]
    public void RecursiveDescentAfterDotNotation()
    {
        var source = """
{"some key": "value", "key": {"complex": "string", "primitives": [0, 1]}}
""";
        AssertInvalidQuery(source, """$.key..""", """[]""");
    }
    [Test]
    public void Root()
    {
        var source = """
{
    "key": "value",
    "another key": {
        "complex": [
            "a",
            1
        ]
    }
}
""";
        AssertQueryResult(source, """$""", """[{"key": "value","another key": {"complex": ["a",1]}}]""");
    }
    [Test] public void RootOnScalar() => AssertQueryResult("42", """$""", """[42]""");
    [Test] public void RootOnScalarFalse() => AssertQueryResult("""false""", """$""", """[false]""");
    [Test] public void RootOnScalarTrue() => AssertQueryResult("""true""", """$""", """[true]""");
    [Test]
    public void ScriptExpression()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertInvalidQuery(source, """$[(@.length-1)]""", """[]""");
    }
    [Test]
    public void Union()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, """$[0,1]""", """["first","second"]""");
    }
    [Test]
    public void UnionWithDuplicationFromArray()
    {
        var source = """
["a"]
""";
        AssertQueryResult(source, """$[0,0]""", """["a","a"]""");
    }
    [Test]
    public void UnionWithDuplicationFromObject()
    {
        var source = """
{"a":1}
""";
        AssertQueryResult(source, """$['a','a']""", """[1,1]""");
    }
    [Test]
    public void UnionWithFilter()
    {
        var source = """
[{"key": 1}, {"key": 8}, {"key": 3}, {"key": 10}, {"key": 7}, {"key": 2}, {"key": 6}, {"key": 4}]
""";
        AssertQueryResult(source, """$[?(@.key<3),?(@.key>6)]""", """[{"key":1},{"key":2},{"key":8},{"key":10},{"key":7}]""");
    }
    [Test]
    public void UnionWithKeys()
    {
        var source = """
{
  "key": "value",
  "another": "entry"
}
""";
        AssertQueryResult(source, """$['key','another']""", """["value","entry"]""");
    }
    [Test]
    public void UnionWithKeysOnObjectWithoutKey()
    {
        var source = """
{
  "key": "value",
  "another": "entry"
}
""";
        AssertQueryResult(source, """$['missing','key']""", """["value"]""");
    }
    [Test]
    public void UnionWithKeysAfterArraySlice()
    {
        var source = """
[{"c":"cc1","d":"dd1","e":"ee1"},{"c":"cc2","d":"dd2","e":"ee2"}]
""";
        AssertQueryResult(source, """$[:]['c','d']""", """["cc1","dd1","cc2","dd2"]""", true);
    }
    [Test]
    public void UnionWithKeysAfterBracketNotation()
    {
        var source = """
[{"c":"cc1","d":"dd1","e":"ee1"},{"c":"cc2","d":"dd2","e":"ee2"}]
""";
        AssertQueryResult(source, """$[0]['c','d']""", """["cc1","dd1"]""", true);
    }
    [Test]
    public void UnionWithKeysAfterDotNotationWithWildcard()
    {
        var source = """
[{"c":"cc1","d":"dd1","e":"ee1"},{"c":"cc2","d":"dd2","e":"ee2"}]
""";
        AssertQueryResult(source, """$.*['c','d']""", """["cc1","dd1","cc2","dd2"]""", true);
    }
    [Test]
    public void UnionWithKeysAfterRecursiveDescent()
    {
        var source = """
[{"c":"cc1","d":"dd1","e":"ee1"}, {"c": "cc2", "child": {"d": "dd2"}}, {"c": "cc3"}, {"d": "dd4"}, {"child": {"c": "cc5"}}]
""";
        AssertQueryResult(source, """$..['c','d']""", """["cc1","cc2","cc3","cc5","dd1","dd2","dd4"]""", true);
    }
    [Test]
    public void UnionWithNumbersInDecreasingOrder()
    {
        var source = """
[1,2,3,4,5]
""";
        AssertQueryResult(source, """$[4,1]""", """[5,2]""");
    }
    [Test]
    public void UnionWithRepeatedMatchesAfterDotNotationWithWildcard()
    {
        var source = """
{
  "a": ["string", null, true],
  "b": [false, "string", 5.4]
}
""";
        AssertQueryResult(source, """$.*[0,:5]""", """["string","string",null,true,false,false,"string",5.4]""");
    }
    [Test]
    public void UnionWithSliceAndNumber()
    {
        var source = """
[1,2,3,4,5]
""";
        AssertQueryResult(source, """$[1:3,4]""", """[2,3,5]""");
    }
    [Test]
    public void UnionWithSpaces()
    {
        var source = """
["first", "second", "third"]
""";
        AssertQueryResult(source, """$[ 0 , 1 ]""", """["first","second"]""");
    }
    [Test]
    public void UnionWithWildcardAndNumber()
    {
        var source = """
["first", "second", "third", "forth", "fifth"]
""";
        AssertQueryResult(source, """$[*,1]""", """["first","second","third","forth","fifth","second"]""");
    }
}