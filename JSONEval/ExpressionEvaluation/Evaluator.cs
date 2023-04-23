/*
Copyright (c) 2023 FlavorfulGecko5

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace JSONEval.ExpressionEvaluation;

/// <summary>
/// Performs all steps of the expression evaluation process
/// </summary>
public static class Evaluator
{
    enum TokenType
    {
        NONE,
        OPERATOR,
        OPERAND
    }

    enum OperandTokenType
    {
        NONE,
        INTEGER,
        DECIMAL,
        STRING,
        VARIABLE
    }

    /*
    * Operators are internally represented using single characters.
    * Those operators composed of multiple characters or those with multiple
    * meanings (unary addition/subtraction) have other characters assigned
    * for their internal representation.
    */
    const char SYM_UNARY_ADDITION = '0';
    const char SYM_UNARY_SUBTRACTION = '2';
    const char SYM_NOT_EQUAL = 'y';
    const char SYM_LESS_THAN_EQUAL = '7';
    const char SYM_GREATER_THAN_EQUAL = '9';
    const int PRECEDENCE_UNARY = 5000;
    static readonly Dictionary<char, int> precedence = new Dictionary<char, int>()
    {
        {'~', PRECEDENCE_UNARY}, {SYM_UNARY_ADDITION, PRECEDENCE_UNARY}, {SYM_UNARY_SUBTRACTION, PRECEDENCE_UNARY},
        {'*', 2000}, {'/', 2000}, {'%', 2000},
        {'+', 1000}, {'-', 1000},
        {'<', 700}, {'>', 700}, {SYM_LESS_THAN_EQUAL, 700}, {SYM_GREATER_THAN_EQUAL, 700},
        {'=', 500}, {SYM_NOT_EQUAL, 500},
        {'&', 400},
        {'|', 300},

        {')', -10000}, {']', -10000}
    };

    /// <summary>
    /// All expressions can use the variables stored here
    /// </summary>
    public static VarDictionary globalVars;

    /// <summary>
    /// Info for all available functions (user-defined and hard-coded)
    /// </summary>
    public static FunctionDictionary functions;

    /// <summary>
    /// Use this to load any standard variables and functions
    /// into their respective dictionaries
    /// </summary>
    static Evaluator()
    {
        globalVars = new VarDictionary();
        globalVars.AddBoolVar("true", true);
        globalVars.AddBoolVar("false", false);

        functions = new FunctionDictionary();
        functions.Add("if", PrefabFunctions.IfElse);
        functions.Add("loop", PrefabFunctions.Loop);
        functions.Add("and", PrefabFunctions.And);
        functions.Add("or", PrefabFunctions.Or);
        functions.Add("int", PrefabFunctions.IntCast);
        functions.Add("decimal", PrefabFunctions.DecimalCast);
        functions.Add("bool", PrefabFunctions.BoolCast);
        functions.Add("string", PrefabFunctions.StringCast);
    }

    /// <summary>
    /// Fully evaluates a string expression
    /// </summary>
    /// <param name="exp">The expression to evaluate</param>
    /// <returns>The result stored in the appropriate Operand object</returns>
    /// <exception cref="ExpressionParsingException">
    /// Thrown if the expression cannot be resolved to an Operand for any
    /// predictable reason
    /// </exception>
    public static PrimitiveOperand Evaluate(string exp)
    {
        return Evaluate(new ExpressionOperand(exp));
    }

    /// <summary>
    /// Fully evaluates an expression
    /// </summary>
    /// <param name="exp">The expression to evaluate</param>
    /// <returns>The result stored in an appropriate operand object</returns>
    /// <exception cref="ExpressionParsingException">
    /// Thrown if the expression cannot be resolved to an Operand for any
    /// predictable reason
    /// </exception>
    public static PrimitiveOperand Evaluate(ExpressionOperand exp)
    {
        if(exp.value.Trim().Length == 0)
            throw SyntaxError(exp.value.Length, "The expression is empty.");

        Stack<PrimitiveOperand> operands = new Stack<PrimitiveOperand>();
        Stack<char> operators = new Stack<char>();

        /*
        * Used to ensure proper parentheses/bracket balance
        * May contain three symbols:
        * e - empty stack
        * ) - parentheses (symbols chosen for code reuse)
        * ] - bracket
        */
        Stack<char> balanceChecker = new Stack<char>();
        balanceChecker.Push('e');

        string activeOperand = "";
        OperandTokenType activeType = OperandTokenType.NONE;
        TokenType lastToken = TokenType.NONE;

        int inc;
        for(inc = 0; inc < exp.value.Length; inc++)
        {   
            char c = exp.value[inc];

            if(Char.IsWhiteSpace(c))
                TryPushOperand();
            else if((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
            {
                switch(activeType)
                {
                    case OperandTokenType.NONE:
                    activeType = OperandTokenType.VARIABLE;
                    break;

                    case OperandTokenType.VARIABLE: break;

                    default:
                    throw SyntaxError(inc, "Improper placement of a letter or underscore.");
                }
                activeOperand += c;
            }
            else switch(c)
            {
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                if(activeType == OperandTokenType.NONE)
                    activeType = OperandTokenType.INTEGER;
                activeOperand += c; 
                break;

                case '.':
                switch(activeType)
                {
                    case OperandTokenType.DECIMAL:
                    throw SyntaxError(inc, "A period was already placed in this decimal value");

                    case OperandTokenType.NONE: case OperandTokenType.INTEGER:
                    activeType = OperandTokenType.DECIMAL;
                    break;
                }
                activeOperand += c;
                break;

                case '!':
                if(activeType != OperandTokenType.NONE)
                    throw SyntaxError(inc, "Exclamation marks may only be at the start of a variable name");
                activeType = OperandTokenType.VARIABLE;
                activeOperand += c;
                break;

                // Reads and parses the entire string literal in one iteration of the for loop
                case '\'':
                if(activeType != OperandTokenType.NONE)
                    throw SyntaxError(inc, "Can't transition to string while parsing another operand.");
                activeType = OperandTokenType.STRING;

                int stringEndIndex = exp.value.IndexOf('\'', inc + 1);
                if(stringEndIndex == -1)
                    throw SyntaxError(exp.value.Length - 1, "String literal has no end-quote");
                
                for(int j = inc+1; j < stringEndIndex; j++)
                    if(exp.value[j] == '`')
                        switch(exp.value[j++ + 1])
                        {
                            case '`': activeOperand += '`'; break;
                            case 'q': activeOperand += '\''; break;
                            case 'n':  activeOperand += '\n'; break;
                            case 't':  activeOperand += '\t'; break;
                            default: throw SyntaxError(j, "Unrecognized escape sequence");
                        }
                    else
                        activeOperand += exp.value[j];
                TryPushOperand();
                inc = stringEndIndex;
                break;

                case '+': case '-':
                TryPushOperand();
                if(lastToken != TokenType.OPERAND)
                    c += (char)5; // Byte-value shifting to eliminate redundant
                goto LABEL_EVAL;  // case blocks

                case '~': case '<': case '>':
                TryPushOperand();
                if(inc != exp.value.Length - 1 && exp.value[inc+1] == '=')
                    c = (char)(exp.value[inc++] - 5);
                goto LABEL_EVAL;

                case '*': case '/': case '%': 
                case '=': case '&': case '|': 
                TryPushOperand();
                LABEL_EVAL:
                if(precedence[c] == PRECEDENCE_UNARY)
                {
                    if (lastToken == TokenType.OPERAND) // Already verified for unary add/sub
                        throw SyntaxError(inc, "Unary operators cannot be placed after an operand.");
                }
                else // Ensures right-left evaluation for unary operators
                {
                    if (lastToken != TokenType.OPERAND)
                        throw SyntaxError(inc, "Binary operators must be placed after an operand.");
                    while(operators.Count > 0) // <= For left-right evaluation, < for right-left
                        if((precedence[c] <= precedence[operators.Peek()]))
                            eval();
                        else break;
                }
                operators.Push(c);
                lastToken = TokenType.OPERATOR;
                break;

                case '(':
                switch(activeType)
                {
                    case OperandTokenType.VARIABLE:
                    functionHandler(); 
                    break;

                    case OperandTokenType.NONE:
                    if(lastToken == TokenType.OPERAND)
                        throw SyntaxError(inc, "Cannot place opening-parentheses after an operand");
                    balanceChecker.Push(')');
                    operators.Push(')');
                    lastToken = TokenType.OPERATOR;
                    break;

                    default:
                        throw SyntaxError(inc, "Improper placement of opening-parentheses");
                }
                break;

                case '[':
                if(activeType != OperandTokenType.VARIABLE)
                    throw SyntaxError(inc, "Brackets may only be used as part of variable names.");
                balanceChecker.Push(']');
                activeType = OperandTokenType.STRING;
                TryPushOperand();  // The current name will be popped from
                operators.Push(']'); // the stack after resolving the bracket contents
                lastToken = TokenType.OPERATOR;
                break;

                case ')': case ']':
                TryPushOperand();
                if(lastToken != TokenType.OPERAND)
                    throw SyntaxError(inc, "Closing symbols must be placed after operands.");
                if(balanceChecker.Peek() != c)
                    throw SyntaxError(inc, "This closing symbol lacks a properly placed opening symbol");
                balanceChecker.Pop();
                while(operators.Peek() != c)
                    eval();
                operators.Pop();
                
                if(c == ')')
                    lastToken = TokenType.OPERAND; // Parentheses contents resolved to single operand
                else
                {
                    if(!(operands.Peek() is IntOperand))
                        throw SyntaxError(inc, "Bracketed expression must resolve to an integer!");
                    IntOperand b1 = (IntOperand)operands.Pop();
                    StringOperand priorName = (StringOperand)operands.Pop();
                    activeOperand = priorName.value + '[' + b1.value + ']';
                    activeType = OperandTokenType.VARIABLE;
                    lastToken = TokenType.OPERATOR; // No operand can follow another operand
                }    
                break;

                default:
                throw SyntaxError(inc, "Invalid usage of unregistered character '" + exp.value[inc] + "'");
            }
        }
        TryPushOperand();
        if(lastToken == TokenType.OPERATOR)
            throw SyntaxError(exp.value.Length - 1, "Cannot end an expression with an operator");
        if(balanceChecker.Peek() != 'e')
            throw SyntaxError(exp.value.Length - 1, "Parentheses and brackets are not properly closed");
        while(operators.Count > 0)
            eval();
        
        return operands.Pop();

        void eval()
        {
            try {
            char op = operators.Pop();
            PrimitiveOperand rightHand = operands.Pop();

            if(precedence[op] == PRECEDENCE_UNARY)
            {
                switch(op)
                {
                    case '~':
                        operands.Push(rightHand.Not());
                    break;

                    case SYM_UNARY_ADDITION:
                        operands.Push(rightHand.UnaryAdd());
                    break;

                    case SYM_UNARY_SUBTRACTION:
                        operands.Push(rightHand.UnarySub());
                    break;
                }
                return;
            }

            PrimitiveOperand leftHand = operands.Pop();
            switch(op)
            {
                case '*':
                    operands.Push(leftHand.Mult(rightHand));
                break;

                case '/':
                    operands.Push(leftHand.Div(rightHand));
                break;

                case '%':
                    operands.Push(leftHand.Rem(rightHand));
                break;

                case '+':
                    operands.Push(leftHand.Add(rightHand));
                break;

                case '-':
                    operands.Push(leftHand.Sub(rightHand));
                break;

                case '=':
                    operands.Push(leftHand.Equal(rightHand));
                break;

                case SYM_NOT_EQUAL:
                    operands.Push(leftHand.NotEqual(rightHand));
                break;

                case '&':
                    operands.Push(leftHand.And(rightHand));
                break;

                case '|':
                    operands.Push(leftHand.Or(rightHand));
                break;

                case '<':
                    operands.Push(leftHand.LessThan(rightHand));
                break;

                case SYM_LESS_THAN_EQUAL:
                    operands.Push(leftHand.LessThanEqual(rightHand));
                break;

                case '>':
                    operands.Push(leftHand.GreaterThan(rightHand));
                break;

                case SYM_GREATER_THAN_EQUAL:
                    operands.Push(leftHand.GreaterThanEqual(rightHand));
                break;
            }}
            catch(OperatorEvaluationException e)
            {
                throw SyntaxError(inc, e.Message);
            }
        }
       
        void TryPushOperand()
        {          
            // We might not have an operand to push
            if(activeType == OperandTokenType.NONE)
                return;
            
            if(lastToken == TokenType.OPERAND)
                throw SyntaxError(inc, "Cannot have two operands in a row - place an operator between them.");

            switch(activeType)
            {
                case OperandTokenType.INTEGER:
                    int tp1;
                    if(!Int32.TryParse(activeOperand, out tp1))
                        throw SyntaxError(inc, "Could not convert integer literal from a string to it's real value");
                    operands.Push(new IntOperand(tp1));
                break;

                case OperandTokenType.DECIMAL:
                    double tp2;
                    if(!Double.TryParse(activeOperand, out tp2))
                        throw SyntaxError(inc, "Could not convert decimal literal from a string to it's real value");
                    operands.Push(new DecimalOperand(tp2));
                break;

                case OperandTokenType.STRING:
                    operands.Push(new StringOperand(activeOperand));
                break;

                case OperandTokenType.VARIABLE:
                    bool isGlobal = globalVars.ContainsKey(activeOperand);
                    bool isLocal = exp.localVars.ContainsKey(activeOperand);

                    if(!isGlobal && !isLocal)
                        throw SyntaxError(inc, "Could not find variable '" + activeOperand + "'");
                    
                    Operand varValue = isLocal ? exp.localVars[activeOperand] : globalVars[activeOperand];

                    if(varValue is ExpressionOperand)
                        operands.Push(recursiveCall( (ExpressionOperand)varValue ));
                    else operands.Push( (PrimitiveOperand)varValue );
                break;
            }

            // Ensure type trackers are ready for next token
            activeOperand = "";
            lastToken = TokenType.OPERAND;
            activeType = OperandTokenType.NONE;
        }

        PrimitiveOperand recursiveCall(ExpressionOperand r)
        {
            try { return Evaluate(r);}
            catch(ExpressionParsingException e)
            {
                string fragment = exp.value.Substring(0, 
                    inc == exp.value.Length ? inc - 1 : inc);
                string appendMsg = String.Format("\n> \"{0}\"", fragment);
                throw new ExpressionParsingException(e.Message + appendMsg);
            }
        }

        void functionHandler()
        {
            if(!functions.ContainsKey(activeOperand))
                throw SyntaxError(inc, "Function named '" + activeOperand + "' is not defined");

            // STEP 1: EXTRACT THE RAW PARAMETERS
            // Track parentheses/non-escaped string chars to ensure we locate the right commas
            FxParamType[] parmData = functions[activeOperand].paramInfo;
            string[] rawParms = new string[parmData.Length];
            int closeIndex = inc + 1; // Gets iterated on until the close parentheses is reached

            for(int i = 0; i < rawParms.Length; i++)
            {
                int paramStartIndex = closeIndex;
                char delimiter = i == rawParms.Length - 1 ? ')' : ',';
                bool foundParamEndIndex = false;

                /*
                * To extract parameters correctly we must ignore extra delimiters
                * Stack has three symbols:
                * e - empty stack
                * s - string
                * p - parentheses
                */
                Stack<char> extraDelimiters = new Stack<char>();
                extraDelimiters.Push('e'); 
                while(!foundParamEndIndex && closeIndex < exp.value.Length)
                {
                    switch(exp.value[closeIndex])
                    {
                        case '(':
                        if(extraDelimiters.Peek() != 's')
                            extraDelimiters.Push('p');
                        break;

                        case ')':
                        switch(extraDelimiters.Peek())
                        {
                            case 'p': extraDelimiters.Pop(); break;

                            case 'e':
                            if(delimiter == ')')
                                foundParamEndIndex = true;
                            else
                                throw SyntaxError(closeIndex, "Too few arguments received for function call");
                            break;
                        }
                        break;

                        case '\'':
                        if(extraDelimiters.Peek() == 's')
                            extraDelimiters.Pop();
                        else
                            extraDelimiters.Push('s');
                        break;

                        case ',':
                        if(extraDelimiters.Peek() == 'e')
                        {
                            if(delimiter == ',')
                                foundParamEndIndex = true;
                            else
                                throw SyntaxError(closeIndex, "Too many arguments received for function call");
                        }
                        break;
                    }
                    closeIndex++;
                }
                if(!foundParamEndIndex)
                    throw SyntaxError(exp.value.Length - 1, "Failed to find parameter #" + (i+1) + " for function call.");
                rawParms[i] = exp.value.Substring(paramStartIndex, closeIndex - paramStartIndex - 1);
            }
            closeIndex--; // Fixup the final index to match the close parentheses
            inc = closeIndex; // Do this here for more accurate error messages

            // STEP 2: BUILD LOCAL CALL PARMS
            VarDictionary callVariables = new VarDictionary();

            for(int i = 0; i < rawParms.Length; i++)
            {
                ExpressionOperand parmExp = new ExpressionOperand(rawParms[i], exp.localVars);
                switch(parmData[i])
                {
                    case FxParamType.PRIMITIVE:
                        callVariables.Add("!" + i, recursiveCall(parmExp));
                    break;

                    case FxParamType.EXPRESSION:
                        callVariables.Add("!" + i, parmExp);
                    break;

                    case FxParamType.REFERENCE:
                        PrimitiveOperand parmExpResult = recursiveCall(parmExp);
                        if(!(parmExpResult is StringOperand))
                            throw SyntaxError(closeIndex, "Reference parameter #" + (i + 1) + " must evaluate to a string");
                        string refName = parmExpResult.GetValueString();

                        bool isLocal = exp.localVars.ContainsKey(refName);
                        bool isGlobal = globalVars.ContainsKey(refName);

                        if(!isLocal && !isGlobal)
                            throw SyntaxError(closeIndex, "Reference parameter '" + refName + "' is not defined as a variable.");
                        
                        copyVars(globalVars);
                        copyVars(exp.localVars); // Local vars will shadow global vars
                        void copyVars(VarDictionary v)
                        {
                            if(v.ContainsKey(refName))
                                callVariables["!" + i] = v[refName];
                            foreach(string key in v.Keys)
                                if(key.StartsWith(refName, StringComparison.OrdinalIgnoreCase))
                                {
                                    string extension = key.Substring(refName.Length);
                                    if(extension.Length > 0 && (extension[0] == '.' || extension[0] == '['))
                                        callVariables["!" + i + extension] = v[key];
                                }
                        }
                    break;
                }
            }

            // STEP 3: EXECUTE THE FUNCTION CALL
            switch(functions[activeOperand])
            {
                case ExpressionFunction f1:
                    ExpressionOperand toPrim = new ExpressionOperand(f1.expression, callVariables);
                    operands.Push(recursiveCall(toPrim));
                break;

                case CodedFunction f2:
                    try {operands.Push(f2.def.Invoke(callVariables)); }
                    catch(CodedFunctionException e)
                    {
                        throw SyntaxError(closeIndex, e.Message);
                    }
                    catch(ExpressionParsingException e)
                    {
                        string fragment = exp.value.Substring(0, closeIndex + 1);
                        string appendMsg = String.Format("\n> \"{0}\"", fragment);
                        throw new ExpressionParsingException(e.Message + appendMsg);  
                    }
                break;
            }
            activeOperand = "";
            activeType = OperandTokenType.NONE;
            lastToken = TokenType.OPERAND;
        }

        ExpressionParsingException SyntaxError(int badCharIndex, string desc)
        {
            if(badCharIndex >= exp.value.Length) // Convenient catch-all
                badCharIndex = exp.value.Length - 1;
            string expFragment = exp.value.Substring(0, badCharIndex + 1);
            string fullMessage = String.Format(
                "{0}\nEvaluation Call Trace: "
                + "\n> \"{1}\" <--- Error found while parsing last character - prior characters may be responsible", desc, expFragment);
            return new ExpressionParsingException(fullMessage);
        }
    }
}