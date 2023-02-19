abstract class FunctionDef
{
    public FxParamType[] paramInfo;

    public FunctionDef(params FxParamType[] p_paramInfo)
    {
        const string ERR_PARM_COUNT = "All functions need at least 1 parameter";
        if(p_paramInfo.Length == 0)
            throw new FunctionDefinitionException(ERR_PARM_COUNT);
        paramInfo = p_paramInfo;
    }
}

enum FxParamType
{
    PRIMITIVE,
    EXPRESSION,
    REFERENCE
}