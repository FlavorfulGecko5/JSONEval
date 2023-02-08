class BoolOperand : PrimitiveOperand, VariableOperand
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

    public PrimitiveOperand Add(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
                throw new Exception("Cannot add this type to a Boolean");
        }
    }

    public PrimitiveOperand UnaryAdd()
    {
        throw new Exception("Cannot unary add a boolean");
    }

    public PrimitiveOperand Sub(PrimitiveOperand b)
    {
        throw new Exception("Cannot subtract from boolean");
    }

    // Unary subtraction functions as logical NOT for booleans
    public PrimitiveOperand UnarySub()
    {
        return new BoolOperand(!value);
    }

    public PrimitiveOperand Mult(PrimitiveOperand b)
    {
        throw new Exception("Cannot multiply a boolean");
    }

    public PrimitiveOperand Div(PrimitiveOperand b)
    {
        throw new Exception("Cannot divide a boolean");
    }

    public PrimitiveOperand Rem(PrimitiveOperand b)
    {
        throw new Exception("Cannot take remainder of a boolean");
    }
}