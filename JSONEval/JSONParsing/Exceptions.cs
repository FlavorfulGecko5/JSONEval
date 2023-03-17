namespace JSONEval.JSONParsing;

/// <summary>
/// Intended to be thrown by the JSON Parser when parsing fails for any
/// predictable reason
/// </summary>
class ParserException : Exception
{
    public ParserException(string msg) : base(msg) { }
}