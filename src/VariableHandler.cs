/*
Naming restrictions:
- At the configuration file level:
    - Letters, underscores only
- At the expression level:
    - Letters, underscores
    - Numbers present (for list indexing), brackets, periods, exclamation marks

Symbols:
- Parentheses ()
- Brackets [] - Function calls
- Braces {} - Loop
- Commas , - parameter delimiters
*/

class VariableHandler : Dictionary<string, Operand>
{
    public VariableHandler() : base(StringComparer.OrdinalIgnoreCase) {}

    public void addIntOperand(string name, int value)
    {
        if(ContainsKey(name))
            throw new Exception("Duplicate variable");
        
        Add(name, new IntOperand(value));
    }

    public void addDecimalOperand(string name, double value)
    {
        if (ContainsKey(name))
            throw new Exception("Duplicate variable");

        Add(name, new DecimalOperand(value));
    }

    public void addBoolOperand(string name, bool value)
    {
        if (ContainsKey(name))
            throw new Exception("Duplicate variable");

        Add(name, new BoolOperand(value));
    }

    public void addExpressionOperand(string name, string value)
    {
        if (ContainsKey(name))
            throw new Exception("Duplicate variable");

        Add(name, new ExpressionOperand(value));
    }

    public void addExpressionOperand(string name, string value, VariableHandler localVars)
    {
        if (ContainsKey(name))
            throw new Exception("Duplicate variable");
        
        Add(name, new ExpressionOperand(value, localVars));
    }
}