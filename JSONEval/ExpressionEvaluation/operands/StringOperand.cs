namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// An Operand representing a string value
/// </summary>
class StringOperand : PrimitiveOperand
{
    /// <summary>
    /// The Operand's string value
    /// </summary>
    public string value { get; private set; }

    /// <param name="vParam">The desired operand value</param>
    public StringOperand(string vParam)
    {
        value = vParam;
    }

    /*
    * Simplifies Exception generation
    */

    private OperatorEvaluationException GenerateError(string operatorDesc, string otherType)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc
            + " with a string and a " + otherType);
    }

    private OperatorEvaluationException GenerateError(string operatorDesc)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc
            + " with a string value.");
    }

    /*
    * PrimitiveOperand method implementations
    */

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
                throw new OperatorEvaluationException("Unreachable");
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