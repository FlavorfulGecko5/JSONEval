class IntOperand : Operand
{
    public int value { get; private set; }

    public IntOperand(int vParam)
    {
        value = vParam;
    }

    public IntOperand(string vParam)
    {
        value = Int32.Parse(vParam);
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public Operand Add(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new IntOperand(value + b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value + b2.value);
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
                throw new Exception("Cannot add this type to an Integer");
        }
    }

    public Operand UnaryAdd()
    {
        return this;
    }

    public Operand Sub(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new IntOperand(value - b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value - b2.value);
            default:
                throw new Exception("Cannot subtract this type from an Integer");
        }
    }

    public Operand UnarySub()
    {
        return new IntOperand(value * -1);
    }

    public Operand Mult(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new IntOperand(value * b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value * b2.value);
            default:
                throw new Exception("Cannot multiply this type to an Integer");
        }
    }

    public Operand Div(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new IntOperand(value / b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value / b2.value);
            default:
                throw new Exception("Cannot divide this type from an Integer");
        }
    }

    public Operand Rem(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new IntOperand(value % b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value % b2.value);
            default:
                throw new Exception("Cannot take the remainder of an integer using this type");
        }
    }
}