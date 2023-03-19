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
/// Base class for defining functions useable in expressions
/// </summary>
public abstract class FunctionDef
{
    /// <summary>
    /// Details each parameter's type.
    /// The length of this array is the number of parameters.
    /// </summary>
    public FxParamType[] paramInfo;

    /// <summary>
    /// FunctionDef Constructor
    /// </summary>
    /// <param name="p_paramInfo">Parameter Type Information</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if the number of parameters is less than 1.
    /// </exception>
    public FunctionDef(params FxParamType[] p_paramInfo)
    {
        if(p_paramInfo.Length == 0)
            throw new System.ArgumentException("All functions need at least 1 parameter");
        paramInfo = p_paramInfo;
    }
}

/// <summary>
/// Used to specify the types of function parameters
/// </summary>
public enum FxParamType
{
    /// <summary>
    /// The parameter should be evaluated and resolved to a PrimitiveOperand
    /// before the function call is executed.
    /// </summary>
    PRIMITIVE,

    /// <summary>
    /// Pass the raw, unresolved parameter as an ExpressionOperand.
    /// This should have access to the same local variables
    /// as the original function.
    /// </summary>
    EXPRESSION,

    /// <summary>
    /// Parameter must be a defined variable's name after bracket contents resolved.
    /// All related variables (identified by naming rules) will also be copied as parameters
    /// </summary>
    REFERENCE
}