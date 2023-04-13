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
/// Contains <see cref="FunctionDef"/> instances for the Evaluator's
/// standard function library.
/// </summary>
public static class PrefabFunctions
{
    /*
    * See documentation for more information on all standard coded functions
    */

    /// <summary>
    /// Conditional If/Else Coded Function
    /// </summary>
    public static readonly CodedFunction IfElse = new CodedFunction(
        parms =>
        {
            switch (parms["!0"])
            {
                case BoolOperand conditionalResult:
                    if (conditionalResult.value)
                        return Evaluator.Evaluate((ExpressionOperand)parms["!1"]);
                    else
                        return Evaluator.Evaluate((ExpressionOperand)parms["!2"]);
                default:
                    throw new CodedFunctionException("The first parameter of an If function must resolve to a Boolean");
            }
        },

        FxParamType.PRIMITIVE,
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    );

    /// <summary>
    /// Restricted For Loop Coded Function
    /// </summary>
    public static readonly CodedFunction Loop = new CodedFunction(
        parms =>
        {
            IntOperand start;
            IntOperand end;
            PrimitiveOperand result = (PrimitiveOperand)parms["!2"];
            ExpressionOperand loopingExp = (ExpressionOperand)parms["!3"];
            string incVar = "!";

            try
            {
                start = (IntOperand)parms["!0"];
                end = (IntOperand)parms["!1"];
            }
            catch (System.InvalidCastException)
            {
                throw new CodedFunctionException("The Loop function's first two parameters must resolve to Integers.");
            }

            while (true) // Allows for nested loops calls to properly function
            {
                incVar += 'i';
                if (!loopingExp.localVars.ContainsKey(incVar))
                    break;
            }

            for (int i = start.value; i <= end.value; i++)
            {
                loopingExp.localVars[incVar] = new IntOperand(i);
                result = result.Add(Evaluator.Evaluate(loopingExp));
            }

            loopingExp.localVars.Remove(incVar);
            return result;
        },
        FxParamType.PRIMITIVE,
        FxParamType.PRIMITIVE,
        FxParamType.PRIMITIVE,
        FxParamType.EXPRESSION
    );

    /// <summary>
    /// Short-Circuiting Logical AND Coded Function
    /// </summary>
    public static readonly CodedFunction And = new CodedFunction(
        parms =>
        {
            try
            {
                PrimitiveOperand leftResult = Evaluator.Evaluate((ExpressionOperand)parms["!0"]);
                if (((BoolOperand)leftResult).value)
                {
                    PrimitiveOperand rightResult = Evaluator.Evaluate((ExpressionOperand)parms["!1"]);
                    if (((BoolOperand)rightResult).value)
                        return BoolOperand.TRUE;
                    return BoolOperand.FALSE;
                }
                else return BoolOperand.FALSE;
            }
            catch (System.InvalidCastException)
            {
                throw new CodedFunctionException("Both And function parameters must resolve to booleans");
            }            
        },
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    );

    /// <summary>
    /// Short-Circuiting Logical OR Coded Function
    /// </summary>
    public static readonly CodedFunction Or = new CodedFunction(
        parms =>
        {
            try
            {
                PrimitiveOperand leftResult = Evaluator.Evaluate((ExpressionOperand)parms["!0"]);
                if (((BoolOperand)leftResult).value)
                    return BoolOperand.TRUE;

                PrimitiveOperand rightResult = Evaluator.Evaluate((ExpressionOperand)parms["!1"]);
                if (((BoolOperand)rightResult).value)
                    return BoolOperand.TRUE;

                return BoolOperand.FALSE;
            }
            catch (System.InvalidCastException)
            {
                throw new CodedFunctionException("Both Or function parameters must resolve to booleans");
            }
        },
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    );

    /// <summary>
    /// Integer Casting Coded Function
    /// </summary>
    public static readonly CodedFunction IntCast = new CodedFunction(
        parms =>
        {
            switch (parms["!0"])
            {
                case IntOperand p1:
                    return p1;

                case DecimalOperand p2:
                    return new IntOperand((int)p2.value);

                case BoolOperand p3:
                    if (p3.value)
                        return new IntOperand(1);
                    else
                        return new IntOperand(0);

                case StringOperand p4:
                    int res;
                    if (!Int32.TryParse(p4.value, out res))
                        throw new CodedFunctionException("This string cannot be converted to an IntOperand");
                    return new IntOperand(res);

                default:
                    throw new CodedFunctionException("Unreachable");
            }
        },
        FxParamType.PRIMITIVE
    );

    /// <summary>
    /// Decimal Casting Coded Function
    /// </summary>
    public static readonly CodedFunction DecimalCast = new CodedFunction(
        parms =>
        {
            switch (parms["!0"])
            {
                case IntOperand p1:
                    return new DecimalOperand(p1.value);

                case DecimalOperand p2:
                    return p2;

                case BoolOperand p3:
                    if (p3.value)
                        return new DecimalOperand(1);
                    else
                        return new DecimalOperand(0);

                case StringOperand p4:
                    double res;
                    if (!Double.TryParse(p4.value, out res))
                        throw new CodedFunctionException("This string cannot be converted to a DecimalOperand");
                    return new DecimalOperand(res);

                default:
                    throw new CodedFunctionException("Unreachable");
            }
        },
        FxParamType.PRIMITIVE
    );

    /// <summary>
    /// Boolean Casting Coded Function
    /// </summary>
    public static readonly CodedFunction BoolCast = new CodedFunction(
        parms =>
        {
            switch (parms["!0"])
            {
                case IntOperand p1:
                    return BoolOperand.ToOperand(p1.value >= 1);

                case DecimalOperand p2:
                    return BoolOperand.ToOperand(p2.value >= 1);

                case BoolOperand p3:
                    return p3;

                case StringOperand p4:
                    bool res;
                    if (!Boolean.TryParse(p4.value, out res))
                        throw new CodedFunctionException("This string cannot be converted to a BoolOperand");
                    return BoolOperand.ToOperand(res);

                default:
                    throw new CodedFunctionException("Unreachable");
            }
        },
        FxParamType.PRIMITIVE
    );

    /// <summary>
    /// String Casting Coded Function
    /// </summary>
    public static readonly CodedFunction StringCast = new CodedFunction(
        parms =>
        {
            string val = "";
            switch (parms["!0"])
            {
                case IntOperand p1:
                    val = p1.value.ToString();
                    break;

                case DecimalOperand p2:
                    val = p2.value.ToString();
                    break;

                case BoolOperand p3:
                    val = p3.value.ToString();
                    break;

                case StringOperand p4:
                    return p4;
            }
            return new StringOperand(val);
        },
        FxParamType.PRIMITIVE
    );
}