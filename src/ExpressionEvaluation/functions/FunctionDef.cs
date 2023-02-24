namespace JSONEval.ExpressionEvaluation;
abstract class FunctionDef
{
    public FxParamType[] paramInfo;

    public FunctionDef(params FxParamType[] p_paramInfo)
    {
        if(p_paramInfo.Length == 0)
            throw new System.ArgumentException("All functions need at least 1 parameter");
        paramInfo = p_paramInfo;
    }
}

enum FxParamType
{
    PRIMITIVE,
    EXPRESSION,
    REFERENCE
}