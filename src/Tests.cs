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