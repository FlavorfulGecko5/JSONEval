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

    private EvaluationException GenerateError(string operatorDesc, string otherType)
    {
        return new EvaluationException("Cannot perform " + operatorDesc
            + " with a string and a " + otherType);
    }

    private EvaluationException GenerateError(string operatorDesc)
    {
        return new EvaluationException("Cannot perform " + operatorDesc
            + " with a string value.");
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
                throw new EvaluationException("Unreachable");
        }
    }

    public PrimitiveOperand UnaryAdd()
    {
        throw GenerateError("unary addition");
    }

    public PrimitiveOperand Sub(PrimitiveOperand b)
    {
        throw GenerateError("subtraction");
    }

    public PrimitiveOperand UnarySub()
    {
        throw GenerateError("unary subtraction");
    }

    public PrimitiveOperand Mult(PrimitiveOperand b)
    {
        throw GenerateError("multiplication");
    }

    public PrimitiveOperand Div(PrimitiveOperand b)
    {
        throw GenerateError("division");
    }

    public PrimitiveOperand Rem(PrimitiveOperand b)
    {
        throw GenerateError("remainder operations");
    }

    public PrimitiveOperand And(PrimitiveOperand b)
    {
        throw GenerateError("bitwise/logical and operations");
    }

    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        throw GenerateError("bitwise/logical or operations");
    }

    public PrimitiveOperand Not()
    {
        throw GenerateError("bitwise/logical not operations");
    }

    public PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4: return BoolOperand.ToOperand(value.Equals(b4.value));
            default:
                throw GenerateError("equality comparisons", "non-string value");
        }
    }

    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4: return BoolOperand.ToOperand(!value.Equals(b4.value));
            default:
                throw GenerateError("inequality comparisons", "non-string value");
        }
    }

    public PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        throw GenerateError("less-than comparisons");
    }

    public PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        throw GenerateError("less-than-equal comparisons");
    }

    public PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        throw GenerateError("greater-than comparisons");
    }

    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
    {
        throw GenerateError("greater-than-equal comparisons");
    }
}