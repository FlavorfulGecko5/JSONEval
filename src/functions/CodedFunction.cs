abstract class CodedFunction : FunctionDef
{
    public CodedFunction(params FxParamType[] p_paramInfo) : base(p_paramInfo) {}

    // By the time the coded function call executes, we know:
    // - That the number of parameters is correct
    // - That the parameters are of the correct FxParamType
    // What we don't know is:
    // - If Primitive Parameters are of the expected type
    public abstract PrimitiveOperand eval(Parser p, VariableHandler parms);
}

class CodedFunction_IfElse : CodedFunction
{
    public CodedFunction_IfElse() : base(
        FxParamType.PRIMITIVE, 
        FxParamType.EXPRESSION, 
        FxParamType.EXPRESSION
    ) {}

    public override PrimitiveOperand eval(Parser p, VariableHandler parms)
    {
        switch(parms["!0"])
        {
            case BoolOperand conditionalResult:
                if(conditionalResult.value)
                    return p.evaluate((ExpressionOperand)parms["!1"]);
                else
                    return p.evaluate((ExpressionOperand)parms["!2"]);
            default:
                throw new Exception("The first parameter of an If function must resolve to a Boolean");
        }
    }
}

class CodedFunction_And : CodedFunction
{
    public CodedFunction_And() : base (
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    ) {}

    public override PrimitiveOperand eval(Parser p, VariableHandler parms)
    {
        try
        {
            PrimitiveOperand leftResult = p.evaluate((ExpressionOperand)parms["!0"]);
            if(((BoolOperand)leftResult).value)
            {
                PrimitiveOperand rightResult = p.evaluate((ExpressionOperand)parms["!1"]);
                if(((BoolOperand)rightResult).value)
                    return BoolOperand.TRUE;
                return BoolOperand.FALSE;
            }
            else return BoolOperand.FALSE;
        }
        catch(System.InvalidCastException)
        {
            throw new Exception("Both And function parameters must resolve to booleans");
        }
    }
}

class CodedFunction_Or : CodedFunction
{
    public CodedFunction_Or() : base(
        FxParamType.EXPRESSION,
        FxParamType.EXPRESSION
    ) {}

    public override PrimitiveOperand eval(Parser p, VariableHandler parms)
    {
        try
        {
            PrimitiveOperand leftResult = p.evaluate((ExpressionOperand)parms["!0"]);
            if (((BoolOperand)leftResult).value)
                return BoolOperand.TRUE;

            PrimitiveOperand rightResult = p.evaluate((ExpressionOperand)parms["!1"]);
            if (((BoolOperand)rightResult).value)
                return BoolOperand.TRUE;
            
            return BoolOperand.FALSE;
        }
        catch (System.InvalidCastException)
        {
            throw new Exception("Both Or function parameters must resolve to booleans");
        }
    }
}

class CodedFunction_IntCast : CodedFunction
{
    public CodedFunction_IntCast() : base (
        FxParamType.PRIMITIVE
    ){}

    public override PrimitiveOperand eval(Parser p, VariableHandler parms)
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
            
            default:
            throw new Exception("Cannot convert this type to an IntOperand");
        }
    }
}

