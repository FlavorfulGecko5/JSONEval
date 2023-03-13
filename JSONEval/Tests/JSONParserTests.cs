namespace JSONEval.Tests;
using JSONEval.JSONParsing;
using JSONEval.ExpressionEvaluation;

class ParserTests
{
    private static string jsonA =
@"
{
    $intVar$: 3,
    $decVar$: 5.5,
    $boolVar$: true,
    $expVar$: $5 + 2$,

    $!comment$: 4534,

    $listVar$: [0, 5, false],

    $objVar$: {
        $int$: 5,
        $Value$: 532,

        $! comment   $: [null, {}]
    },

    $nest$: [[{$Value$: 6}], 4],

    $obj_list$: {
        $Value$: [true],

        $extra$: [5]
    },

    $typetesting$: {
        $Type$: $standard$,
        $Value$: 9,

        $stype$: {
            $Type$: $string$,
            $Value$: [true, $ yo$]
        },

        $etype$: {
            $Type$: $expression$,
            $Value$: [3, 2]
        }
    }
}
".Replace('$', '"');

    private static VarDictionary resultsA = new VarDictionary()
    {
        {"intvar", new IntOperand(3)}, {"DECVAR", new DecimalOperand(5.5)},
        {"boOLvAR", BoolOperand.TRUE}, {"expVar", new ExpressionOperand("5 + 2")},

        {"listvar", new IntOperand(3)}, {"listvar[0]", new IntOperand(0)},
        {"listvar[1]", new IntOperand(5)}, {"listvar[2]", BoolOperand.FALSE},

        {"objvar", new IntOperand(532)}, {"objvar.int", new IntOperand(5)},

        {"nest", new IntOperand(2)}, {"nest[0]", new IntOperand(1)},
        {"nest[0][0]", new IntOperand(6)}, {"nest[1]", new IntOperand(4)},

        {"obj_list", new IntOperand(1)}, {"obj_list[0]", BoolOperand.TRUE},
        {"obj_list.extra", new IntOperand(1)}, {"obj_list.extra[0]", new IntOperand(5)},

        {"typetesting", new IntOperand(9)}, {"typetesting.stype", new StringOperand("True yo")},
        {"typetesting.etype", new ExpressionOperand("32")}
    };

    private static string jsonB =
@"
{
    $StringExpressions$: false,

    $realstring$: $345$
}
".Replace('$', '"');

    private static VarDictionary resultsB = new VarDictionary()
    {
        {"realstring", new StringOperand("345")}
    };

    private static string jsonC = 
@"
{
    $Functions$: {
        $abc$: {
            $Parameters$: [$primitive$, $ expression   $, $reference$],
            $Definition$: [234, $ + 45$]
        }
    }
}
".Replace('$', '"');

    private static FunctionDictionary funcC = new FunctionDictionary()
    {
        {"abc", new ExpressionFunction("234 + 45", FxParamType.PRIMITIVE, FxParamType.EXPRESSION, FxParamType.REFERENCE)}
    };

private static string jsonD =
@"
{
    $resA$: 345,
    $resB$: {
        $i$: 5.4
    },
    $keep$: 900
}
".Replace('$', '"');

    private static VarDictionary resultsD = new VarDictionary()
    {
        {"keep", new IntOperand(900)}
    };


    public static void Main(string[] args)
    {
        RunUnitTests();
    }
    
    public static void RunUnitTests()
    {
        // Variable Parsing Tests
        assertEqualDictionaries("Test #1", jsonA, resultsA, new string[0]);
        assertEqualDictionaries("Test #2", jsonB, resultsB, new string[0]);

        // FunctionDef Tests
        Parser functions = new Parser();
        functions.Parse(jsonC);
        foreach(string key in funcC.Keys)
            if(!Evaluator.functions.ContainsKey(key) 
                || !((ExpressionFunction)funcC[key]).Equals((ExpressionFunction)Evaluator.functions[key]))
            {
                Console.WriteLine("JSON Parser Function Definition test failed");
                Environment.Exit(0);
            }
        
        // Reserved Properties Test
        assertEqualDictionaries("Reserved Properties", jsonD, resultsD, new string[]{"resA", "resB"});

        Console.WriteLine("All JSON Parser tests succeeded.");

        void assertEqualDictionaries(string testName, string j, VarDictionary r, string[] reservedProps)
        {
            Parser p = new Parser();
            p.Parse(j, reservedProps);
            if(!p.vars.Equals(r))
            {
                Console.WriteLine("JSON Parser " + testName + " failed.");
                Environment.Exit(0);
            }
        }
    }
}