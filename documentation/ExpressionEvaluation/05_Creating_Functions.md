# Creating Functions
At the top-most level, creating your own function consists of two steps.
1. Creating a custom instance of a `FunctionDef`
2. Adding this instance to the Evaluator's `FunctionDictionary`

Step #1 is accomplished differently if creating a `CodedFunction` versus an `ExpressionFunction`. However, both classes of functions require choosing your parameters' types and accessing your parameters.

## Parameters
Every function needs AT LEAST one parameter. Each parameter needs an `FxParamType` enum value associated with it, which determines how the parameter is internally parsed and converted to variables.

---

### FxParamType.PRIMITIVE
The parameter is fully evaluated and resolved to a `PrimitiveOperand` before the function call executes.

---

### FxParamType.EXPRESSION
The raw, unevaluated parameter is passed to the function call as an `ExpressionOperand`

Expression parameters are best utilized by a `CodedFunction`.

---

### FxParamType.REFERENCE
> If you are not using the JSON Parser, you can most likely ignore this parameter type.

Reference parameters simulate the passing of an object reference.
* The parameter MUST resolve to a string
* The string MUST be the name of a variable defined and accessible to the expression.

The following variables are passed as parameters:
* The variable whose name equals the string
* Any variable whose name starts with the passed string, followed by a `[` or `.`
* Local variables shadow global variables when passed.

Example: Let `list[0]` be the reference parameter

| globalVars - Passed?      | localVars - Passed?   |
| :---                      | :---                  |
| `list` - No               | `list` - No           | 
| `list[0]` - Yes           |                       | 
| `list[0][5]` - Yes        |                       | 
|                           | `list[0].tag` - Yes   | 
| `list[0]_data` - No       |                       |
|                           | `list[0]Extra` - No   | 
| `list[0].shadow` - No     | `list[0].shadow` - Yes|


### Using Passed Parameters

Every function call creates a `VarDictionary` containing the parameters passed to the function. Variables are named according to the parameter's index.

Example:
```
function(2.5, 'hello', 1)

Variables:
!0 = 2.5
!1 = 'hello'
!2 = 1
```

Reference parameters are named in a similar way:
```
function('list[0]')  // Let 'list[0]' be a reference parameter

Passed variables get named as follows:
list[0]         <--> !0
list[0].data    <--> !0.data
list[0][1][2]   <--> !0[1][2]
(and so forth)
```

## Creating FunctionDef Instance

See `JSONEval/ExpressionEvaluation/functions/FunctionDef.cs` for class definition

### Coded Functions

To create a hard-coded function, you must create a class inheriting the abstract class `CodedFunction`

See `JSONEval/ExpressionEvaluation/functions/CodedFunction.cs` for class definition and all implementations of `CodedFunction` in the evaluator's Standard Function Library. These standard functions should provide adequate examples to learn from.

### Expression-Based Functions

For Expression-Based Functions, simply create an instance of the `ExpressionFunction` class
* See `JSONEval/ExpressionEvaluation/functions/ExpressionFunction.cs`

Example:
```csharp
using JSONEval.ExpressionEvaluation;
ExpressionFunction myFunction = new ExpressionFunction("!0 + !1", FxParamType.PRIMITIVE, FxParamType.PRIMITIVE);
```

> Expression-Based Functions can also be defined using the JSON Parser. See the relevant JSONParser documentation to learn about that functionality.

## Registering the Function

The Evaluator defines a `FunctionDictionary` that maps function names to `FunctionDef` instances. All expressions can access the functions stored in that dictionary.
* See `JSONEval/ExpressionEvaluation/FunctionDictionary.cs`

Example:
```csharp
using JSONEval.ExpressionEvaluation;
ExpressionFunction myFunction = new ExpressionFunction("!0 + !1", FxParamType.PRIMITIVE, FxParamType.PRIMITIVE);
Evaluator.functions.Add("CustomFunction", myFunction);
```
For more examples, see the static constructor in `JSONEval/ExpressionEvaluation/Evaluator.cs`

Function Names:
* Obey the same evaluator-enforced naming rules as variable names
* Are case-insensitive

It is recommended that simple function names containing only letters (A-Z), (a-z) and underscores ( _ ) are used.