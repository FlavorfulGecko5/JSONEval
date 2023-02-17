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
        Parser.globalVars.addIntOperand("firstvar", 34);
        assert("firstvar", "34");
        assert("FIRSTVAR + 500", "534");
        Parser.globalVars.addExpressionOperand("basicexpression", "1 + 2 + 3");
        assert("basicexpression", "6");
        assert("true", "True");
        assert("FALSE", "False");
        assert("true + ' big victory'", "True big victory");
        assert("firstvar * basicexpression", "204");
        Parser.globalVars.addExpressionOperand("nested", "basicexpression + firstvar");
        assert("nested", "40");

        // Iteration #6 variable naming tests
        Parser.globalVars.addIntOperand("_hello_there", 450);
        assert("_hello_there", "450");
        Parser.globalVars.addIntOperand("!advanced.naming.stuff", 400);
        assert("!advanced.naming.stuff", "400");
        Parser.globalVars.addIntOperand("b[0]", 234);
        Parser.globalVars.addIntOperand("b[1]", 100);
        Parser.globalVars.addIntOperand("b[0][0]", 5);
        Parser.globalVars.addIntOperand("b[100]", -1);
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

        // Iteration #8 UserFunction with Primitive Parameter tests
        Parser.functions.Add("funcA", new UserFunction("!0 + !1 + !2", FxParamType.PRIMITIVE, FxParamType.PRIMITIVE, FxParamType.PRIMITIVE));
        Parser.functions.Add("delimtest", new UserFunction("!0", FxParamType.PRIMITIVE));
        assert("funcA(   1 , 2  , 3 )", "6");
        assert("funcA(  'hello('  , 'door', '')"  , "hello(door");
        assert("DELIMTEST( ',()(`',,(`'`'),()(,,())' )", ",()(',,(''),()(,,())");
        assert("delimtest(1)+ 3", "4");
        assert("delimtest(delimtest('(boo)'))", "(boo)");
        assert("delimtest((~true))", "False");

        // Iteration #9 Coded Functions and Expression Parameter tests
        assert("if(true, 5, 1 / 0)", "5");
        assert("if(true & false, 1 / 0, 4)", "4");
        assert("and(true, true)", "True");
        assert("and(true, false)", "False");
        assert("and(false, true)", "False");
        assert("AND(false, false)", "False");
        assert("OR(true, true)", "True");
        assert("OR(true, false)", "True");
        assert("OR(false, true)", "True");
        assert("OR(false, false)", "False");
        assert("int(1.5)", "1");
        assert("int(-1.5)", "-1");
        assert("int(true)", "1");
        assert("int(false)", "0");
        
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