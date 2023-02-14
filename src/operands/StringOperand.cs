// Only string literals read from expressions should use this
class StringOperand : PrimitiveOperand
{
    public string value { get; private set; }

    public StringOperand(string vParam)
    {
        value = vParam;
    }

    public override string ToString()
    {
        return value;
    }

    public PrimitiveOperand Add(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new StringOperand(value + b1.value);
            case DecimalOperand b2:
                return new StringOperand(value + b2.value);
            case BoolOperand b3:
                return new StringOperand(value + b3.value);
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
                throw new Exception("Cannot add this type to a String");
        }
    }

    public PrimitiveOperand UnaryAdd()
    {
        throw new Exception("Cannot unary add a string");
    }

    public PrimitiveOperand Sub(PrimitiveOperand b)
    {
        throw new Exception("Cannot subtract from a string");
    }

    public PrimitiveOperand UnarySub()
    {
        throw new Exception("Cannot unary subtract a string");
    }

    public PrimitiveOperand Mult(PrimitiveOperand b)
    {
        throw new Exception("Cannot multiply a string");
    }

    public PrimitiveOperand Div(PrimitiveOperand b)
    {
        throw new Exception("Cannot divide a string");
    }

    public PrimitiveOperand Rem(PrimitiveOperand b)
    {
        throw new Exception("Cannot take the remainder of a string");
    }

    public PrimitiveOperand And(PrimitiveOperand b)
    {
        throw new Exception("Invalid Bitwise/Logical AND operand combination");
    }

    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        throw new Exception("Invalid Bitwise/Logical OR operand combination");
    }

    public PrimitiveOperand Not()
    {
        throw new Exception("Cannot perform bitwise/logical not on a string");
    }

    public PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4: return new BoolOperand(value.Equals(b4.value));
            default:
                throw new Exception("Invalid equality comparison");
        }
    }

    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4: return new BoolOperand(!value.Equals(b4.value));
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