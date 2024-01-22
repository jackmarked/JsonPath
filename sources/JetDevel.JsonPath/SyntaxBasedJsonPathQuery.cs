using System.Text.Json;
using JetDevel.JsonPath.CodeAnalysis;

namespace JetDevel.JsonPath;

sealed partial class SyntaxBasedJsonPathQuery: JsonPathQuery
{
    readonly JsonPathQuerySyntax jsonPathQuerySyntax;

    internal SyntaxBasedJsonPathQuery(JsonPathQuerySyntax jsonPathQuerySyntax, JsonPathServices services) : base(services)
    {
        this.jsonPathQuerySyntax = jsonPathQuerySyntax;
    }
    private protected override JsonDocument ExecuteCore(JsonDocument document, CancellationToken cancellationToken = default)
    {
        if(jsonPathQuerySyntax.Segments.Count < 1)
            return JsonSerializer.SerializeToDocument(new[] { document });
        var root = document.RootElement;
        var context = new QueryContext(root, Services, cancellationToken);
        var result = ProcSegments(root, jsonPathQuerySyntax.Segments, context);
        return JsonSerializer.SerializeToDocument(result);
    }

    static private List<JsonElement> ProcSegments(JsonElement start, IReadOnlyList<SegmentSyntax> segments, QueryContext context)
    {
        List<JsonElement> result = [start];
        foreach(var segment in segments)
        {
            var newResult = new List<JsonElement>();
            if(segment is ChildSegmentSyntax childSegment)
            {
                var selector = childSegment.Selector;
                foreach(var element in result)
                    newResult.AddRange(Select(element, selector, context));
            }
            else if(segment is BracketedSelectionSegmentSyntax bracketedSegment)
            {
                foreach(var element in result)
                    foreach(var selector in bracketedSegment.Selectors)
                        newResult.AddRange(Select(element, selector, context));
            }
            else if(segment is DescendantSegmentSyntax descendantSegment)
            {
                var selector = descendantSegment.Selector;
                if(selector != null)
                    foreach(var element in result)
                        newResult.AddRange(Select(element, selector, context, true));
                else
                    foreach(var element in result)
                        foreach(var selector1 in descendantSegment.SelectionSegmentSyntax!.Selectors)
                            newResult.AddRange(Select(element, selector1, context, true));
            }
            result = newResult;
            if(result.Count < 1)
                return result;
        }

        return result;
    }

    private static List<JsonElement> Select(JsonElement element, SelectorSyntax selector, QueryContext context, bool descendant = false)
    {
        switch(selector)
        {
            case MemberNameShorthandSelectorSyntax memberNameShorthandSelector:
                return SelectPropertyValue(element, memberNameShorthandSelector.MemberName, context, descendant);
            case NameSelectorSyntax nameSelectorSyntax:
                return SelectPropertyValue(element, nameSelectorSyntax.Name, context, descendant);
            case WildcardSelectorSyntax:
                return WildcardSelect(element, descendant);
            case IndexSelectorSyntax indexSelector:
                return IndexSelect(element, indexSelector.Index, descendant);
            case SliceSelectorSyntax sliceSelector:
                return SliceSelector(element, sliceSelector, context, descendant);
            case FlterSelectorSyntax flterSelector:
                return FilterSelect(element, flterSelector.Expression, context, descendant);
            default:
                return [];
        }
    }

    private static List<JsonElement> FilterSelect(JsonElement element, ExpressionSyntax expression, QueryContext context, bool descendant)
    {
        var result = new List<JsonElement>();
        var enumerator = descendant ? EnumerateDescendant(element) : EnumerateCurrent(element);
        foreach(var current in enumerator)
        {
            var evaluationContext = new ExpressionEvaluationContext(context.Root, current, context.Services, context.CancellationToken);
            bool isSelected = EvaluateLogicalExpression(expression, evaluationContext);
            if(isSelected)
                result.Add(current);
        }
        return result;
    }

    static (long LowerBound, long UpperBound) GetNormalizedBounds(long? originalStart, long? originalEnd, long step, int length)
    {
        var start = originalStart ?? (step < 0 ? length - 1 : 0);
        var end = originalEnd ?? (step < 0 ? -length - 1 : length);
        long normalizedStart = NormalizeStartEnd(start, length);
        long normalizedEnd = NormalizeStartEnd(end, length);
        if(step < 0)
        {
            var upperBound = long.Min(long.Max(normalizedStart, -1), length - 1);
            var lowerBound = long.Min(long.Max(normalizedEnd, -1), length - 1);
            return (lowerBound, upperBound);
        }
        else
        {
            var lowerBound = long.Min(long.Max(normalizedStart, 0), length);
            var upperBound = long.Min(long.Max(normalizedEnd, 0), length);
            return (lowerBound, upperBound);
        }
    }
    static long NormalizeStartEnd(long value, int length)
    {
        if(value > -1)
            return value;
        return value + length;
    }
    private static List<JsonElement> SliceSelector(JsonElement source, SliceSelectorSyntax sliceSelector, QueryContext context, bool descendant)
    {
        var start = sliceSelector.Start;
        var end = sliceSelector.End;
        var step = sliceSelector.Step ?? 1;
        List<JsonElement> result = [];
        if(step == 0)
            return result;
        var elements = descendant ? EnumerateDescendantAndSelf(source, JsonValueKind.Array) : [source];
        foreach(var element in elements)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if(element.ValueKind != JsonValueKind.Array)
                continue;
            var length = element.GetArrayLength();
            (var lowerBound, var upperBound) = GetNormalizedBounds(start, end, step, length);
            if(step == 1 && lowerBound == 0 && upperBound == length)
                result.AddRange(element.EnumerateArray());
            else if(step > 0)
                for(var i = lowerBound; i < upperBound; i += step)
                    result.Add(element[(int)i]);
            else // step < 0
                for(var i = upperBound; i > lowerBound; i += step)
                    result.Add(element[(int)i]);
        }
        return result;
    }

    static List<JsonElement> IndexSelect(JsonElement source, int index, bool descendant)
    {
        void AddToResultIfNeeded(List<JsonElement> result, JsonElement item)
        {
            var length = item.GetArrayLength();
            var normalizedIndex = index < 0 ? length + index : index;
            if(normalizedIndex < length && normalizedIndex > -1)
                result.Add(item[normalizedIndex]);
        }
        var result = new List<JsonElement>();
        if(source.ValueKind == JsonValueKind.Array)
            AddToResultIfNeeded(result, source);
        if(descendant)
            foreach(var element in EnumerateDescendant(source, JsonValueKind.Array))
                AddToResultIfNeeded(result, element);
        return result;
    }
    private static List<JsonElement> SelectPropertyValue(JsonElement element, string propertyName, QueryContext context, bool descendant)
    {
        var newResult = new List<JsonElement>();
        if(element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out var value))
            newResult.Add(value);
        if(descendant)
            foreach(var descendantElement in EnumerateDescendant(element, JsonValueKind.Object))
                if(descendantElement.TryGetProperty(propertyName, out var descendanValue))
                    newResult.Add(descendanValue);
        context.CancellationToken.ThrowIfCancellationRequested();
        return newResult;
    }
    static List<JsonElement> WildcardSelect(JsonElement element, bool descendant = false)
    {
        if(descendant)
            return new(EnumerateDescendant(element));
        return new(EnumerateCurrent(element));
    }
    static IEnumerable<JsonElement> EnumerateCurrent(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject().Select(e => e.Value),
            JsonValueKind.Array => element.EnumerateArray(),
            _ => Enumerable.Empty<JsonElement>()
        };
    }
    static IEnumerable<JsonElement> EnumerateDescendant(JsonElement element, JsonValueKind valueKind = JsonValueKind.Undefined)
    {
        foreach(var value in EnumerateCurrent(element))
        {
            if(valueKind == JsonValueKind.Undefined || value.ValueKind == valueKind)
                yield return value;
            foreach(var descendant in EnumerateDescendant(value, valueKind))
                yield return descendant;
        }
    }
    static IEnumerable<JsonElement> EnumerateDescendantAndSelf(JsonElement element, JsonValueKind valueKind = JsonValueKind.Undefined)
    {
        if(valueKind == JsonValueKind.Undefined || element.ValueKind == valueKind)
            yield return element;
        foreach(var item in EnumerateDescendant(element, valueKind))
            yield return item;
    }
}