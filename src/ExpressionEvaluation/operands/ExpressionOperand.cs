namespace JSONEval.ExpressionEvaluation;
// String variables from configuration files should be regarded as sub-expressions
// being substituted into the main expression 
class ExpressionOperand : Operand
{
    public string value { get; private set; }
    public VarDictionary localVars;

    public ExpressionOperand(string vParam)
    {
        value = vParam;
        localVars = new VarDictionary();
    }

    public ExpressionOperand(string vParam, VarDictionary lvParam)
    {
        value = vParam;
        localVars = lvParam;
    }
}