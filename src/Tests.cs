using System.Diagnostics;
class Tests
{
    public static void RunUnitTests()
    {
        Parser p = new Parser();
        Stopwatch timer = new Stopwatch();
        timer.Start();


        // Iteration #1 Tests
        assert("    3 \n", "3");
        assert("2 + 2", "4");
        assert(" 4 - 5", "-1");
        assert("3 * 3", "9");
        assert("8 / 2", "4");
        assert("8 % 3", "2");
        assert("(8 + 2)", "10");
        assert("8 * (3 + 5)", "64");
        assert("(6) + (7) * (2)", "20");
        assert("(3 - 2 * 4 / 2) - 9 + 7 + (6)", "3");
        assert("2 * (5 * (3 + 6)) / 5 - 2", "16");
        assert("((2 + 3) * 5)", "25");

        // Iteration #2 Tests
        assert("  \n 120 \t", "120");
        assert("(1 + ((30 + 12)) * 200)", "8401");
        assert("500 + 20", "520");
        assert("30-25 ", "5");
        assert("40 / 10 - (0) * 2 + (50 - 50)", "4");

        // Iteration #3 Tests
        assert("+4", "4");
        assert("-4", "-4");
        assert("+      400", "400");
        assert("+-+300", "-300");
        assert("-(30 * 2)", "-60");
        assert("3 + -(20 + 2)", "-19");
        assert("5 * 40 * --20", "4000");
        assert("5 * 40 * (--20)", "4000");
        assert("-(30 * 2)+-50", "-110");
        assert("5 * 2 + - 3", "7");

        // Iteration #4 Floating Point Tests
        assert("1.5", "1.5");
        assert("1.25 + 2.25", "3.5");
        assert("2.50000 *2.5+ 3", "9.25");
        assert(".25", "0.25");
        assert("3.", "3");
        assert("3/2.0", "1.5");

        // Iteration #4 String Tests
        assert("''", "");
        assert("      '   '  ", "   ");
        assert(" 'abcdefg' ", "abcdefg");
        assert("'1 + 2 + 3'", "1 + 2 + 3");
        assert("1 + 2 + '4'", "34");
        assert("'4' + 3 + 2", "432");
        assert("'  hello ' + 'everyone'", "  hello everyone");

        // Iteration #4 String Escape Sequence Tests
        assert("'`''", "'");
        assert(@"'\n'", "\n"); // Better way to test these would be by reading lines from a file
        assert(@"'\\hooray\\'", @"\hooray\");
        

        // Iteration #5 basic variable tests
        p.globalVars.addIntOperand("firstvar", 34);
        assert("firstvar", "34");
        assert("FIRSTVAR + 500", "534");
        p.globalVars.addExpressionOperand("basicexpression", "1 + 2 + 3");
        assert("basicexpression", "6");
        assert("true", "True");
        assert("FALSE", "False");
        assert("true + ' big victory'", "True big victory");
        assert("firstvar * basicexpression", "204");
        p.globalVars.addExpressionOperand("nested", "basicexpression + firstvar");
        assert("nested", "40");

        // Iteration #6 variable naming tests
        p.globalVars.addIntOperand("_hello_there", 450);
        assert("_hello_there", "450");
        p.globalVars.addIntOperand("!advanced.naming.stuff", 400);
        assert("!advanced.naming.stuff", "400");
        p.globalVars.addIntOperand("b[0]", 234);
        p.globalVars.addIntOperand("b[1]", 100);
        p.globalVars.addIntOperand("b[0][0]", 5);
        p.globalVars.addIntOperand("b[100]", -1);
        assert("b[0]", "234");
        assert("b[10 * 10 - 100]", "234");
        assert("b[b[1] + b[0] - 334]", "234");
        assert("b[0][0]", "5");
        assert("b[100]", "-1");

        // Iteration #7 Logical operation tests
        assert("3 & 6 ", "2");
        assert("true & true", "True");
        assert("false & true", "False");
        assert ("false & false", "False");
        assert("7 | 8", "15");
        assert("true | true", "True");
        assert("true | false", "True");
        assert("false | false", "False");
        assert("~ - 1", "0");
        assert("~true", "False");
        assert("~False", "True");
        assert("5 = 5", "True");
        assert(" 4 = 5", "False");
        assert("2.5 = 2.5", "True");
        assert("true = FALSE", "False");
        assert("'string' = 'string'", "True");
        assert(" 4 ~= 5", "True");
        assert(" 5 ~= 5", "False");
        assert("2.5 ~= 56", "True");
        assert("2.0 ~= 2", "False");
        assert("True ~= True", "False");
        assert("'abc' ~= 'abcc'", "True");
        assert("1 < 2", "True");
        assert("1.0 < 2", "True");
        assert("1 < 1", "False");
        assert("1 <= 1", "True");
        assert("1 >= 1", "True");
        assert("1 > 1", "False");
        assert("250.0 <= 250", "True");
        assert("true & (4 <= 5)", "True");
        assert("~true | ~(5 + 7 > 11)", "False");
        
        timer.Stop();
        Console.WriteLine("ALL TESTS SUCCEEDED (Time: {0} MS)", timer.ElapsedMilliseconds);

        void assert(string exp, string expected)
        {
            string result = p.evaluate(exp);
            if (!result.Equals(expected))
            {
                Console.WriteLine("TEST FAILED:\nExpression: \"{0}\"\nExpected: \"{1}\"\nResult: \"{2}\"", exp, expected, result);
                Environment.Exit(0);
            }
        }
    }
}