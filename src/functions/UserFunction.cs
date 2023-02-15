class UserFunction : FunctionDef
{
    public string expression;

    public UserFunction(string p_exp, params FxParamType[] p_paramInfo) : base(p_paramInfo)
    {
        expression = p_exp;
    }
}