// String variables from configuration files should be regarded as sub-expressions
// being substituted into the main expression 
class ExpressionOperand : VariableOperand
{
    public string value { get; private set; }

    public ExpressionOperand(string vParam)
    {
        value = vParam;
    }
}