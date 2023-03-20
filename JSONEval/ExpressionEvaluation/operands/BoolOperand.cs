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
/// An Operand representing a Boolean value
/// </summary>
public class BoolOperand : PrimitiveOperand
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
    * Operand method implementations
    */

    /// <inheritdoc/>
    public bool Equals(Operand b)
    {
        switch(b)
        {
            case BoolOperand b1:
            return value == b1.value;
        }
        return false;
    }

    /*
    * PrimitiveOperand method implementations
    */

    /// <inheritdoc/>  
    public string GetValueString()
    {
        return value.ToString();
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public PrimitiveOperand UnaryAdd()
    {
        throw GenerateError("unary addition");
    }

    /// <inheritdoc/>
    public PrimitiveOperand Sub(PrimitiveOperand b)
    {
        throw GenerateError("subtraction");
    }

    /// <inheritdoc/>
    public PrimitiveOperand UnarySub()
    {
        throw GenerateError("unary subtraction");
    }

    /// <inheritdoc/>
    public PrimitiveOperand Mult(PrimitiveOperand b)
    {
        throw GenerateError("multiplication");
    }

    /// <inheritdoc/>
    public PrimitiveOperand Div(PrimitiveOperand b)
    {
        throw GenerateError("division");
    }

    /// <inheritdoc/>
    public PrimitiveOperand Rem(PrimitiveOperand b)
    {
        throw GenerateError("remainder operations");
    }

    /// <inheritdoc/>
    public PrimitiveOperand And(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value & b3.value);
            default:
                throw GenerateError("bitwise/logical and operations", "non-Boolean value");
        }
    }

    /// <inheritdoc/>
    public PrimitiveOperand Or(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value | b3.value);
            default:
                throw GenerateError("bitwise/logical or operations", "non-Boolean value");
        }
    }

    /// <inheritdoc/>
    public PrimitiveOperand Not()
    {
        return ToOperand(!value);
    }

    /// <inheritdoc/>
    public PrimitiveOperand Equal(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value == b3.value);
            default:
                throw GenerateError("equality comparisons", "non-Boolean value");
        }
    }

    /// <inheritdoc/>
    public PrimitiveOperand NotEqual(PrimitiveOperand b)
    {
        switch (b)
        {
            case BoolOperand b3: return ToOperand(value != b3.value);
            default:
                throw GenerateError("inequality comparisons", "non-Boolean value");
        }
    }

    /// <inheritdoc/>
    public PrimitiveOperand LessThan(PrimitiveOperand b)
    {
        throw GenerateError("less-than comparisons");
    }

    /// <inheritdoc/>
    public PrimitiveOperand LessThanEqual(PrimitiveOperand b)
    {
        throw GenerateError("less-than-equal comparisons");
    }

    /// <inheritdoc/>
    public PrimitiveOperand GreaterThan(PrimitiveOperand b)
    {
        throw GenerateError("greater-than comparisons");
    }

    /// <inheritdoc/>
    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b)
    {
        throw GenerateError("greater-than-equal comparisons");
    }
}