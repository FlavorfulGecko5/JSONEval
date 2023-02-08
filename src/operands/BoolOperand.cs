class BoolOperand : Operand
{
    public bool value { get; private set; }

    public BoolOperand(bool vParam)
    {
        value = vParam;
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public Operand Add(Operand b)
    {
        switch (b)
        {
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
                throw new Exception("Cannot add this type to a Boolean");
        }
    }

    public Operand UnaryAdd()
    {
        throw new Exception("Cannot unary add a boolean");
    }

    public Operand Sub(Operand b)
    {
        throw new Exception("Cannot subtract from boolean");
    }

    // Unary subtraction functions as logical NOT for booleans
    public Operand UnarySub()
    {
        return new BoolOperand(!value);
    }

    public Operand Mult(Operand b)
    {
        throw new Exception("Cannot multiply a boolean");
    }

    public Operand Div(Operand b)
    {
        throw new Exception("Cannot divide a boolean");
    }

    public Operand Rem(Operand b)
    {
        throw new Exception("Cannot take remainder of a boolean");
    }
}