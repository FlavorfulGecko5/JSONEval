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
public interface Operand 
{
    /// <summary>
    /// Determines whether this Operand is equivalent to another Operand
    /// </summary>
    /// <param name="b">Operand to compare with</param>
    /// <returns>True if the two Operands are equal, otherwise False</returns>
    public bool Equals(Operand b);
}

/// <summary>
/// An Operand useable by operators to produce another Operand
/// </summary>
public interface PrimitiveOperand : Operand
{
    /// <returns>The string representation of the Operand's value</returns>
    public string ToString();

    /// <summary>
    /// Performs addition using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Add(PrimitiveOperand b);

    /// <summary>
    /// Performs unary addition using this operand.
    /// </summary>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand UnaryAdd();

    /// <summary>
    /// Performs subtraction using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Sub(PrimitiveOperand b);

    /// <summary>
    /// Performs unary subtraction using this operand.
    /// </summary>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand UnarySub();

    /// <summary>
    /// Performs multiplication using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Mult(PrimitiveOperand b);

    /// <summary>
    /// Performs division using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Div(PrimitiveOperand b);

    /// <summary>
    /// Performs the modulus (remainder) operation using this operand.
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Rem(PrimitiveOperand b);

    /// <summary>
    /// Performs the bitwise/logical AND operation using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand And(PrimitiveOperand b);

    /// <summary>
    /// Performs the bitwise/logical OR operation using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Or(PrimitiveOperand b);

    /// <summary>
    /// Performs the bitwise/logical NOT operation using this operand.
    /// </summary>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Not();

    /// <summary>
    /// Performs an equality comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand Equal(PrimitiveOperand b);

    /// <summary>
    /// Performs an inequality comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand NotEqual(PrimitiveOperand b);

    /// <summary>
    /// Performs a less-than comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand LessThan(PrimitiveOperand b);

    /// <summary>
    /// Performs a less-than-or-equal-to comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand LessThanEqual(PrimitiveOperand b);

    /// <summary>
    /// Performs a greater-than comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand GreaterThan(PrimitiveOperand b);

    /// <summary>
    /// Performs a greater-than-or-equal-to comparison using this operand
    /// </summary>
    /// <param name="b">Operand on the right side of the operator</param>
    /// <returns>A PrimitiveOperand containing the operation's result</returns>
    /// <exception cref="OperatorEvaluationException">
    /// The operation cannot be performed on this operand or combination of operands.
    /// </exception>
    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b);
}