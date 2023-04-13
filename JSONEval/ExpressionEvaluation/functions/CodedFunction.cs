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
/// Function that requires execution of C# code to return it's desired results
/// </summary>
public class CodedFunction : FunctionDef
{
    /// <summary>
    /// Represents the definition for a <see cref="CodedFunction"/>
    /// </summary>
    /// <param name="parms">
    /// <para>Variables representing the function call's arguments where:</para>
    /// <para>- The number of argument groups matches the number required for this function</para>
    /// <para>- Each argument is of the appropriate <see cref="FxParamType"/></para>
    /// </param>
    /// <returns>The PrimitiveOperand result of the function call</returns>
    /// <exception cref="CodedFunctionException">
    /// Thrown when the function call cannot return a result for
    /// any predictable reason.
    /// </exception>
    /// <exception cref="ExpressionParsingException">
    /// Thrown if an expression parameter cannot be resolved to an operand
    /// for any predictable reason
    /// </exception>
    public delegate PrimitiveOperand Definition(VarDictionary parms);

    /// <summary>
    /// Method to invoke when the evaluator calls this function
    /// </summary>
    public Definition def;

    /// <param name="p_definition">Function to invoke when the evaluator calls this function</param>
    /// <param name="p_paramInfo">Type information for each parameter</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if the number of parameters is less than 1.
    /// </exception>
    public CodedFunction(Definition p_definition, params FxParamType[] p_paramInfo) : base(p_paramInfo) 
    {
        def = p_definition;
    }
}