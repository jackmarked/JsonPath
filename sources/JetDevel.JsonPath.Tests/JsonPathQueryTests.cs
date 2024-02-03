using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace JetDevel.JsonPath.Tests;

sealed class JsonPathQueryTests: JsonPathQueryTestFixture
{
    [Test]
    public void Execute_WithoutSegment_ReturnsSameDocument()
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
    public void Execute_WithNamedSegment_ReturnsPropertyValue()
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
    public void Execute_WithWildcardSegment_ReturnsAllPropertyValues()
    {
        // Arrange.
        string json = """
{
   "a": 1,
   "b": 7
}
""";
        var services = new JsonPathServices();
        var expectedResult = JsonDocument.Parse("[1,7]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$.*";
        var query = services.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithWildcardSegmentOnArray_ReturnsPropertyValue()
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
        var services = new JsonPathServices();
        var query = services.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithWildcardAndIndexSegmentOnArrays_ReturnsPrpertyValue()
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
        var services = new JsonPathServices();
        var query = services.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithWildcardAndMultiIndexSegmentOnArrays_ReturnsPropertyValue()
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
        var services = new JsonPathServices();
        var query = services.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithWildcardAndNegativeIndexSegmentOnArrays_ReturnsPropertyValue()
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
        var services = new JsonPathServices();
        var query = services.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithOnOutOfBoundIndexesSegmentOnArray_ReturnsValue()
    {
        // Arrange.
        string json = """
[ 7, 2, 4]
""";
        var expectedResult = JsonDocument.Parse("[2, 4]");
        var document = JsonDocument.Parse(json);
        var pathQuerySource = "$[1, 2, 3]";
        var services = new JsonPathServices();
        var query = services.FromSource(pathQuerySource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithAllOnOutOfBoundIndexesSegmentOnArray_ReturnsValue()
    {
        // Arrange.
        var services = new JsonPathServices();
        var query = services.FromSource("$[-10, 7, 3]");
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
    public void ExecUte_WithAllOnOutOfBoundIndexesSegmentOnObject_ReturnsEmptyArray()
    {
        // Arrange.
        var services = new JsonPathServices();
        var query = services.FromSource("$[-10, 7, 3]");
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
    public void Execute_IndexesSegmentOnValue_ReturnsEmptyArray()
    {
        // Arrange.
        var services = new JsonPathServices();
        var query = services.FromSource("$[-10, 7, 3]");
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
    public void Execute_TooLongNamedSegments_ReturnsEmptyArray()
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
        var services = new JsonPathServices();
        var query = services.FromSource("$.a.b.c");
        var expectedResult = JsonDocument.Parse("[]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithEmptySliceSelector_ReturnsArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
[1, 2, 3, 4, 5]
""");
        var services = new JsonPathServices();
        var query = services.FromSource("$[::]");
        var expectedResult = JsonDocument.Parse("[1, 2, 3, 4, 5]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithOneElementSliceSelector_ReturnsArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
[1, 2, 3, 4, 5]
""");
        var services = new JsonPathServices();
        var query = services.FromSource("$[:2]");
        var expectedResult = JsonDocument.Parse("[1, 2]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test]
    public void Execute_WithNegativeStepSliceSelector_ReturnsReversedArray()
    {
        // Arrange.
        var document = JsonDocument.Parse("""
[1, 2, 3, 4, 5]
""");
        var services = new JsonPathServices();
        var query = services.FromSource("$[::-1]");
        var expectedResult = JsonDocument.Parse("[5, 4, 3, 2, 1]");

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult);
    }
    [Test, Explicit]
    public void Execute_()
    {
        var list = new ConcurrentBag<string>();
        var services = new JsonPathServices();
        // Arrange.
        for(int i = 0; i < 128; i++)
            try
            {
                ReadOnlySpan<byte> source = [(byte)'$', (byte)i];
                if(services.TryParse(source, out _))
                    list.Add(Encoding.UTF8.GetString(source));
            }
            catch { }
        for(int i = 0; i < 128; i++)
            Parallel.For(0, 128, j =>
            {
                try
                {
                    ReadOnlySpan<byte> source = [(byte)'$', (byte)i, (byte)j];

                    if(services.TryParse(source, out _))
                        list.Add(Encoding.UTF8.GetString(source));
                }
                catch { }
            });
        for(int i = 0; i < 128; i++)
            for(int j = 0; j < 128; j++)
                Parallel.For(0, 128, k =>
                {
                    try
                    {
                        ReadOnlySpan<byte> source = [(byte)'$', (byte)i, (byte)j, (byte)k];
                        if(services.TryParse(source, out _))
                            list.Add(Encoding.UTF8.GetString(source));
                    }
                    catch { }
                });
        for(int i = 0; i < 128; i++)
            for(int j = 0; j < 128; j++)
                for(int k = 0; k < 128; k++)
                Parallel.For(0, 128, l =>
                {
                    try
                    {
                        ReadOnlySpan<byte> source = [(byte)'$', (byte)i, (byte)j, (byte)k, (byte)l];
                        if(services.TryParse(source, out _))
                            list.Add(Encoding.UTF8.GetString(source));
                    }
                    catch { }
                });
        Console.WriteLine(list.ToArray());
    }
    [Test]
    public void Execute_DescendantSegment_ReturnsEmptyArray()
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
    public void Execute_DescendantSegmentDeep_ReturnsEmptyArray()
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
    public void RfcStartSamples()
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
        AssertQueryResult(source, "$.store.book[*].author", @"[""Nigel Rees"",""Evelyn Waugh"",""Herman Melville"",""J. R. R. Tolkien""]");
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
    [Test]
    public void FilterExpressionWithMatchFunction()
    {
        var source = """
[
  "1974-05-11",
  [1,2,3,4],
  [1,2,3]
]
""";
        AssertQueryResult(source, """$[?match(@, "1974-05-..")]""", """ ["1974-05-11"] """);
    }
    [Test]
    public void FilterExpressionWithMatchFunctionNotMatch()
    {
        var source = """
[
  "1974-07-11",
  [1,2,3]
]
""";
        AssertQueryResult(source, """$[?match(@, "1974-05-..")]""", """ [] """);
    }
    [Test]
    public void FilterExpressionWithSearchFunctionMatch()
    {
        var source = """
[
  {
    "author":"Dob",
    "number":1
  },
  {
    "author":"ZeleBoba",
    "number":2
  }
]
""";
        AssertQueryResult(source, """$[?search(@.author, "[BR]ob")]""", """ [{"author":"ZeleBoba","number":2}] """);
    }
    [Test]
    public void FilterExpressionWithSearchFunctionNotMatch()
    {
        var source = """
[
  {
    "author":"Dob",
    "number":1
  },
  {
    "author":"ZeleBoba",
    "number":2
  }
]
""";
        AssertQueryResult(source, """$[?search(@.author, "[WM]ob")]""", """ [] """);
    }
}