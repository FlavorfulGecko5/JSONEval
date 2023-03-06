# JSONEval
A .Net C# library that parses JSON files for variables and uses them to dynamically evaluate expressions from strings. The libary's expression evaluation component can be fully utilized without the JSON parser.

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
string resultString = Evaluate("x + y").ToString();

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

```

The following examples show various JSON files and lists what variables are generated when they're parsed.
```json
{
}
```
See the documentation for a breakdown on all the JSON parser's features and nuances.

## Dependencies
* The Expression Evaluator is built entirely from scratch, with no third-party libraries or dependencies