// String variables from configuration files should be regarded as sub-expressions
// being substituted into the main expression 
class ExpressionOperand : Operand
{
    public string value { get; private set; }

    public ExpressionOperand(string vParam)
    {
        value = vParam;
    }

    public override Operand Add(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public override Operand Sub(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public override Operand Mult(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public override Operand Div(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

    public override Operand Rem(Operand b)
    {
        throw new Exception("Cannot use operations with ExpressionOperands");
    }

}