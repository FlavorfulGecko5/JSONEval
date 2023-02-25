namespace JSONEval.ExpressionEvaluation;
/// <summary>
/// Function defined as an expression
/// </summary>
class UserFunction : FunctionDef
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
    public UserFunction(string p_exp, params FxParamType[] p_paramInfo) : base(p_paramInfo)
    {
        expression = p_exp;
    }
}