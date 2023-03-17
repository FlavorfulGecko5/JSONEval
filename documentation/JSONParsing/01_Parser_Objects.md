# Parser Objects
Before showing how JSON files get converted to variables, the `Parser` object performing this process must be understood. 

Generally speaking, the `JSONParsing` documentation assumes you've read and understood the `ExpressionEvaluation` documentation.

## Workflow
The `Parser` is defined at `JSONEval/JSONParsing/Parser.cs` and for each usage it's actions can be summarized as follows:
1. Parse raw JSON data from a string.
2. Convert the JSON's properties into variables, storing them in it's referenced `VarDictionary`
3. Create `ExpressionFunction` instances from function definition properties, adding them to the Evaluator's `FunctionDictionary`
4. Return a list of unprocessed JSON properties reserved by the user, if they exist.

This sample program showcases how to setup and use a `Parser`

```csharp
using JSONEval.JSONParsing;
using JSONEval.ExpressionEvaluation;
using Newtonsoft.Json.Linq;

Parser p = new Parser()
{
    // Instead of adding to an original VarDictionary (default behavior),
    // add directly to the Evaluator's global variables
    vars = Evaluator.globalVars,

    // JSON string properties can be interpreted as expressions (default) or string literals
    // See next section for more information
    StringExpressions = false
};

string json = 
@"
{
    $var$: 435,
    $reserve_me$: true
}
".Replace('$', '"');

Dictionary<string, JToken> reservedProps = p.Parse(json, "reserve_me");
```

