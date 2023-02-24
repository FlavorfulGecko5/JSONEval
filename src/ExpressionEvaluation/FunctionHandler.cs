namespace JSONEval.ExpressionEvaluation;
class FunctionHandler : Dictionary<string, FunctionDef>
{
    public FunctionHandler() : base(StringComparer.OrdinalIgnoreCase) {}
}