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
/// An Operand representing a string value
/// </summary>
public class StringOperand : PrimitiveOperand
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
    * Operand method implementations
    */

    /// <inheritdoc/>
    public override bool Equals(Operand b)
    {
        switch(b)
        {
            case StringOperand b1:
            return value.Equals(b1.value);
        }
        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return "'" + value + "' (String)";
    }

    /*
    * PrimitiveOperand method implementations
    */

    /// <inheritdoc/>
    public override string GetValueString()
    {
        return value;
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Add(PrimitiveOperand b)
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

    /// <inheritdoc/>
    public override PrimitiveOperand UnaryAdd()
    {
        throw GenerateError("unary addition");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Sub(PrimitiveOperand b)
    {
        throw GenerateError("subtraction");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand UnarySub()
    {
        throw GenerateError("unary subtraction");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Mult(PrimitiveOperand b)
    {
        throw GenerateError("multiplication");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Div(PrimitiveOperand b)
    {
        throw GenerateError("division");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Rem(PrimitiveOperand b)
    {
        throw GenerateError("remainder operations");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand And(PrimitiveOperand b)
    {
        throw GenerateError("bitwise/logical and operations");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Or(PrimitiveOperand b)
    {
        throw GenerateError("bitwise/logical or operations");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Not()
    {
        throw GenerateError("bitwise/logical not operations");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4: return BoolOperand.ToOperand(value.Equals(b4.value));
            default:
                throw GenerateError("equality comparisons", "non-string value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case StringOperand b4: return BoolOperand.ToOperand(!value.Equals(b4.value));
            default:
                throw GenerateError("inequality comparisons", "non-string value");
        }
    }

    /// <inheritdoc/>
    public override PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        throw GenerateError("less-than comparisons");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        throw GenerateError("less-than-equal comparisons");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        throw GenerateError("greater-than comparisons");
    }

    /// <inheritdoc/>
    public override PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
    {
        throw GenerateError("greater-than-equal comparisons");
    }
}