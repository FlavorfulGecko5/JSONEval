class BoolOperand : PrimitiveOperand
{
    public static readonly BoolOperand TRUE = new BoolOperand(){value = true};
    public static readonly BoolOperand FALSE = new BoolOperand(){value = false};

    public static BoolOperand ToOperand(bool b)
    {
        if(b) return TRUE;
        else return FALSE;
    }

    public bool value { get; private set; }

    // Use ToOperand() to get the appropriate BoolOperand for a boolean value
    private BoolOperand(){}

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

    public PrimitiveOperand UnarySub()
    {
        throw new Exception("Cannot unary subtract a boolean");
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

    public PrimitiveOperand And(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value & b3.value);
            default:
                throw new Exception("Invalid Bitwise/Logical AND operand combination");
        }
    }

    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value | b3.value);
            default:
                throw new Exception("Invalid Bitwise/Logical OR operand combination");
        }
    }

    public PrimitiveOperand Not()
    {
        return ToOperand(!value);
    }

    public PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value == b3.value);
            default:
                throw new Exception("Invalid equality comparison");
        }
    }

    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value != b3.value);
            default:
                throw new Exception("Invalid not-equals comparison");
        }
    }

    public PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        throw new Exception("Invalid Less-Than operand combination");
    }

    public PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        throw new Exception("Invalid Less-Than-Equal operand combination");
    }

    public PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        throw new Exception("Invalid Less-Than operand combination");
    }

    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
    {
        throw new Exception("Invalid Greater-Than-Equal operand combination");
    }
}