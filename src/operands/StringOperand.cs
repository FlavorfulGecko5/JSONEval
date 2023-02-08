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
}