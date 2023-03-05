namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// Function that requires execution of C# code to return it's desired results
/// </summary>
abstract class CodedFunction : FunctionDef
{
    /// <param name="p_paramInfo">Type information for each parameter</param>
    /// <exception cref="System.ArgumentException">
    /// Thrown if the number of parameters is less than 1.
    /// </exception>
    public CodedFunction(params FxParamType[] p_paramInfo) : base(p_paramInfo) {}

    /// <summary>
    /// Executes this function on a given set of parameters
    /// </summary>
    /// <param name="parms">
    /// Variables representing the function call's arguments where:
    /// - The number of argument groups matches the number required for this function
    /// - Each argument is of the appropriate FxParamType
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
    public abstract PrimitiveOperand eval(VarDictionary parms);
}

/*
* See documentation for information on all standard coded functions
*/

class CodedFunction_IfElse : CodedFunction
{
    public CodedFunction_IfElse() : base(
        FxParamType.PRIMITIVE, 
        FxParamType.EXPRESSION, 
        FxParamType.EXPRESSION
    ) {}

    public override PrimitiveOperand eval(VarDictionary parms)
    {
        switch(parms["!0"])
        {
            case BoolOperand conditionalResult:
                if(conditionalResult.value)
                    return Evaluator.Evaluate((ExpressionOperand)parms["!1"]);
                else
                    return Evaluator.Evaluate((ExpressionOperand)parms["!2"]);
            default:
                throw new CodedFunctionException("The first parameter of an If function must resolve to a Boolean");
        }
    }
}

class CodedFunction_Loop : CodedFunction
{
    public CodedFunction_Loop() : base (
        FxParamType.PRIMITIVE,
        FxParamType.PRIMITIVE,
        FxParamType.PRIMITIVE,
        FxParamType.EXPRESSION
    ){}

    public override PrimitiveOperand eval(VarDictionary parms)
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
        catch(System.InvalidCastException)
        { 
            throw new CodedFunctionException("The Loop function's first two parameters must resolve to Integers.");
        }

        while(true) // Allows for nested loops calls to properly function
        {
            incVar += 'i';
            if(!loopingExp.localVars.ContainsKey(incVar))
                break;
        }

        for(int i = start.value; i <= end.value; i++)
        {
            loopingExp.localVars[incVar] = new IntOperand(i);
            result = result.Add(Evaluator.Evaluate(loopingExp));
        }

        loopingExp.localVars.Remove(incVar);
        return result;
    }
}

class CodedFunction_And : CodedFunction
{
    public CodedFunction_And() : base (
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    ) {}

    public override PrimitiveOperand eval(VarDictionary parms)
    {
        try
        {
            PrimitiveOperand leftResult = Evaluator.Evaluate((ExpressionOperand)parms["!0"]);
            if(((BoolOperand)leftResult).value)
            {
                PrimitiveOperand rightResult = Evaluator.Evaluate((ExpressionOperand)parms["!1"]);
                if(((BoolOperand)rightResult).value)
                    return BoolOperand.TRUE;
                return BoolOperand.FALSE;
            }
            else return BoolOperand.FALSE;
        }
        catch(System.InvalidCastException)
        {
            throw new CodedFunctionException("Both And function parameters must resolve to booleans");
        }
    }
}

class CodedFunction_Or : CodedFunction
{
    public CodedFunction_Or() : base(
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    ) {}

    public override PrimitiveOperand eval(VarDictionary parms)
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
    }
}

class CodedFunction_IntCast : CodedFunction
{
    public CodedFunction_IntCast() : base (
        FxParamType.PRIMITIVE
    ){}

    public override PrimitiveOperand eval(VarDictionary parms)
    {
        switch(parms["!0"])
        {
            case IntOperand p1:
            return p1;

            case DecimalOperand p2:
            return new IntOperand((int)p2.value);

            case BoolOperand p3:
            if(p3.value)
                return new IntOperand(1);
            else
                return new IntOperand(0);
            
            case StringOperand p4:
            int res;
            if(!Int32.TryParse(p4.value, out res))
                throw new CodedFunctionException("This string cannot be converted to an IntOperand");
            return new IntOperand(res);

            default:
            throw new CodedFunctionException("Unreachable");
        }
    }
}

class CodedFunction_DecimalCast : CodedFunction
{
    public CodedFunction_DecimalCast() : base (
        FxParamType.PRIMITIVE
    ){}

    public override PrimitiveOperand eval(VarDictionary parms)
    {
        switch(parms["!0"])
        {
            case IntOperand p1:
            return new DecimalOperand(p1.value);

            case DecimalOperand p2:
            return p2;

            case BoolOperand p3:
            if(p3.value)
                return new DecimalOperand(1);
            else
                return new DecimalOperand(0);
            
            case StringOperand p4:
            double res;
            if(!Double.TryParse(p4.value, out res))
                throw new CodedFunctionException("This string cannot be converted to a DecimalOperand");
            return new DecimalOperand(res);

            default:
            throw new CodedFunctionException("Unreachable");
        }
    }
}

class CodedFunction_BoolCast : CodedFunction
{
    public CodedFunction_BoolCast() : base (
        FxParamType.PRIMITIVE
    ){}

    public override PrimitiveOperand eval(VarDictionary parms)
    {
        switch(parms["!0"])
        {
            case IntOperand p1:
            return BoolOperand.ToOperand(p1.value >= 1);

            case DecimalOperand p2:
            return BoolOperand.ToOperand(p2.value >= 1);

            case BoolOperand p3:
            return p3;

            case StringOperand p4:
            bool res;
            if(!Boolean.TryParse(p4.value, out res))
                throw new CodedFunctionException("This string cannot be converted to a BoolOperand");
            return BoolOperand.ToOperand(res);

            default:
                throw new CodedFunctionException("Unreachable");
        }
    }
}

class CodedFunction_StringCast : CodedFunction
{
    public CodedFunction_StringCast() : base (
        FxParamType.PRIMITIVE
    ){}

    public override PrimitiveOperand eval(VarDictionary parms)
    {
        string val = "";
        switch(parms["!0"])
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
    }
}


