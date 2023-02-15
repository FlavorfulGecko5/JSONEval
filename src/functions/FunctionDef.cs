abstract class FunctionDef
{
    public FxParamType[] paramInfo;

    public FunctionDef(params FxParamType[] p_paramInfo)
    {
        paramInfo = p_paramInfo;
    }
}

enum FxParamType
{
    PRIMITIVE,
    EXPRESSION,
    REFERENCE
}