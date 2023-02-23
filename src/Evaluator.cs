class Evaluator
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

        {'(', -10000}, {'[', -10000}
    };

    /// <summary>
    /// All expressions can use the variables stored here
    /// </summary>
    public static VariableHandler globalVars;

    /// <summary>
    /// Info for all available functions (user-defined and hard-coded)
    /// </summary>
    public static FunctionHandler functions;

    /// <summary>
    /// Use this to load any standard variables and functions
    /// into their respective dictionaries
    /// </summary>
    static Evaluator()
    {
        globalVars = new VariableHandler();
        globalVars.addBoolOperand("true", true);
        globalVars.addBoolOperand("false", false);

        functions = new FunctionHandler();
        functions.Add("if", new CodedFunction_IfElse());
        functions.Add("and", new CodedFunction_And());
        functions.Add("or", new CodedFunction_Or());
        functions.Add("int", new CodedFunction_IntCast());
    }

    /// <summary>
    /// Fully evaluates a string expression
    /// </summary>
    /// <param name="exp">The expression to evaluate</param>
    /// <returns>The string representation of the result</returns>
    public string evaluate(string exp)
    {
        PrimitiveOperand result = evaluate(new ExpressionOperand(exp));
        return result.ToString();
    }

    /// <summary>
    /// Fully evaluates an expression
    /// </summary>
    /// <param name="exp">The expression to evaluate</param>
    /// <param name="flag_reference">If true, treat the expresion as a reference parameter</param>
    /// <returns>The result stored in an appropriate operand object</returns>
    public PrimitiveOperand evaluate(ExpressionOperand exp, bool flag_reference=false)
    {
        if(exp.value.Trim().Length == 0)
            throw SyntaxError(exp.value.Length, "The expression is empty.");

        Stack<PrimitiveOperand> operands = new Stack<PrimitiveOperand>();
        Stack<char> operators = new Stack<char>();

        /*
        * Used to ensure proper parentheses/bracket balance
        * May contain three symbols:
        * e - empty stack
        * p - parentheses
        * b - bracket
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
            {
                TryPushOperand();
                continue;
            }

            if((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
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
                continue;
            }
                
            switch(c)
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

                    case OperandTokenType.VARIABLE:
                    if(exp.value[inc-1] == '.')
                        throw SyntaxError(inc, "Variable names cannot contain two consecutive periods."); 
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
                if(activeType > OperandTokenType.NONE)
                    throw SyntaxError(inc, "Can't transition to string while parsing another operand.");
                activeType = OperandTokenType.STRING;

                int stringEndIndex = -1;
                for(int j = inc+1; j < exp.value.Length; j++)
                    if(exp.value[j] == '\'' && exp.value[j-1] != '`')
                    {
                        stringEndIndex = j;
                        break;
                    }
                if(stringEndIndex == -1)
                    throw SyntaxError(exp.value.Length - 1, "String literal has no end-quote");
                
                // At this point we know the start and end index of the string,
                // and that the index before the end quote does not have a `
                for(int j = inc+1; j < stringEndIndex; j++)
                    switch(exp.value[j])
                    {
                        // Might want to revise escape sequence system in the future,
                        // considering the revelation that System.Data did not have escape sequences
                        case '`':
                            if(exp.value[j+1] == '\'')
                            {
                                activeOperand += '\'';
                                j++;
                            }          
                        break;

                        case '\\':
                        switch(exp.value[j+1]) // This can include the end quote if before it
                        {
                            // All JSON-supported escape sequences
                            case '\\': activeOperand += '\\'; break;
                            case 'n': activeOperand += '\n'; break;
                            case 't': activeOperand += '\t'; break;
                            case 'b': activeOperand += '\b'; break;
                            case 'r': activeOperand += '\r'; break;
                            case 'f': activeOperand += '\f'; break;
                            
                            default:
                                throw SyntaxError(j + 1, "Invalid escape sequence inside string literal.");
                        }
                        j++;
                        break;

                        default:
                        activeOperand += exp.value[j];
                        break;
                    }
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
                {
                    c -= (char)5;
                    inc++;
                }
                goto LABEL_EVAL;

                case '*': case '/': case '%': 
                case '=': case '&': case '|': 
                TryPushOperand();
                LABEL_EVAL:
                switch(c)
                {
                    // Technically this has already been verified for unary add/sub
                    case '~': case SYM_UNARY_ADDITION: case SYM_UNARY_SUBTRACTION:
                    if(lastToken == TokenType.OPERAND)
                        throw SyntaxError(inc, "Unary operators cannot be placed after an operand.");
                    break;

                    default:
                    if(lastToken != TokenType.OPERAND)
                        throw SyntaxError(inc, "Binary operators must be placed after an operand.");
                    break;

                }
                while(operators.Count > 0)
                {
                    // <= instead of < Causes left-right evaluation instead of right-left
                    // For unary operations - we need to ensure right-left evaluation
                    // This current setup relies on unary operators having the highest-precedence (other than sub-expressions)
                    if((precedence[c] <= precedence[operators.Peek()]) && precedence[c] != PRECEDENCE_UNARY)
                        eval();
                    else
                        break;
                }
                operators.Push(c);
                lastToken = TokenType.OPERATOR;
                break;

                case '(':
                if (flag_reference && balanceChecker.Peek() == 'e')
                    throw SyntaxError(inc, "Cannot pass a function call or expression as a reference parameter");
                switch(activeType)
                {
                    case OperandTokenType.VARIABLE:
                    functionHandler(); 
                    break;

                    case OperandTokenType.NONE:
                    if(lastToken == TokenType.OPERAND)
                        throw SyntaxError(inc, "Cannot place opening-parentheses after an operand");
                    balanceChecker.Push('p');
                    operators.Push(c);
                    lastToken = TokenType.OPERATOR;
                    break;

                    default:
                        throw SyntaxError(inc, "Improper placement of opening-parentheses");
                }
                break;

                case ')':
                TryPushOperand();
                if (lastToken != TokenType.OPERAND)
                    throw SyntaxError(inc, "A closing-parentheses must be placed after an operand");
                if(balanceChecker.Peek() != 'p')
                    throw SyntaxError(inc, "This closing-parentheses lacks a properly placed opening-parentheses");
                balanceChecker.Pop();
                while(operators.Peek() != '(')
                    eval();
                operators.Pop();               // At this point, the contents of the parentheses
                lastToken = TokenType.OPERAND; // Have been resolved to a single operand
                break;

                case '[':
                if(activeType != OperandTokenType.VARIABLE)
                    throw SyntaxError(inc, "Brackets may only be used as part of variable names.");
                balanceChecker.Push('b');
                activeType = OperandTokenType.STRING;
                TryPushOperand();  // The current name will be popped from
                operators.Push(c); // the stack after resolving the bracket contents
                lastToken = TokenType.OPERATOR;
                break;

                case ']':
                TryPushOperand();
                if(lastToken != TokenType.OPERAND)
                    throw SyntaxError(inc, "A closing-bracket must be placed after an operand");
                if(balanceChecker.Peek() != 'b')
                    throw SyntaxError(inc, "This closing-bracket lacks a properly placed opening-bracket");
                balanceChecker.Pop();
                while(operators.Peek() != '[')
                    eval();
                operators.Pop();
                switch(operands.Pop())
                {
                    case IntOperand b1:
                    StringOperand priorName = (StringOperand)operands.Pop();
                    activeOperand = priorName.value + '[' + b1.value + ']';
                    break;

                    default:
                    throw SyntaxError(inc, "Bracketed expression must resolve to an integer!");
                }
                activeType = OperandTokenType.VARIABLE;
                lastToken = TokenType.OPERATOR; // No operand should follow another operand
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
            catch(EvaluationException e)
            {
                throw SyntaxError(inc == exp.value.Length ? inc - 1 : inc, e.Message);
            }
        }
       
        void TryPushOperand()
        {          
            // We might not have an operand to push
            if(activeType == OperandTokenType.NONE)
                return;
            
            if(lastToken == TokenType.OPERAND)
                throw SyntaxError(inc, "Cannot have two operands in a row - place an operator between them.");

            if (flag_reference && balanceChecker.Peek() == 'e')
            {
                if(activeType != OperandTokenType.VARIABLE)
                    throw SyntaxError(inc, "A reference parameter cannot be a literal value");
                if(operators.Count > 0 || operands.Count > 0)
                    throw SyntaxError(inc, "A reference parameter may only have bracketed expressions");
                for(int k = inc; k < exp.value.Length; k++)
                    if(!Char.IsWhiteSpace(exp.value[k]))
                        throw SyntaxError(k, "A reference parameter must be a single variable name");
                operands.Push(new StringOperand(activeOperand));
            }
            else switch(activeType)
            {
                case OperandTokenType.INTEGER:
                    operands.Push(new IntOperand(activeOperand));
                break;

                case OperandTokenType.DECIMAL:
                    operands.Push(new DecimalOperand(activeOperand));
                break;

                case OperandTokenType.STRING:
                    operands.Push(new StringOperand(activeOperand));
                break;

                case OperandTokenType.VARIABLE:
                    bool isGlobal = globalVars.ContainsKey(activeOperand);
                    bool isLocal = exp.localVars.ContainsKey(activeOperand);

                    // Checking for variable presence in both dictionaries shouldn't be necessary
                    // if the rest of the code is put together properly
                    if(!isGlobal && !isLocal)
                        throw SyntaxError(inc, "Could not find variable '" + activeOperand + "'");
                    
                    Operand varValue = isLocal ? exp.localVars[activeOperand] : globalVars[activeOperand];

                    switch(varValue)
                    {
                        case PrimitiveOperand v1:
                        operands.Push(v1);
                        break;

                        case ExpressionOperand v2:
                        operands.Push(recursiveCall(v2));
                        break;
                    }
                break;
            }

            // Ensure type trackers are ready for next token
            activeOperand = "";
            lastToken = TokenType.OPERAND;
            activeType = OperandTokenType.NONE;
        }

        PrimitiveOperand recursiveCall(ExpressionOperand r, bool setRefFlag=false)
        {
            try { return evaluate(r, setRefFlag);}
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
                            case 's': break;

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
                        switch(extraDelimiters.Peek())
                        {
                            case 'p': case 'e':
                            extraDelimiters.Push('s');
                            break;

                            case 's':
                            if(exp.value[closeIndex - 1] != '`') // Accounts for escape sequence
                                extraDelimiters.Pop();
                            break;
                        }
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
            VariableHandler callVariables = new VariableHandler();

            for(int i = 0; i < rawParms.Length; i++)
            {
                switch(parmData[i])
                {
                    case FxParamType.PRIMITIVE:
                        ExpressionOperand toPrim = new ExpressionOperand(rawParms[i], exp.localVars);
                        callVariables.Add("!" + i, recursiveCall(toPrim));
                    break;

                    case FxParamType.EXPRESSION:
                        callVariables.addExpressionOperand("!" + i, rawParms[i], exp.localVars);
                    break;

                    case FxParamType.REFERENCE: // Allow for evaluation of bracket contents before checking for existence
                        ExpressionOperand toRef = new ExpressionOperand(rawParms[i], exp.localVars);
                        StringOperand refVarName = (StringOperand)recursiveCall(toRef, true);

                        bool isLocal = exp.localVars.ContainsKey(refVarName.value);
                        bool isGlobal = globalVars.ContainsKey(refVarName.value);

                        if(!isLocal && !isGlobal)
                            throw SyntaxError(closeIndex, "Reference parameter '" + refVarName.value + "' is not defined as a variable.");
                        
                        copyVars(globalVars);
                        copyVars(exp.localVars); // Local vars will shadow global vars
                        void copyVars(VariableHandler v)
                        {
                            foreach(string key in v.Keys)
                                if(key.StartsWith(refVarName.value, StringComparison.OrdinalIgnoreCase))
                                {
                                    string extension = key.Substring(refVarName.value.Length);
                                    callVariables["!" + i + extension] = v[key];
                                }
                        }
                    break;
                }
            }

            // STEP 3: EXECUTE THE FUNCTION CALL
            switch(functions[activeOperand])
            {
                case UserFunction f1:
                    ExpressionOperand toPrim = new ExpressionOperand(f1.expression, callVariables);
                    operands.Push(recursiveCall(toPrim));
                break;

                case CodedFunction f2:
                    try {operands.Push(f2.eval(this, callVariables)); }
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
            if(badCharIndex >= exp.value.Length)
                badCharIndex = exp.value.Length - 1;
            string expFragment = exp.value.Substring(0, badCharIndex + 1);
            string fullMessage = String.Format(
                "{0}\nEvaluation Call Trace: "
                + "\n> \"{1}\" <--- Error found while parsing last character - prior characters may be responsible", desc, expFragment);
            return new ExpressionParsingException(fullMessage);
        }
    }
}