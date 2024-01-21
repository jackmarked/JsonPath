using System.Runtime.CompilerServices;
using System.Text.Json;

namespace JetDevel.JsonPath;
public readonly struct ExpressionValue
{
    readonly object? value;
    public static readonly ExpressionValue LogicalTrue = new(true);
    public static readonly ExpressionValue LogicalFalse = new(false);
    public static readonly ExpressionValue Nothing = new();
    public static readonly ExpressionValue Null = new(default(object));
    public static readonly ExpressionValue EmptyNodes = new(Array.Empty<JsonElement>());
    public ExpressionValue()
    {
        value = null;
        ValueType = ValueType.Nothing;
    }
    internal ExpressionValue(IReadOnlyList<JsonElement> value)
    {
        this.value = value;
        ValueType = ValueType.Nodes;
    }
    internal ExpressionValue(bool value)
    {
        this.value = value;
        ValueType = ValueType.Logical;
        PrimitiveKind = PrimitiveKind.Boolean;
    }
    internal ExpressionValue(JsonElement json)
    {
        this.value = json;
        ValueType = ValueType.Node;
        PrimitiveKind = PrimitiveKind.None;
    }
    public ExpressionValue(double value)
    {
        this.value = value;
        ValueType = ValueType.PrimitiveValue;
        PrimitiveKind = PrimitiveKind.Float;
    }
    public ExpressionValue(string value)
    {
        this.value = value;
        ValueType = ValueType.PrimitiveValue;
        PrimitiveKind = PrimitiveKind.String;
    }

    ExpressionValue(object? value)
    {
        this.value = value;
        ValueType = ValueType.PrimitiveValue;
        PrimitiveKind = PrimitiveKind.Null;
    }
    public ExpressionValue(long value)
    {
        this.value = value;
        ValueType = ValueType.PrimitiveValue;
        PrimitiveKind = PrimitiveKind.Integer;
    }
    public bool IsNothing => ValueType == ValueType.Nothing;
    public IReadOnlyList<JsonElement> AsNodes()
    {
        if(ValueType != ValueType.Nodes)
            throw new InvalidOperationException("Invalid value type.");
        return Unsafe.As<IReadOnlyList<JsonElement>>(value)!;
    }

    public bool AsBoolean()
    {
        if(ValueType == ValueType.Logical)
            return (bool)value!;
        if(ValueType == ValueType.PrimitiveValue && PrimitiveKind == PrimitiveKind.Boolean)
            return (bool)value!;
        throw new InvalidOperationException("Invalid value type.");
    }

    public double AsDouble()
    {
        if(PrimitiveKind == PrimitiveKind.Float)
            return (double)value!;
        throw new InvalidOperationException("Invalid value type.");
    }
    public long AsLong()
    {
        if(PrimitiveKind == PrimitiveKind.Integer)
            return (long)value!;
        throw new InvalidOperationException("Invalid value type.");
    }
    public string AsString()
    {
        if(PrimitiveKind == PrimitiveKind.String)
            return value!.ToString()!;
        throw new InvalidOperationException("Invalid value type.");
    }
    public JsonElement AsJson()
    {
        if(ValueType == ValueType.Node)
            return (JsonElement)value!;
        throw new InvalidOperationException("Invalid value type.");
    }

    public ValueType ValueType { get; }
    public PrimitiveKind PrimitiveKind { get; }
    public override string ToString()
    {
        return value == null ? "null" : (value.ToString() ?? string.Empty);
    }
}