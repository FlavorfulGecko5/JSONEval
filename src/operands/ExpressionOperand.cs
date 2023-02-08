// String variables from configuration files should be regarded as sub-expressions
// being substituted into the main expression 
class ExpressionOperand : Operand
{
    public string value { get; private set; }

    public ExpressionOperand(string vParam)
    {
        value = vParam;
    }

    // Future: Create multiple interfaces for different operand types
    public Operand Add(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public Operand UnaryAdd()
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public Operand Sub(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public Operand UnarySub()
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public Operand Mult(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public Operand Div(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public Operand Rem(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

}