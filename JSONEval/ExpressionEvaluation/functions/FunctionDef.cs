namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// Base class for defining functions useable in expressions
/// </summary>
abstract class FunctionDef
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
enum FxParamType
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