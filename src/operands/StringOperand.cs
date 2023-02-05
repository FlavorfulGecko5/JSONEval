// Only string literals read from expressions should use this
class StringOperand : Operand
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

    public override Operand Add(Operand b)
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

    public override Operand Sub(Operand b)
    {
        throw new Exception("Cannot subtract from a string");
    }

    public override Operand Mult(Operand b)
    {
        throw new Exception("Cannot multiply a string");
    }

    public override Operand Div(Operand b)
    {
        throw new Exception("Cannot divide a string");
    }

    public override Operand Rem(Operand b)
    {
        throw new Exception("Cannot take the remainder of a string");
    }
}