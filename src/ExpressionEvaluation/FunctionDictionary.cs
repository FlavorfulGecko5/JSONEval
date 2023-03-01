namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// Maps function names to a function definition
/// </summary>
class FunctionDictionary : Dictionary<string, FunctionDef>
{
    public FunctionDictionary() : base(StringComparer.OrdinalIgnoreCase) {}
}