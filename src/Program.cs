class MainProgram
{
    public static void Main(string[] args)
    {
        Tests.RunUnitTests();
    }
}

class Parser
{
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

    public VariableHandler globalVars = new VariableHandler(true);

    public string evaluate(string exp)
    {
        PrimitiveOperand result = evaluate(new ExpressionOperand(exp), new VariableHandler(false));
        return result.ToString() ?? "How can this possibly return null?";
    }

    private PrimitiveOperand evaluate(ExpressionOperand expWrapper, VariableHandler localVars)
    {
        Stack<PrimitiveOperand> operands = new Stack<PrimitiveOperand>();
        Stack<char> operators = new Stack<char>();
        string exp = expWrapper.value;
        string activeOperand = "";
        OperandTokenType activeType = OperandTokenType.NONE;
        TokenType lastToken = TokenType.NONE;

        for(int i = 0; i < exp.Length; i++)
        {   
            char c = exp[i];

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
                for(int j = i+1; j < exp.Length; j++)
                    if(exp[j] == '\'' && exp[j-1] != '`')
                    {
                        stringEndIndex = j;
                        break;
                    }
                if(stringEndIndex == -1)
                    throw new Exception("Could not find end to string");
                
                // At this point we know the start and end index of the string,
                // and that the index before the end quote does not have a `
                for(int j = i+1; j < stringEndIndex; j++)
                    switch(exp[j])
                    {
                        // Might want to revise escape sequence system in the future,
                        // considering the revelation that System.Data did not have escape sequences
                        case '`':
                            if(exp[j+1] == '\'')
                            {
                                activeOperand += '\'';
                                j++;
                            }          
                        break;

                        case '\\':
                        switch(exp[j+1]) // This can include the end quote if before it
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
                        activeOperand += exp[j];
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
                if(exp[i+1] == '=')
                {
                    c = SYM_NOT_EQUAL;
                    i++;
                }
                goto LABEL_EVAL;

                case '<':            // Since these are operators
                TryPushOperand();    // Out-of-bounds index implies
                if(exp[i+1] == '=')  // bad expression syntax
                {
                    c = SYM_LESS_THAN_EQUAL;
                    i++;
                }
                goto LABEL_EVAL;

                case '>':
                TryPushOperand();
                if(exp[i+1] == '=')
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
                // Considerations for Try-Push
                // With strict arithmetic rules - an operand into a parenthesis is a syntax error
                // Alternatively, could implicitly add a multiplication operator
                operators.Push(c);
                lastToken = TokenType.OPERATOR; 
                break;

                case ')':
                TryPushOperand();
                while(operators.Peek() != '(')
                    eval();
                operators.Pop();
                // A close parenthesis means a sub-expression has been forcibly resolved
                // to an operand - hence, say it's an operand so unary operators function
                // Think harder to make sure this is a good idea or if it's better to make a new enum value for these niche cases
                // (A more complex solution will be needed if you want to add implicit multiplication (please don't....please))
                lastToken = TokenType.OPERAND;
                break;

                case '[':
                activeType = OperandTokenType.STRING;
                TryPushOperand();
                operators.Push(c);
                lastToken = TokenType.OPERATOR;
                break;

                case ']':
                TryPushOperand();
                while(operators.Peek() != '[')
                    eval();
                operators.Pop();

                // First thing to pop is the IntOperand (throw error if not this type)
                // Second thing to pop is the StringOperand containing prior part of the variablename
                // Concatenate prior name + IntOperand + ]
                // Return activetype to variable - last token type should not matter...big question mark?
                Operand bracketResult = operands.Pop();
                if(!bracketResult.GetType().Equals(typeof(IntOperand)))
                    throw new Exception("Bracket result must be an integer!");
                StringOperand priorName = (StringOperand)operands.Pop();
                activeOperand = priorName.value + '[' + bracketResult.ToString() + ']';
                activeType = OperandTokenType.VARIABLE;
                lastToken = TokenType.OPERATOR;
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
            // Deduce the type - Can be done in advance:
            /*
                If active operand length is 0:
                - If first character read in operand is a number --> integer
                - If first character read is decimal --> decimal
                - If a period is read at some point and operand is an integer --> decimal
                    - If a period is read and operand is a decimal --> throw a syntax error
                - If first character read is a single quote --> string
                - If first character read is a letter, underscore, or exclamation --> expression --> Fetch from variables and:
                    > If type is int, decimal or bool - push
                    > If expression - evaluate in recursive call (.json string vars should be capable of acting as standalone expressions)
                    > If it can't be found in the variable dictionary - throw an error
            */
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
                    bool isLocal = localVars.ContainsKey(activeOperand);

                    // Checking for variable presence in both dictionaries shouldn't be necessary
                    // if the rest of the code is put together properly
                    if(!isGlobal && !isLocal)
                        throw new Exception("Could not find variable '" + activeOperand + "'");
                    
                    VariableOperand varValue = isGlobal ? globalVars[activeOperand] : localVars[activeOperand];

                    switch(varValue)
                    {
                        case IntOperand: case DecimalOperand: case BoolOperand:
                        operands.Push((PrimitiveOperand)varValue);
                        break;

                        case ExpressionOperand v1:
                        operands.Push(evaluate(v1, new VariableHandler(false)));
                        break;
                    }
                break;
            }

            // Ensure type trackers are ready for next token
            activeOperand = "";
            lastToken = TokenType.OPERAND;
            activeType = OperandTokenType.NONE;
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