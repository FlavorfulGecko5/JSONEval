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
/// An Operand representing a decimal (floating-point) value
/// </summary>
class DecimalOperand : PrimitiveOperand
{
    /// <summary>
    /// The Operand's decimal value
    /// </summary>
    public double value { get; private set; }

    /// <param name="vParam">The desired Operand value</param>
    public DecimalOperand(double vParam)
    {
        value = vParam;
    }

    /*
    * Simplifies Exception generation
    */

    private OperatorEvaluationException GenerateError(string operatorDesc, string otherType)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc
            + " with a decimal and a " + otherType);
    }

    private OperatorEvaluationException GenerateError(string operatorDesc)
    {
        return new OperatorEvaluationException("Cannot perform " + operatorDesc 
            + " with a decimal value.");
    }

    /*
    * Operand method implementations
    */

    public bool Equals(Operand b)
    {
        switch (b)
        {
            case DecimalOperand b1:
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
                return new DecimalOperand(value + b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value + b2.value);
            case StringOperand b4:
                return new StringOperand(value + b4.value);
            default:
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
                return new DecimalOperand(value - b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value - b2.value);
            default:
                throw GenerateError("subtraction", "non-numerical value");
        }
    }

    public PrimitiveOperand UnarySub()
    {
        return new DecimalOperand(value * -1);
    }

    public PrimitiveOperand Mult(PrimitiveOperand b)
    {
        switch (b)
        {
            case IntOperand b1:
                return new DecimalOperand(value * b1.value);
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
                return new DecimalOperand(value / b1.value);
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
                return new DecimalOperand(value % b1.value);
            case DecimalOperand b2:
                return new DecimalOperand(value % b2.value);
            default:
                throw GenerateError("remainder operations", "non-numerical value");
        }
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
                throw GenerateError("less-than-equal comparisons", "non-numerical value");
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
                throw GenerateError("greater-than-equal comparisons", "non-numerical value");
        }
    }
}