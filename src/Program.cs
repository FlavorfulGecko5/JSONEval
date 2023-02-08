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
    const int PRECEDENCE_UNARY = 5000;
    static readonly Dictionary<char, int> precedence = new Dictionary<char, int>()
    {
        {SYM_UNARY_ADDITION, PRECEDENCE_UNARY}, {SYM_UNARY_SUBTRACTION, PRECEDENCE_UNARY},
        {'*', 2000}, {'/', 2000}, {'%', 2000},
        {'+', 1000}, {'-', 1000},

        {'(', -10000}, {')', -10000}
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

        // This method is correct - check by last token type, not by last
        // thing pushed to a stack
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

            if((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                if(activeType == OperandTokenType.NONE)
                    activeType = OperandTokenType.VARIABLE;
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
                if(activeType == OperandTokenType.DECIMAL)
                    throw new Exception("Extra decimal");
                else if (activeType == OperandTokenType.NONE || activeType == OperandTokenType.INTEGER)
                    activeType = OperandTokenType.DECIMAL;
                activeOperand += c;
                break;

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

                case '*': 
                TryPushOperand();
                goto LABEL_EVAL;

                case '/': 
                TryPushOperand();
                goto LABEL_EVAL;

                case '%':
                TryPushOperand();
                goto LABEL_EVAL;

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
                
                This set of rules imply the following about variable names:
                - Cannot start with a number or a period
                - The full range of acceptable chars in a variable name will be:
                    - Any character: (A-Z) (a-z), underscores (_), exclamation marks (!), 
                    - Any character except the first: numbers (0-9) periods (.) and brackets []
                    - If a var has an opening bracket [, it must have a closing bracket ]
                    - The contents of a bracket are a sub-expression that must resolve to an integer
                - The full range of user-defineable chars in a variable name will be:
                    - (A-Z) (a-z), underscores (_)
                    - Others chars are added as a result of parsing the config file and standardizing variable names
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