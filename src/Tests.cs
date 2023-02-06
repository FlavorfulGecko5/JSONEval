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