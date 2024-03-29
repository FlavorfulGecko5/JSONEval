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
/// Intended to be thrown by the expression parser/evaluator when 
/// an expression cannot be resolved for any predictable reason.
/// </summary>
public class ExpressionParsingException : Exception
{
    /// <param name="msg">Exception message</param>
    public ExpressionParsingException(string msg) : base(msg) {}
}

/// <summary>
/// Intended to be thrown by PrimitiveOperands when an operation function
/// cannot return a result.
/// </summary>
public class OperatorEvaluationException : Exception
{
    /// <param name="msg">Exception message</param>
    public OperatorEvaluationException(string msg) : base(msg) {}
}

/// <summary>
/// Intended to be thrown by Coded Functions when their evaluation function
/// cannot return a result for any predictable reason.
/// </summary>
public class CodedFunctionException : Exception
{
    /// <param name="msg">Exception message</param>
    public CodedFunctionException(string msg) : base(msg) {}
}