class MainProgram
{
    public static void Main(string[] args)
    {
        Tests.RunUnitTests();
    }
}

class Parser
{
    static readonly Dictionary<char, int> precedence = new Dictionary<char, int>()
    {
        {'*', 2000}, {'/', 2000}, {'%', 2000},
        {'+', 1000}, {'-', 1000},

        {'(', -10000}, {')', -10000}
    };

    VariableHandler vars = new VariableHandler();

    public string evaluate(string exp)
    {
        return evaluate(new ExpressionOperand(exp));
    }

    private string evaluate(ExpressionOperand expWrapper)
    {
        Stack<Operand> operands = new Stack<Operand>();
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
                
            switch(c)
            {
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                if(activeType == OperandTokenType.NONE)
                    activeType = OperandTokenType.INTEGER;
                activeOperand += c; // Possible alternative - track start index and length then take substring?
                break;

                case '*': case '/': case '%': case '+': case '-':
                TryPushOperand();
                while(operators.Count > 0)
                {
                    // <= instead of < Causes left-right evaluation instead of right-left
                    if(precedence[c] <= precedence[operators.Peek()])
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
            return operands.Pop().ToString() ?? "";
        else
            throw new Exception("Evaluation Failed - Unbalanced stacks");

        void eval()
        {
            Operand rightHand = operands.Pop();
            Operand leftHand = operands.Pop();
            char op = operators.Pop();

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
                - If a decimal is read at some point and operand is an integer --> decimal
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
                // DO ME LATER
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
        NONE, // Should be equivalent to the current token having a length of 0
        INTEGER,
        DECIMAL,
        STRING,
        VARIABLE
    }
}