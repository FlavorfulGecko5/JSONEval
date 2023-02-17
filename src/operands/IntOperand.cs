class IntOperand : PrimitiveOperand
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

    public PrimitiveOperand Add(PrimitiveOperand b)
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

    public PrimitiveOperand UnaryAdd()
    {
        return this;
    }

    public PrimitiveOperand Sub(PrimitiveOperand b)
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

    public PrimitiveOperand UnarySub()
    {
        return new IntOperand(value * -1);
    }

    public PrimitiveOperand Mult(PrimitiveOperand b)
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

    public PrimitiveOperand Div(PrimitiveOperand b)
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

    public PrimitiveOperand Rem(PrimitiveOperand b)
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

    public PrimitiveOperand And(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return new IntOperand(value & b1.value);
            default:
                throw new Exception("Invalid Bitwise/Logical AND operand combination");
        }
    }

    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return new IntOperand(value | b1.value);
            default:
                throw new Exception("Invalid Bitwise/Logical OR operand combination");
        }
    }

    public PrimitiveOperand Not()
    {
        return new IntOperand(~value);
    }

    public PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value == b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value == b2.value);
            default:
                throw new Exception("Invalid equality comparison");
        }
    }

    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value != b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value != b2.value); 
            default:
                throw new Exception("Invalid not-equals comparison");
        }
    }

    public PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value < b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value < b2.value);
            default:
                throw new Exception("Invalid Less-Than operand combination");
        }
    }

    public PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value <= b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value <= b2.value);
            default:
                throw new Exception("Invalid Less-Than-Equal operand combination");
        }
    }

    public PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value > b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value > b2.value);
            default:
                throw new Exception("Invalid Greater-Than operand combination");
        }
    }

    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value >= b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value >= b2.value);
            default:
                throw new Exception("Invalid Greater-Than-Equal operand combination");
        }
    }
}