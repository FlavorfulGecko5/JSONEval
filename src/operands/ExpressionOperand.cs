// String variables from configuration files should be regarded as sub-expressions
// being substituted into the main expression 
class ExpressionOperand : Operand
{
    public string value { get; private set; }
    public VariableHandler localVars;

    public ExpressionOperand(string vParam)
    {
        value = vParam;
        localVars = new VariableHandler();
    }

    public ExpressionOperand(string vParam, VariableHandler lvParam)
    {
        value = vParam;
        localVars = lvParam;
    }
}