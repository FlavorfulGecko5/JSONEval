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
/// Function defined as an expression
/// </summary>
class ExpressionFunction : FunctionDef
{
    /// <summary>
    /// The expression to be evaluated when this function is called
    /// </summary>
    public string expression {get; private set;}

    /// <param name="p_exp">The expression to be evaluated when this function is called</param>
    /// <param name="p_paramInfo">Type information for each parameter</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if the number of parameters is less than 1.
    /// </exception>
    public ExpressionFunction(string p_exp, params FxParamType[] p_paramInfo) : base(p_paramInfo)
    {
        expression = p_exp;
    }

    /// <summary>
    /// Checks if two ExpressionFunctions are identical
    /// </summary>
    /// <param name="e">ExpressionFunction to compare with</param>
    /// <returns>True if the functions are identical, otherwise false</returns>
    public bool Equals(ExpressionFunction e)
    {
        if(expression != e.expression)
            return false;
        if(paramInfo.Length != e.paramInfo.Length)
            return false;
        for(int i = 0; i < paramInfo.Length; i++)
            if(paramInfo[i] != e.paramInfo[i])
                return false;
        return true;
    }
}