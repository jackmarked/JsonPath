namespace JetDevel.JsonPath;
partial class JsonPathServices
{
    public sealed class Builder
    {
        readonly List<FunctionDefinitionInstance> functions = [];
        public JsonPathServices Build()
        {
            var result = new JsonPathServices();
            functions.ForEach(result.RegisterFunctionDefinition);
            return result;
        }
        public Builder AddFunction(FunctionDefinitionInstance function)
        {
            if(function != null)
                functions.Add(function);
            return this;
        }
    }
}