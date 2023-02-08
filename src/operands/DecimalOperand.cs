class DecimalOperand : Operand
{
    public double value { get; private set; }

    public DecimalOperand(double vParam)
    {
        value = vParam;
    }

    public DecimalOperand(string vParam)
    {
        value = Double.Parse(vParam);
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public override Operand Add(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new DecimalOperand(value + b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value + b2.value);
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
                throw new Exception("Cannot add this type to a decimal");
        }
    }

    public override Operand UnaryAdd()
    {
        return this;
    }

    public override Operand Sub(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new DecimalOperand(value - b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value - b2.value);
            default:
                throw new Exception("Cannot subtract this type from a decimal");
        }
    }

    public override Operand UnarySub()
    {
        return new DecimalOperand(value * -1);
    }

    public override Operand Mult(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new DecimalOperand(value * b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value * b2.value);
            default:
                throw new Exception("Cannot subtract this type from a decimal");
        }
    }

    public override Operand Div(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new DecimalOperand(value / b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value / b2.value);
            default:
                throw new Exception("Cannot subtract this type from a decimal");
        }
    }

    public override Operand Rem(Operand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new DecimalOperand(value % b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value % b2.value);
            default:
                throw new Exception("Cannot subtract this type from a decimal");
        }
    }
}