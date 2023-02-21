class ExpressionParsingException : Exception
{
    public ExpressionParsingException(string msg) : base(msg) {}
}

class EvaluationException : Exception
{
    public EvaluationException(string msg) : base(msg) {}
}

class FunctionDefinitionException : Exception
{
    public FunctionDefinitionException(string msg) : base(msg) {}
}