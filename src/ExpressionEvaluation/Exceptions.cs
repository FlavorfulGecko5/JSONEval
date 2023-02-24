namespace JSONEval.ExpressionEvaluation;
class ExpressionParsingException : Exception
{
    public ExpressionParsingException(string msg) : base(msg) {}
}

class EvaluationException : Exception
{
    public EvaluationException(string msg) : base(msg) {}
}

class CodedFunctionException : Exception
{
    public CodedFunctionException(string msg) : base(msg) {}
}