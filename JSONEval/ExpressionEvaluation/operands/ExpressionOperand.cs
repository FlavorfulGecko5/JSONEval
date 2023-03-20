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
/// An Operand representing a complete expression
/// </summary>
public class ExpressionOperand : Operand
{
    /// <summary>
    /// The expression stored by this operand
    /// </summary>
    public string value { get; private set; }

    /// <summary>
    /// The non-global variables accessible by this expression
    /// </summary>
    public VarDictionary localVars;

    /// <summary>
    /// Constructs an ExpressionOperand with 0 predefined non-global variables
    /// </summary>
    /// <param name="vParam">The complete expression</param>
    public ExpressionOperand(string vParam)
    {
        value = vParam;
        localVars = new VarDictionary();
    }

    /// <summary>
    /// Constructs an ExpressionOperand with a predefined set of non-global variables
    /// </summary>
    /// <param name="vParam">The complete expression</param>
    /// <param name="lvParam">The non-global variables accessible to the expression</param>
    public ExpressionOperand(string vParam, VarDictionary lvParam)
    {
        value = vParam;
        localVars = lvParam;
    }

    /*
    * Operand method implementations
    */

    /// <inheritdoc/>
    public bool Equals(Operand b)
    {
        switch(b)
        {
            case ExpressionOperand b1:
            return value.Equals(b1.value) && localVars.Equals(b1.localVars);
        }
        return false;
    }
}