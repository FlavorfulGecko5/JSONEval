# JSONEval
A .Net C# library that parses JSON files for variables and uses them to dynamically evaluate expressions from strings. The libary's expression evaluation component can be fully utilized without the JSON parser.

JSONEval is available on Nuget:
https://www.nuget.org/packages/FG5.JSONEval/

Source Code is available on GitHub: https://github.com/FlavorfulGecko5/JSONEval

## Dependencies
* The Expression Evaluator is built entirely from scratch, with no third-party libraries or dependencies
* The JSON Parser utilizes [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/#readme-body-tab)
* JSONEval is built on .Net 6.0. In all likelihood, it is compatible with many older versions of .Net

## Show Me
Want to see if this library suits your needs without reading a small book? Look no further!

### Expression Evaluator
This C# snippet demonstrates how to quickly setup and use the evaluator without the JSON parser.
```csharp
using static JSONEval.ExpressionEvaluation.Evaluator;
using JSONEval.ExpressionEvaluation;

// Start by adding some variables
globalVars.AddIntVar("x", 5);
globalVars.AddDecimalVar("y", 2.5);
globalVars.AddBoolVar("some_boolean", true);
globalVars.AddStringVar("stringVar", "hello there");

// Evaluate:
Evaluate("x * 2");        // returns 10
Evaluate("-x / (5 - y)"); // returns -2.0

// A full range of logical statements are useable
Evaluate("some_boolean = true");   // Returns the Boolean 'True'
Evaluate("x <= 4 | y > 200");      // Returns the Boolean 'False'

// Strings can be utilized
// Returns the string "hello there String concatenation 5"
Evaluate(" stringVar + ' String concatenation ' + x "); 

// An adequate library of functions are present - and other custom functions are easily implemented:
Evaluate("if(x ~= 0, 500 / x, 0)"); // returns 100
Evaluate("string(x)"); // returns the string "5"

// How to use the result of an expression?
// Method 1: Convert the result to a string
string resultString = Evaluate("x + y").GetValueString();

// Method 2: Use the raw data type
switch(Evaluate("x + y"))
{
    case IntOperand r1:
    int i = r1.value;
    break;

    case DecimalOperand r2:
    double d = r2.value;
    break;

    case BoolOperand r3:
    bool b = r3.value;
    break;

    case StringOperand r4:
    string s = r4.value;
    break;
}
```
See the documentation for a breakdown on all the evaluator's features and nuances.

### JSON Variable Parsing
This C# snippet demonstrates how to quickly parse variables from JSON files.
```csharp
using JSONEval.ExpressionEvaluation;
using JSONEval.JSONParsing;

// Create your Parser object
// Make it add directly to the Evaluator's global variables
Parser p = new Parser()
{
    vars = Evaluator.globalVars
};

// Parse your JSON
string json = File.ReadAllText("Your_JSON_File.json");
p.Parse(json);

// Start evaluating expressions using your newly parsed variables
PrimitiveOperand result = Evaluator.Evaluate(" 'Expression goes here' ");
```

The following example show a JSON file and lists what variables are generated when it's parsed.
```json
{
    "int_var": 123,
    "decimal_var": 10.5,
    "bool_var": true,

    "basic_object": {
        "Value": false,

        "x": 90,
        "y": 91,
        "z": 92,

        "! I am a comment": true
    },

    "string_property": "1 + 2 + 3",

    "true_string": {
        "Type": "string",
        "Value": "Hello World!"
    },

    "true_expression": {
        "Type": "expression",
        "Value": "4 + 5 + 6"
    },

    "list": [454, true, " if( true, 100, 200) "],

    "!Comment": "I am also a comment",

    "nested": [
        [14.5, 15],
        true,
        {"Value": 5, "xyz": 95}
    ]
}
```
| Name | Type | Value |
| :---: | :---: | :---:  |
| int_var | Integer | 123 |
| decimal_var | Decimal | 10.5 |
| bool_var | Boolean | True
| basic_object | Boolean | False |
| basic_object.x | Integer | 90 |
| basic_object.y | Integer | 91 |
| basic_object.z | Integer | 92 |
| string_property | Expression | 1 + 2 + 3 |
| true_string | String | "Hello World!" |
| true_expression | Expression | 4 + 5 + 6 |
| list | Integer | 3 |
| list[0] | Integer | 454 |
| list[1] | Boolean | True |
| list[2] | Expression | if( true, 100, 200) |
| nested | Integer | 3 |
| nested[0] | Integer | 2 |
| nested[0][0] | Decimal | 14.5 |
| nested[0][1] | Integer | 15 |
| nested[1] | Boolean | True |
| nested[2] | Integer | 5 |
| nested[2].xyz | Integer | 95 |

See the documentation for a breakdown on all the JSON parser's features and nuances.