﻿class ExpressionSolver
{
    /*
    * Operators are internally represented using single characters.
    * Those operators composed of multiple characters or those with multiple
    * meanings (unary addition/subtraction) have other characters assigned
    * for their internal representation.
    */
    const char SYM_UNARY_ADDITION = 'a';
    const char SYM_UNARY_SUBTRACTION = 's';
    const char SYM_NOT_EQUAL = 'e';
    const char SYM_LESS_THAN_EQUAL = 'l';
    const char SYM_GREATER_THAN_EQUAL = 'g';
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
    static ExpressionSolver()
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
    /// <returns>The result stored in an appropriate operand object</returns>
    public PrimitiveOperand evaluate(ExpressionOperand exp)
    {
        Stack<PrimitiveOperand> operands = new Stack<PrimitiveOperand>();
        Stack<char> operators = new Stack<char>();
        string activeOperand = "";
        OperandTokenType activeType = OperandTokenType.NONE;
        TokenType lastToken = TokenType.NONE;

        for(int i = 0; i < exp.value.Length; i++)
        {   
            char c = exp.value[i];

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
                    throw new Exception("Invalid char");
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
                activeOperand += c; // Possible alternative - track start index and length then take substring?
                break;

                case '.':
                switch(activeType)
                {
                    case OperandTokenType.DECIMAL:
                    throw new Exception("Extra decimal");

                    case OperandTokenType.NONE: case OperandTokenType.INTEGER:
                    activeType = OperandTokenType.DECIMAL;
                    break;

                    case OperandTokenType.VARIABLE: break;

                    default:
                    throw new Exception("Invalid char");
                }
                activeOperand += c;
                break;

                case '!':
                if(activeType != OperandTokenType.NONE)
                    throw new Exception("Exclamation marks may only be at the start of a variable");
                activeType = OperandTokenType.VARIABLE;
                activeOperand += c;
                break;

                // Reads and parses the entire string literal in one iteration of the for loop
                case '\'':
                if(activeType > OperandTokenType.NONE)
                    throw new Exception("Transitioning to string with active operand");
                activeType = OperandTokenType.STRING;

                int stringEndIndex = -1;
                for(int j = i+1; j < exp.value.Length; j++)
                    if(exp.value[j] == '\'' && exp.value[j-1] != '`')
                    {
                        stringEndIndex = j;
                        break;
                    }
                if(stringEndIndex == -1)
                    throw new Exception("Could not find end to string");
                
                // At this point we know the start and end index of the string,
                // and that the index before the end quote does not have a `
                for(int j = i+1; j < stringEndIndex; j++)
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
                                throw new Exception("Invalid escape sequence");
                        }
                        j++;
                        break;

                        default:
                        activeOperand += exp.value[j];
                        break;
                    }

                // Finally push the string operand
                TryPushOperand();
                i = stringEndIndex;
                break;

                case '+':
                TryPushOperand();
                if(lastToken != TokenType.OPERAND)
                    c = SYM_UNARY_ADDITION;
                goto LABEL_EVAL;

                case '-':
                TryPushOperand();
                if(lastToken != TokenType.OPERAND)
                    c = SYM_UNARY_SUBTRACTION;
                goto LABEL_EVAL;

                case '~':
                TryPushOperand();
                if(exp.value[i+1] == '=')
                {
                    c = SYM_NOT_EQUAL;
                    i++;
                }
                goto LABEL_EVAL;

                case '<':            // Since these are operators
                TryPushOperand();    // Out-of-bounds index implies
                if(exp.value[i+1] == '=')  // bad expression syntax
                {
                    c = SYM_LESS_THAN_EQUAL;
                    i++;
                }
                goto LABEL_EVAL;

                case '>':
                TryPushOperand();
                if(exp.value[i+1] == '=')
                {
                    c = SYM_GREATER_THAN_EQUAL;
                    i++;
                }
                goto LABEL_EVAL;

                case '*': case '/': case '%': 
                case '=': case '&': case '|': 
                TryPushOperand();
                LABEL_EVAL:
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
                if(activeType == OperandTokenType.VARIABLE) // Function call where activeOperand is the function name
                    i = functionHandler(i);                 // i becomes close parentheses index
                else
                {
                    operators.Push(c);
                    lastToken = TokenType.OPERATOR;
                }
                break;

                case ')':
                TryPushOperand();
                while(operators.Peek() != '(')
                    eval();
                operators.Pop();               // At this point, the contents of the parentheses
                lastToken = TokenType.OPERAND; // Have been resolved to a single operand
                break;

                case '[':
                activeType = OperandTokenType.STRING;
                TryPushOperand();  // The current name will be popped from
                operators.Push(c); // the stack are resolving the bracket contents
                lastToken = TokenType.OPERATOR;
                break;

                case ']':
                TryPushOperand();
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
                    throw new Exception("Bracket result must be an integer!");
                }
                activeType = OperandTokenType.VARIABLE;
                lastToken = TokenType.OPERATOR; // No operand should follow another operand
                break;

                default:
                throw new Exception("Unrecognized character");
            }
            printStacks(0);
        }
        TryPushOperand();
        while(operators.Count > 0)
            eval();
        
        if(operands.Count == 1)
            return operands.Pop();
        else
            throw new Exception("Evaluation Failed - Unbalanced stacks");

        void eval()
        {
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
            }
        }

        void TryPushOperand()
        {
            switch(activeType)
            {
                // We might not have an operand to push (equivalent to token string being empty)
                case OperandTokenType.NONE:
                    return;
                
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
                        throw new Exception("Could not find variable '" + activeOperand + "'");
                    
                    Operand varValue = isGlobal ? globalVars[activeOperand] : exp.localVars[activeOperand];

                    switch(varValue)
                    {
                        case PrimitiveOperand v1:
                        operands.Push(v1);
                        break;

                        case ExpressionOperand v2:
                        operands.Push(evaluate(v2));
                        break;
                    }
                break;
            }

            // Ensure type trackers are ready for next token
            activeOperand = "";
            lastToken = TokenType.OPERAND;
            activeType = OperandTokenType.NONE;
        }

        int functionHandler(int openIndex)
        {
            if(!functions.ContainsKey(activeOperand))
                throw new Exception("Function name not recognized");

            // STEP 1: EXTRACT THE RAW PARAMETERS
            // Track parentheses/non-escaped string chars to ensure we locate the right commas
            FxParamType[] parmData = functions[activeOperand].paramInfo;
            string[] rawParms = new string[parmData.Length];
            int closeIndex = openIndex + 1; // Gets iterated on until the close parentheses is reached

            for(int i = 0; i < rawParms.Length; i++)
            {
                int paramStartIndex = closeIndex;
                char delimiter = i == rawParms.Length - 1 ? ')' : ',';
                bool foundParamEndIndex = false;
                Stack<char> extraDelimiters = new Stack<char>();
                while(!foundParamEndIndex && closeIndex < exp.value.Length)
                {
                    switch(exp.value[closeIndex])
                    {
                        case '(':
                        if(extraDelimiters.Count > 0)
                        {
                            if(extraDelimiters.Peek() != '\'')
                                extraDelimiters.Push('(');
                        }
                        else extraDelimiters.Push('(');
                        break;

                        case ')':
                        if(extraDelimiters.Count > 0)
                        {
                            if(extraDelimiters.Peek() == '(')
                                extraDelimiters.Pop();
                        }
                        else if(delimiter == ')') // Once found, closeIndex will be delimiter index + 1
                            foundParamEndIndex = true;
                        else
                            throw new Exception("Too few arguments received for function call"); 
                        break;

                        case '\'':
                        if(extraDelimiters.Count > 0)
                        {
                            switch(extraDelimiters.Peek())
                            {
                                case '(':
                                extraDelimiters.Push('\'');
                                break;

                                case '\'':
                                if(exp.value[closeIndex - 1] != '`') // Accounts for escape sequence
                                    extraDelimiters.Pop();
                                break;
                            }
                        }
                        else extraDelimiters.Push('\'');
                        break;

                        case ',':
                        if(extraDelimiters.Count == 0)
                        {
                            if(delimiter == ',')
                                foundParamEndIndex = true;
                            else
                                throw new Exception("Too many arguments received for function call");
                        }
                        break;
                    }
                    closeIndex++;
                }
                if(!foundParamEndIndex)
                    throw new Exception("Failed to find parameter #" + (i+1) + " for function call.");
                rawParms[i] = exp.value.Substring(paramStartIndex, closeIndex - paramStartIndex - 1);
            }
            closeIndex--; // Fixup the final index to match the close parentheses

            // STEP 2: BUILD LOCAL CALL PARMS
            VariableHandler callVariables = new VariableHandler();

            for(int i = 0; i < rawParms.Length; i++)
            {
                switch(parmData[i])
                {
                    case FxParamType.PRIMITIVE:
                        ExpressionOperand toPrim = new ExpressionOperand(rawParms[i], exp.localVars);
                        callVariables.Add("!" + i, evaluate(toPrim));
                    break;

                    case FxParamType.EXPRESSION:
                        callVariables.addExpressionOperand("!" + i, rawParms[i], exp.localVars);
                    break;

                    case FxParamType.REFERENCE:
                    throw new Exception("Not implemented");
                }
            }

            // STEP 3: EXECUTE THE FUNCTION CALL
            switch(functions[activeOperand])
            {
                case UserFunction f1:
                    ExpressionOperand toPrim = new ExpressionOperand(f1.expression, callVariables);
                    operands.Push(evaluate(toPrim));
                break;

                case CodedFunction f2:
                    operands.Push(f2.eval(this, callVariables));
                break;
            }
            activeOperand = "";
            activeType = OperandTokenType.NONE;
            lastToken = TokenType.OPERAND;
            return closeIndex;
        }

        void printStacks(int printIfOne)
        {
            if(printIfOne != 1)
                return;

            Console.Write("Operators: ");
            foreach(char c in operators)
                Console.Write(c + "   ");
            Console.Write("\nOperands: ");
            foreach(Operand o in operands)
                Console.Write(o.ToString() + "   ");
            Console.Write("\n\n");
        }
    }

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
}