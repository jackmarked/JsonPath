using System.Text.Json.Nodes;
using System.Text.Json;

namespace JetDevel.JsonPath.Tests;

abstract class JsonPathQueryTestFixture
{
    protected JsonPathQueryTestFixture() { }

    protected static void AssertJsonEquivalent(JsonDocument document1, JsonDocument document2, bool arrayAsSet = false)
    {
        var root1 = Convert(document1.RootElement);
        var root2 = Convert(document2.RootElement);
        if (arrayAsSet)
        {
            if (root1.GetValueKind() != JsonValueKind.Array || root2.GetValueKind() != JsonValueKind.Array)
            {
                Assert.Fail("It is not array.");
            }
            var values1 = root1.AsArray().ToList();
            var values2 = root2.AsArray().ToList();
            if (values1.Count != values2.Count)
            {
                Assert.Fail();
            }
            for (int i = values1.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < values2.Count; j++)
                {
                    if (JsonNode.DeepEquals(values1[i], values2[j]))
                    {
                        values1.RemoveAt(i);
                        values2.RemoveAt(j);
                        break;
                    }
                }
            }
            Assert.That(values1, Is.Empty);
            Assert.That(values2, Is.Empty);
        }
        else
            Assert.That(JsonNode.DeepEquals(root1, root2), $"Expcted {document1.RootElement} but was {document2.RootElement}.");
    }
    protected static JsonNode Convert(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.Object => JsonObject.Create(element),
            JsonValueKind.Array => JsonArray.Create(element),
            JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False or JsonValueKind.Null => JsonValue.Create(element),
            _ => null,
        };
    protected void AssertQueryResult(string jsonSource, string querySource, string expectedResultSource, bool arrayAsSet = false)
    {
        // Arrange.
        var document = JsonDocument.Parse(jsonSource);
        var query = JsonPathQuery.FromSource(querySource);
        var expectedResult = JsonDocument.Parse(expectedResultSource);

        // Act.
        var queryResult = query.Execute(document);

        // Assert.
        AssertJsonEquivalent(expectedResult, queryResult, arrayAsSet);
    }
    protected void AssertInvalidQuery(string jsonSource, string querySource, string expectedResultSource, bool arrayAsSet = false)
    {
        Assert.That(() => AssertQueryResult(jsonSource, querySource, expectedResultSource, arrayAsSet), Throws.Exception);
    }
    protected void AssertQueryResultIsEmpty(string jsonSource, string querySource) =>
        AssertQueryResult(jsonSource, querySource, "[]");
}
