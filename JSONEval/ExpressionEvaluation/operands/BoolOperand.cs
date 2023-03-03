namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// An Operand representing a Boolean value
/// </summary>
class BoolOperand : PrimitiveOperand
{
    /// <summary>
    /// BoolOperand representing 'True'
    /// </summary>
    public static readonly BoolOperand TRUE = new BoolOperand(){value = true};

    /// <summary>
    /// BoolOperand representing 'False'
    /// </summary>
    public static readonly BoolOperand FALSE = new BoolOperand(){value = false};

    /// <summary>
    /// Converts a Boolean value to the appropriate BoolOperand
    /// </summary>
    /// <param name="b">The Boolean value</param>
    /// <returns>
    /// BoolOperand.TRUE or BoolOperand.FALSE, depending on the Boolean value.
    /// </returns>
    public static BoolOperand ToOperand(bool b)
    {
        if(b) return TRUE;
        else return FALSE;
    }

    /// <summary>
    /// The Operand's Boolean value
    /// </summary>
    public bool value { get; private set; }

    // Use ToOperand() to get the appropriate BoolOperand for a boolean value
    // It's unnecessary to ever have more than 2 BoolOperand objects instantiated
    private BoolOperand(){}

    /*
    * Simplifies Exception generation
    */

    private OperatorEvaluationException GenerateError(string operatorDesc, string otherType)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc
            + " with a Boolean and a " + otherType);
    }

    private OperatorEvaluationException GenerateError(string operatorDesc)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc
            + " with a Boolean value.");
    }

    /*
    * PrimitiveOperand method implementations
    */
    
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