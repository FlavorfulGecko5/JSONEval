namespace JSONEval.ExpressionEvaluation;
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

    private EvaluationException GenerateError(string operatorDesc, string otherType)
    {
        return new EvaluationException("Cannot perform " + operatorDesc
            + " with a Boolean and a " + otherType);
    }

    private EvaluationException GenerateError(string operatorDesc)
    {
        return new EvaluationException("Cannot perform " + operatorDesc
            + " with a Boolean value.");
    }

    public PrimitiveOperand Add(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
                throw GenerateError("addition", "non-string value");
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
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value & b3.value);
            default:
                throw GenerateError("bitwise/logical and operations", "non-Boolean value");
        }
    }

    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value | b3.value);
            default:
                throw GenerateError("bitwise/logical or operations", "non-Boolean value");
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
                throw GenerateError("equality comparisons", "non-Boolean value");
        }
    }

    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value != b3.value);
            default:
                throw GenerateError("inequality comparisons", "non-Boolean value");
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