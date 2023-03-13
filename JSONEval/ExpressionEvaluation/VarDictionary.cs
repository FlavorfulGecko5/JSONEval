namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// Stores expression variable names and values
/// </summary>
class VarDictionary : Dictionary<string, Operand>
{
    /// <summary>
    /// Variable Dictionary Constructor
    /// </summary>
    /// <returns> Variable Dictionary with case-insensitive variable name lookups</returns>
    public VarDictionary() : base(StringComparer.OrdinalIgnoreCase) {}

    /// <summary>
    /// Adds data for an integer variable to this dictionary
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The integer value</param>
    /// <exception cref="System.ArgumentException">
    /// A variable with the same name already exists in this dictionary
    /// </exception>
    public void AddIntVar(string name, int value)
    {
        Add(name, new IntOperand(value));
    }

    /// <summary>
    /// Adds data for a decimal (floating-point) variable to this dictionary
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The decimal value</param>
    /// <exception cref="System.ArgumentException">
    /// A variable with the same name already exists in this dictionary
    /// </exception>
    public void AddDecimalVar(string name, double value)
    {
        Add(name, new DecimalOperand(value));
    }

    /// <summary>
    /// Adds data for a Boolean variable to this dictionary
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The Boolean value</param>
    /// <exception cref="System.ArgumentException">
    /// A variable with the same name already exists in this dictionary
    /// </exception>
    public void AddBoolVar(string name, bool value)
    {
        Add(name, BoolOperand.ToOperand(value));
    }

    /// <summary>
    /// Adds data for a string variable to this dictionary
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The string value</param>
    /// <exception cref="System.ArgumentException">
    /// A variable with the same name already exists in this dictionary
    /// </exception>
    public void AddStringVar(string name, string value)
    {
        Add(name, new StringOperand(value));
    }

    /// <summary>
    /// Adds data for a variable representing a complete expression
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The expression</param>
    /// <exception cref="System.ArgumentException">
    /// A variable with the same name already exists in this dictionary
    /// </exception>
    public void AddExpressionVar(string name, string value)
    {
        Add(name, new ExpressionOperand(value));
    }

    /// <summary>
    /// Adds data for a variable representing a complete expression
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The expression</param>
    /// <param name="localVars">The non-global variables this expression can utilize.</param>
    /// <exception cref="System.ArgumentException">
    /// A variable with the same name already exists in this dictionary
    /// </exception>
    public void AddExpressionVar(string name, string value, VarDictionary localVars)
    {
        Add(name, new ExpressionOperand(value, localVars));
    }

    /// <summary>
    /// Checks if two VarDictionaries have the same contents
    /// </summary>
    /// <param name="v">The dictionary to compare with</param>
    /// <returns>True if the contents are equal, otherwise false</returns>
    public bool Equals(VarDictionary v)
    {
        if(Count != v.Count)
            return false;
        
        foreach(string key in Keys)
            if(!v.ContainsKey(key) || !this[key].Equals(v[key]))
                return false;
        return true;
    }
}