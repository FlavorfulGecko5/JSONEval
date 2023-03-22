/*
Copyright (c) 2023 FlavorfulGecko5

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// An Operand representing a standard integer value
/// </summary>
public class IntOperand : PrimitiveOperand
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

    /// <inheritdoc/>
    public override bool Equals(Operand b)
    {
        switch(b)
        {
            case IntOperand b1:
            return value == b1.value;
        }
        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return value + " (Integer)";
    }

    /*
    * PrimitiveOperand method implementations
    */

    /// <inheritdoc/>
    public override string GetValueString()
    {
        return value.ToString();
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Add(PrimitiveOperand b)
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

    /// <inheritdoc/>
    public override PrimitiveOperand UnaryAdd()
    {
        return this;
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Sub(PrimitiveOperand b)
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

    /// <inheritdoc/>
    public override PrimitiveOperand UnarySub()
    {
        return new IntOperand(value * -1);
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Mult(PrimitiveOperand b)
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

    /// <inheritdoc/>
    public override PrimitiveOperand Div(PrimitiveOperand b)
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

    /// <inheritdoc/>
    public override PrimitiveOperand Rem(PrimitiveOperand b)
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

    /// <inheritdoc/>
    public override PrimitiveOperand And(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return new IntOperand(value & b1.value);
            default:
                throw GenerateError("bitwise and operations", "non-integer");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Or(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return new IntOperand(value | b1.value);
            default:
                throw GenerateError("bitwise or operations", "non-integer");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Not()
    {
        return new IntOperand(~value);
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value == b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value == b2.value);
            default:
                throw GenerateError("equality comparisons", "non-numerical value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value != b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value != b2.value); 
            default:
                throw GenerateError("inequality comparisons", "non-numerical value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value < b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value < b2.value);
            default:
                throw GenerateError("less-than comparisons", "non-numerical value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value <= b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value <= b2.value);
            default:
                throw GenerateError("less-than equal comparisons", "non-numerical value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1: return BoolOperand.ToOperand(value > b1.value);
            case DecimalOperand b2: return BoolOperand.ToOperand(value > b2.value);
            default:
                throw GenerateError("greater-than comparisons", "non-numerical value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
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