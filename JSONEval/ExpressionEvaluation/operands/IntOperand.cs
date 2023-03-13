namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// An Operand representing a standard integer value
/// </summary>
class IntOperand : PrimitiveOperand
{
    /// <summary>
    /// The Operand's integer value
    /// </summary>
    public int value { get; private set; }

    /// <param name="vParam">The desired operand value</param>
    public IntOperand(int vParam)
    {
        value = vParam;
    }

    /* 
    * Simplfies Exception generation 
    */

    private OperatorEvaluationException GenerateError(string operatorDesc, string otherType)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc
            + " with an integer and a " + otherType);
    }

    /*
    * Operand method implementations
    */

    public bool Equals(Operand b)
    {
        switch(b)
        {
            case IntOperand b1:
            return value == b1.value;
        }
        return false;
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
            case IntOperand b1:
                return new IntOperand(value + b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value + b2.value);
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            case BoolOperand: default:
                throw GenerateError("addition", "Boolean");
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
                throw GenerateError("subtraction", "non-numerical value");
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
                throw GenerateError("multiplication", "non-numerical value");
        }
    }

    public PrimitiveOperand Div(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1:
                if (b1.value == 0)
                    throw new OperatorEvaluationException("Attempted to divide by 0");
                return new IntOperand(value / b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value / b2.value);
            default:
                throw GenerateError("division", "non-numerical value");
        }
    }

    public PrimitiveOperand Rem(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1:
                if (b1.value == 0)
                    throw new OperatorEvaluationException("Attempted to take the remainder with 0");
                return new IntOperand(value % b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value % b2.value);
            default:
                throw GenerateError("remainder operations", "non-numerical value");
        }
    }

    public PrimitiveOperand And(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return new IntOperand(value & b1.value);
            default:
                throw GenerateError("bitwise and operations", "non-integer");
        }
    }

    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return new IntOperand(value | b1.value);
            default:
                throw GenerateError("bitwise or operations", "non-integer");
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
                throw GenerateError("equality comparisons", "non-numerical value");
        }
    }

    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value != b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value != b2.value); 
            default:
                throw GenerateError("inequality comparisons", "non-numerical value");
        }
    }

    public PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value < b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value < b2.value);
            default:
                throw GenerateError("less-than comparisons", "non-numerical value");
        }
    }

    public PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value <= b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value <= b2.value);
            default:
                throw GenerateError("less-than equal comparisons", "non-numerical value");
        }
    }

    public PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value > b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value > b2.value);
            default:
                throw GenerateError("greater-than comparisons", "non-numerical value");
        }
    }

    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value >= b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value >= b2.value);
            default:
                throw GenerateError("greater-than equal comparisons", "non-numerical value");
        }
    }
}