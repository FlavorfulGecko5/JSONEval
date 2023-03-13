namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// An Operand representing a complete expression
/// </summary>
class ExpressionOperand : Operand
{
    /// <summary>
    /// The expression stored by this operand
    /// </summary>
    public string value { get; private set; }

    /// <summary>
    /// The non-global variables accessible by this expression
    /// </summary>
    public VarDictionary localVars;

    /// <summary>
    /// Constructs an ExpressionOperand with 0 predefined non-global variables
    /// </summary>
    /// <param name="vParam">The complete expression</param>
    public ExpressionOperand(string vParam)
    {
        value = vParam;
        localVars = new VarDictionary();
    }

    /// <summary>
    /// Constructs an ExpressionOperand with a predefined set of non-global variables
    /// </summary>
    /// <param name="vParam">The complete expression</param>
    /// <param name="lvParam">The non-global variables accessible to the expression</param>
    public ExpressionOperand(string vParam, VarDictionary lvParam)
    {
        value = vParam;
        localVars = lvParam;
    }

    /*
    * Operand method implementations
    */

    public bool Equals(Operand b)
    {
        switch(b)
        {
            case ExpressionOperand b1:
            return value.Equals(b1.value) && localVars.Equals(b1.localVars);
        }
        return false;
    }
}