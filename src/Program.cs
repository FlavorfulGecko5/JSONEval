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

    // ITERATION #1: FULLY FUNCTIONAL BY EARLY INSPECTION
    // - Operands: Single digit integers only
    // - Operators: + - * / %
    //     - NO UNARY + or - 
    // - Other Symbols: ()
    // - Same precedence operators evaluated left-right
    // - Assume no syntax errors
    private string evaluate(ExpressionOperand expWrapper)
    {
        Stack<Operand> operands = new Stack<Operand>();
        Stack<char> operators = new Stack<char>();
        string exp = expWrapper.value;

        for(int i = 0; i < exp.Length; i++)
        {
            char c = exp[i];
            if(Char.IsWhiteSpace(c))
                continue;

            switch(c)
            {
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                operands.Push(new IntOperand(c - 48));
                break;

                case '*': case '/': case '%': case '+': case '-':
                while(operators.Count > 0)
                {
                    // <= instead of < Causes left-right evaluation instead of right-left
                    if(precedence[c] <= precedence[operators.Peek()])
                        eval();
                    else
                        break;
                }
                operators.Push(c);
                break;

                case '(':
                operators.Push(c); 
                break;

                case ')':
                while(operators.Peek() != '(')
                    eval();
                operators.Pop();
                break;
            }
            printStacks(0);
        }
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

        void printStacks(int printIfNotZero)
        {
            if(printIfNotZero != 0)
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
}