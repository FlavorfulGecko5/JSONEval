namespace JSONEval.ExpressionEvaluation;
class VariableHandler : Dictionary<string, Operand>
{
    public VariableHandler() : base(StringComparer.OrdinalIgnoreCase) {}

    private void checkDuplicates(string name)
    {
        if(ContainsKey(name))
            throw new System.ArgumentException("This variable dictionary already contains a definition for '" + name + "'");
    }

    public void addIntOperand(string name, int value)
    {
        checkDuplicates(name);
        Add(name, new IntOperand(value));
    }

    public void addDecimalOperand(string name, double value)
    {
        checkDuplicates(name);
        Add(name, new DecimalOperand(value));
    }

    public void addBoolOperand(string name, bool value)
    {
        checkDuplicates(name);
        Add(name, BoolOperand.ToOperand(value));
    }

    public void addExpressionOperand(string name, string value)
    {
        checkDuplicates(name);
        Add(name, new ExpressionOperand(value));
    }

    public void addExpressionOperand(string name, string value, VariableHandler localVars)
    {
        checkDuplicates(name); 
        Add(name, new ExpressionOperand(value, localVars));
    }
}