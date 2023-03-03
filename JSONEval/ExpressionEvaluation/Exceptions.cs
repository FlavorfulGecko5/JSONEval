namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// Intended to be thrown by the expression parser/evaluator when 
/// an expression cannot be resolved for any predictable reason.
/// </summary>
class ExpressionParsingException : Exception
{
    public ExpressionParsingException(string msg) : base(msg) {}
}

/// <summary>
/// Intended to be thrown by PrimitiveOperands when an operation function
/// cannot return a result.
/// </summary>
class OperatorEvaluationException : Exception
{
    public OperatorEvaluationException(string msg) : base(msg) {}
}

/// <summary>
/// Intended to be thrown by Coded Functions when their evaluation function
/// cannot return a result for any predictable reason.
/// </summary>
class CodedFunctionException : Exception
{
    public CodedFunctionException(string msg) : base(msg) {}
}