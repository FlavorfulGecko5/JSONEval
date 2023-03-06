# Variables
Variables can represent:
* Any basic Operand type (Integers, Decimals, Booleans, and Strings)
* Another Expression stored as a string
    * See `JSONEval/ExpressionEvaluation/operands/ExpressionOperand.cs`

## Defining and Storing Variables
All variables are stored inside a modified Dictionary called a `VarDictionary`
* See `JSONEval/ExpressionEvaluation/VarDictionary.cs` for the class definition.

Variables are explicitly typed, and the `VarDictionary` has methods for defining variables of each type.

```csharp
using JSONEval.ExpressionEvaluation;

VarDictionary variables = new VarDictionary();
variables.AddIntVar       ("integer_variable", 123);
variables.AddDecimalVar   ("decimal_variable", 1.33);
variables.AddBoolVar      ("bool_variable",    true);
variables.AddStringVar    ("string_variable",  "Hello World!");
variables.AddExpressionVar("exp_variable",     "100 + integer_variable > decimal_variable");
```

### Global Variables
The evaluator defines a `VarDictionary` whose variables are available to all expressions:

```csharp
using JSONEval.ExpressionEvaluation;

Evaluator.globalVars.AddIntVar("global_integer", 547);
Evaluator.Evaluate("global_integer - 43");
```

By default, `globalVars` contains the variables `true` and `false` to represent the appropriate Boolean values. Modification or removal of these is possible, but not recommended.

### Local Variables
In addition to `globalVars`, all expressions can reference a unique `VarDictionary` called `localVars`
```csharp
using JSONEval.ExpressionEvaluation;

ExpressionOperand exp = new ExpressionOperand("local_integer - 45");
exp.localVars.AddIntVar("local_integer", 4532);
Evaluator.Evaluate(exp);

// This next statement fails to evaluate.
// Expressions passed as a string have no local variables
// and "local_integer" isn't defined in globalVars
Evaluator.Evaluate("local_integer - 45");
```

### Shadowing
A variable in an expression's `localVars` can have the same name as a `globalVars` variable. If this occurs, the `localVars` entry is used over the `globalVars` entry.
```csharp
using JSONEval.ExpressionEvaluation;

ExpressionOperand exp = new ExpressionOperand("shadowed_var + 5");
exp.localVars.AddIntVar("shadowed_var", 234);
Evaluator.globalVars.AddBoolVar("shadowed_var", true);

// The integer '239' is returned due to the local variable
// shadowing it's global copy.
Evaluator.Evaluate(exp);
```

### Case Insensitivity
The `VarDictionary` uses case-insensitive name lookups:
```csharp
using static JSONEval.ExpressionEvaluation.Evaluator;

globalVars.AddStringVar("A_STRING", "Hello World!");

// These expressions access the same variable "A_STRING" 
// and return the same result
Evaluate("A_STRING + 234");
Evaluate("a_String + 234");

// Consequently, this next statement causes an error
// because "A_STRING" is already defined in globalVars
globalVars.AddStringVar("a_string", "hello_world");
```

## Naming Rules
The following table lists all characters useable in variable names and any constraints when using them.

| Character(s)                               | Constraints                             | Example Names           |
| :---                                       | :---                                    | :---                    |
| Letters (A-Z) (a-z) <br> Underscores ( _ ) | Useable anywhere in the name.           | `Simple_Name`           |
| Numbers (0-9) <br> Periods ( . )           | Useable AFTER the first character       | `_0342_a_number` <br> `Complex.Variable.Name`|
| Exclamation Marks ( ! )                    | May only be used as the first character | `!Var_With_Exclamation` |
| Brackets ( [ ] )                           | Useable AFTER the first character. <br> Format MUST be `[INTEGER]` | `list[0]_val` <br> `a[234][34]` |


### Enforcement
Variable naming rules are enforced by the evaluator. Adding a variable with an invalid name to a `VarDictionary` will not cause an error. Errors only occur when expressions attempt to utilize that variable.

### Recommended Conventions
These naming rules revolve around usage of the JSON Parser. If working directly with a `VarDictionary` to define variables instead of using the JSON Parser, the following naming conventions are recommended:
* Do not use exclamation marks. Exclamation marks are used when the evaluator auto-generates variables under specific circumstances
* Do not use periods or brackets unless you understand what you're doing.

## Bracketed Expressions
As seen in the above section, variable names may include bracketed integers. When referencing these variables, the contents of brackets are initially regarded as expressions:

```csharp
using static JSONEval.ExpressionEvaluation.Evaluator;

globalVars.AddIntVar("list[0]_value", 100);

// After evaluating the bracketed contents,
// the variable list[0]_value will be used
Evaluate(" list[ 5 * 2 - 10  ]_value");
```


