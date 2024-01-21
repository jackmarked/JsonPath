using System.Collections.Concurrent;
using System.Text.Json;

namespace JetDevel.JsonPath.Tests;

sealed class JsonPathQueryTests: JsonPathQueryTestFixture
{
    [Test]
    public void Execte_WithoutSegment_ReturnsSameDocument()
    {
        // Arrange.
        string json = """
{
   "a": 1
}
""";
        AssertQueryResult(json, "$", """[{"a": 1}]""");
    }
    [Test]
    public void Execte_WithNamedSegment_ReturnsPropertyValue()
    {
        // Arrange.
        string json = """
{
   "a": 1
}
""";
        // Assert.
        AssertQueryResult(json, "$.a", "[1]");
        AssertQueryResult(json, "$['a']", "[1]");
    }
    [Test]
    public void Execte_WithWildcardSegment_ReturnsAllPrpertyValues()
    {
        // Arrange.
        string json = """
{
   "a": 1,
   "b": 7
}
""";
        var expectedResult = JsonDocument.Parse("[1,7]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$.*";
        var query = JsonPathQuery.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithWildcardSegmentOnArray_ReturnsPrpertyValue()
    {
        // Arrange.
        string json = """
{
   "a": 1,
   "b": [ 4, 7 ]
}
""";
        var expectedResult = JsonDocument.Parse("[4,7]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$.b.*";
        var query = JsonPathQuery.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithWildcardAndIndexSegmentOnArrays_ReturnsPrpertyValue()
    {
        // Arrange.
        string json = """
{
   "a": [ 1, 2 ],
   "b": [ 3, 4, 7 ]
}
""";
        var expectedResult = JsonDocument.Parse("[2,4]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$.*[1]";
        var query = JsonPathQuery.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithWildcardAndMultiIndexSegmentOnArrays_ReturnsPrpertyValue()
    {
        // Arrange.
        string json = """
{
   "a": [ 1, 2 ],
   "b": [ 3, 4, 7 ]
}
""";
        var expectedResult = JsonDocument.Parse("[2, 4, 7]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$.*[1, 2]";
        var query = JsonPathQuery.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithWildcardAndNegativeIndexSegmentOnArrays_ReturnsPrpertyValue()
    {
        // Arrange.
        string json = """
{
   "a": [ 1, 2 ],
   "b": [ 3, 4, 7 ]
}
""";
        var expectedResult = JsonDocument.Parse("[2, 7]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$.*[-1]";
        var query = JsonPathQuery.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithOnOutOfoundIndexesSegmentOnArray_ReturnsValue()
    {
        // Arrange.
        string json = """
[ 7, 2, 4]
""";
        var expectedResult = JsonDocument.Parse("[2, 4]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$[1, 2, 3]";
        var query = JsonPathQuery.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithAllOnOutOfoundIndexesSegmentOnArray_ReturnsValue()
    {
        // Arrange.
        var query = JsonPathQuery.FromSource("$[-10, 7, 3]");
        var document = JsonDocument.Parse("""
[ 7, 2, 4]
""");
        var expectedResult = JsonDocument.Parse("[]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithAllOnOutOfoundIndexesSegmentOnObject_ReturnsEmptyArray()
    {
        // Arrange.
        var query = JsonPathQuery.FromSource("$[-10, 7, 3]");
        var document = JsonDocument.Parse("""
{"a":7}
""");
        var expectedResult = JsonDocument.Parse("[]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_IndexesSegmentOnValue_ReturnsEmptyArray()
    {
        // Arrange.
        var query = JsonPathQuery.FromSource("$[-10, 7, 3]");
        var document = JsonDocument.Parse("""
"a"
""");
        var expectedResult = JsonDocument.Parse("[]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_TooLongNamedSegments_ReturnsEmptyArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
{
  "a":
  {
    "b": 2
  }
}
""");
        var query = JsonPathQuery.FromSource("$.a.b.c");
        var expectedResult = JsonDocument.Parse("[]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithEmptySliceSelector_ReturnsArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
[1, 2, 3, 4, 5]
""");
        var query = JsonPathQuery.FromSource("$[::]");
        var expectedResult = JsonDocument.Parse("[1, 2, 3, 4, 5]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithOneElementSliceSelector_ReturnsArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
[1, 2, 3, 4, 5]
""");
        var query = JsonPathQuery.FromSource("$[:2]");
        var expectedResult = JsonDocument.Parse("[1, 2]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execte_WithNegativeStepSliceSelector_ReturnsReversedArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
[1, 2, 3, 4, 5]
""");
        var query = JsonPathQuery.FromSource("$[::-1]");
        var expectedResult = JsonDocument.Parse("[5, 4, 3, 2, 1]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test, Ignore("")]
    public void Execte_()
    {
        var list = new ConcurrentBag<string>();
        // Arrange.
        for(int i = 0; i < 128; i++)
            try
            {
                var source = "$" + (char)i;
                var query = JsonPathQuery.FromSource(source);
                if(query != null)
                    list.Add(source);
            }
            catch { }
        for(int i = 0; i < 128; i++)
            Parallel.For(0, 128, j =>
            {
                try
                {
                    var source = "$" + (char)i + (char)j;
                    var query = JsonPathQuery.FromSource(source);
                    if(query != null)
                        list.Add(source);
                }
                catch { }
            });
        for(int i = 0; i < 128; i++)
            for(int j = 0; j < 128; j++)
                Parallel.For(0, 128, k =>
                {
                    try
                    {
                        var source = "$" + (char)i + (char)j + (char)k;
                        var query = JsonPathQuery.FromSource(source);
                        if(query != null)
                            list.Add(source);
                    }
                    catch { }
                });
    }
    [Test]
    public void Execte_DescendantSegment_ReturnsEmptyArray()
    {
        // Arrange.
        var source = """
{
  "a":
  {
    "b": 2
  },
  "c": 3
}
""";
        AssertQueryResult(source, "$..*", @"[{""b"": 2}, 2, 3]");
        AssertQueryResult(source, "$..[*]", @"[{""b"": 2}, 2, 3]");
        AssertQueryResult(source, "$..b", @"[2]");
    }
    [Test]
    public void Execte_DescendantSegmentDeep_ReturnsEmptyArray()
    {
        // Arrange.
        var source = """
{
  "a":
  {
    "b": [
        {"e": [5, 7]}
    ]
  },
  "c": 3
}
""";
        AssertQueryResult(source, "$..b..[1]", @"[7]");
        AssertQueryResult(source, "$..c", @"[3]");
        AssertQueryResult(source, "$.a[::]", @"[]");
    }
    [Test]
    public void RfcDescendantSamples()
    {
        var source = """
{ "store": {
    "book": [
      { "category": "reference",
        "author": "Nigel Rees",
        "title": "Sayings of the Century",
        "price": 8.95
      },
      { "category": "fiction",
        "author": "Evelyn Waugh",
        "title": "Sword of Honour",
        "price": 12.99
      },
      { "category": "fiction",
        "author": "Herman Melville",
        "title": "Moby Dick",
        "isbn": "0-553-21311-3",
        "price": 8.99
      },
      { "category": "fiction",
        "author": "J. R. R. Tolkien",
        "title": "The Lord of the Rings",
        "isbn": "0-395-19395-8",
        "price": 22.99
      }
    ],
    "bicycle": {
      "color": "red",
      "price": 399
    }
  }
}
""";
        AssertQueryResult(source, "$..author", @"[""Nigel Rees"",""Evelyn Waugh"",""Herman Melville"",""J. R. R. Tolkien""]");
        AssertQueryResult(source, "$..book[2].author", @"[""Herman Melville""]");
        AssertQueryResult(source, "$..book[2].publisher", @"[]");
        AssertQueryResult(source, "$..book[-1]", @"[{ ""category"": ""fiction"",""author"": ""J. R. R. Tolkien"",""title"": ""The Lord of the Rings"",""isbn"": ""0-395-19395-8"",""price"": 22.99}]");
        AssertQueryResult(source, "$..book[0,1].author", @"[""Nigel Rees"",""Evelyn Waugh""]");
        AssertQueryResult(source, "$..book[:2].author", @"[""Nigel Rees"",""Evelyn Waugh""]");

    }
    [Test]
    public void RfcSliceSamples()
    {
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[1:3]", "[1,2]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[5:]", "[5,6]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[1:5:2]", "[1,3]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[5:1:-2]", "[5,3]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[::-1]", "[6,5,4,3,2,1,0]");
    }
    [Test]
    public void RfcIndexSamples()
    {
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[1]", "[1]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[-2]", "[5]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[1,0,5]", "[1,0,5]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[1,9,4]", "[1,4]");
        AssertQueryResult("[0,1,2,3,4,5,6]", "$[-11,3,1]", "[3,1]");
    }
    [Test]
    public void RfcWildcardSamples()
    {
        var source = """
{
  "o": {"j": 1, "k": 2},
  "a": [5, 3]
}
""";
        AssertQueryResult(source, "$[*]", @"[{""j"": 1, ""k"": 2}, [5, 3]]");
        AssertQueryResult(source, "$.o[*]", @"[1,2]");
        AssertQueryResult(source, "$.o[*, *]", @"[1,2,1,2]");
        AssertQueryResult(source, "$.a[*]", @"[5,3]");
    }
    [Test]
    public void ExpressionCompareNumbersSamples()
    {
        var source = """
{
  "o": [
    {
      "name": "Bill",
      "isAutor": true
    },
    {
      "name": "Fill",
      "isAutor": false
    },
    {
      "name": "Mill",
      "isAutor": true
    }
  ]
}
""";
        AssertQueryResult(source, "$.o[?2 == 2].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2 == 3].name", @"[]");
        AssertQueryResult(source, "$.o[?2 != 3].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2 < 3].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2 <= 3].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2 > 3].name", @"[]");
        AssertQueryResult(source, "$.o[?2 >= 3].name", @"[]");

        AssertQueryResult(source, "$.o[?2.3 == 2.3].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2.2 == 3.3].name", @"[]");
        AssertQueryResult(source, "$.o[?2.2 != 3.2].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2.2 < 3.2].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2.2 <= 3.2].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?2.2 > 3.2].name", @"[]");
        AssertQueryResult(source, "$.o[?2.2 >= 3.2].name", @"[]");
        //AssertQueryResult(source, "$.o[?@.isAutor].name", @"[""Bill"",""Mill""]");
    }
    [Test]
    public void ExpressionCompareStringsSamples()
    {
        var source = """
{
  "o": [
    {
      "name": "Bill",
      "isAutor": true
    },
    {
      "name": "Fill",
      "isAutor": false
    },
    {
      "name": "Mill",
      "isAutor": true
    }
  ]
}
""";
        AssertQueryResult(source, "$.o[?'2' == '2'].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?'2' == '3'].name", @"[]");
        AssertQueryResult(source, "$.o[?'2' != '3'].name", @"[""Bill"", ""Fill"", ""Mill""]");
        //AssertQueryResult(source, "$.o[?@.isAutor].name", @"[""Bill"",""Mill""]");
    }
    [Test]
    public void ExpressionLogicalNotSamples()
    {
        var source = """
{
  "o": [
    {
      "name": "Bill",
      "isAutor": true
    },
    {
      "name": "Fill",
      "isAutor": false
    },
    {
      "name": "Mill",
      "isAutor": true
    }
  ]
}
""";
        AssertQueryResult(source, "$.o[?!('2' == '2')].name", @"[]");
        AssertQueryResult(source, "$.o[?!('2' == '3')].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?'2' != '3'].name", @"[""Bill"", ""Fill"", ""Mill""]");
    }
    [Test]
    public void ExpressionLogicalAndSamples()
    {
        var source = """
{
  "o": [
    {
      "name": "Bill",
      "isAutor": true
    },
    {
      "name": "Fill",
      "isAutor": false
    },
    {
      "name": "Mill",
      "isAutor": true
    }
  ]
}
""";
        AssertQueryResult(source, "$.o[?'3' == '3' && 4 == 4].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?'3' == '3' && 4 == 3].name", @"[]");
        AssertQueryResult(source, "$.o[?'3' == '4' && 4 == 4].name", @"[]");
    }
    [Test]
    public void ExpressionLogicalOrSamples()
    {
        var source = """
{
  "o": [
    {
      "name": "Bill",
      "isAutor": true
    },
    {
      "name": "Fill",
      "isAutor": false
    },
    {
      "name": "Mill",
      "isAutor": true
    }
  ]
}
""";
        AssertQueryResult(source, "$.o[?'3' == '2' || 4 == 4].name", """["Bill", "Fill", "Mill"]""");
        AssertQueryResult(source, "$.o[?'3' == '3' || 4 == 3].name", @"[""Bill"", ""Fill"", ""Mill""]");
        AssertQueryResult(source, "$.o[?'3' == '4' || 4 == 5].name", @"[]");
    }
}