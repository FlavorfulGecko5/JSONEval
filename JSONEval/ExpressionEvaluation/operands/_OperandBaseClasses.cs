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
/// Any Operand useable in expressions
/// </summary>
public abstract class Operand 
{
    /// <summary>
    /// Determines whether this Operand is equivalent to another Operand
    /// </summary>
    /// <param name="b">Operand to compare with</param>
    /// <returns>True if the two Operands are equal, otherwise False</returns>
    public abstract bool Equals(Operand b);

    /// <summary>
    /// Generates a string representation of this object
    /// </summary>
    /// <returns>
    /// A string stating the Operand's value, type, and any other relevant details. 
    /// </returns>
    public abstract override string ToString();

    /// <param name="a">Operand to cast</param>
    /// <exception cref="System.InvalidCastException">
    /// The Operand is not an instance of <see cref="IntOperand"/>
    /// </exception>
    public static explicit operator Int32(Operand a)
    {
        if(a is IntOperand)
            return ((IntOperand)a).value;
        throw new System.InvalidCastException("Operand could not be converted to an Integer");
    }

    /// <param name="a">Operand to cast</param>
    /// <exception cref="System.InvalidCastException">
    /// The Operand is not an instance of <see cref="IntOperand"/> or <see cref="DecimalOperand"/>
    /// </exception>
    public static explicit operator Double(Operand a)
    {
        switch(a)
        {
            case IntOperand a1:
            return ((IntOperand)a1).value;

            case DecimalOperand a2:
            return ((DecimalOperand)a2).value;

            default:
            throw new System.InvalidCastException("Operand could not be converted to a Decimal");
        }
    }

    /// <param name="a">Operand to cast</param>
    /// <exception cref="System.InvalidCastException">
    /// The Operand is not an instance of <see cref="BoolOperand"/>
    /// </exception>
    public static explicit operator Boolean(Operand a)
    {
        if(a is BoolOperand)
            return ((BoolOperand)a).value;
        throw new System.InvalidCastException("Operand could not be converted to a Boolean.");
    }
}

/// <summary>
/// An Operand useable by operators to produce another Operand
/// </summary>
public abstract class PrimitiveOperand : Operand
{
    /// <summary>
    /// Gets the string representation of this PrimitiveOperand's value
    /// </summary>
    /// <returns>The string representation of the PrimitiveOperand's value</returns>
    public abstract string GetValueString();

    /// <summary>
    /// Performs addition using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Add(PrimitiveOperand b);

    /// <summary>
    /// Performs unary addition using this operand.
    /// </summary>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand UnaryAdd();

    /// <summary>
    /// Performs subtraction using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Sub(PrimitiveOperand b);

    /// <summary>
    /// Performs unary subtraction using this operand.
    /// </summary>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand UnarySub();

    /// <summary>
    /// Performs multiplication using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Mult(PrimitiveOperand b);

    /// <summary>
    /// Performs division using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Div(PrimitiveOperand b);

    /// <summary>
    /// Performs the modulus (remainder) operation using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Rem(PrimitiveOperand b);

    /// <summary>
    /// Performs the bitwise/logical AND operation using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand And(PrimitiveOperand b);

    /// <summary>
    /// Performs the bitwise/logical OR operation using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Or(PrimitiveOperand b);

    /// <summary>
    /// Performs the bitwise/logical NOT operation using this operand.
    /// </summary>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Not();

    /// <summary>
    /// Performs an equality comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand Equal(PrimitiveOperand b);

    /// <summary>
    /// Performs an inequality comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand NotEqual(PrimitiveOperand b);

    /// <summary>
    /// Performs a less-than comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand LessThan(PrimitiveOperand b);

    /// <summary>
    /// Performs a less-than-or-equal-to comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand LessThanEqual(PrimitiveOperand b);

    /// <summary>
    /// Performs a greater-than comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand GreaterThan(PrimitiveOperand b);

    /// <summary>
    /// Performs a greater-than-or-equal-to comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public abstract PrimitiveOperand GreaterThanEqual(PrimitiveOperand b);
}