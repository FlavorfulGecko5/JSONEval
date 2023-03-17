# Defining Functions
`ExpressionFunctions` can be defined and added to the Evaluator's `FunctionDictionary` using the JSON Parser. 

Parsers reserve the `Functions` property for this purpose.

This document assumes you've read and understood the documentation on functions in `documentation/ExpressionEvaluation/`

## Simple Example

```json
{
    "Functions": {

        "square": {
            "Parameters": "primitive",
            "Definition": "!0 * !0"
        },

        "multiply": {
            "Parameters": ["PRIMITIVE", "   primitive   "],
            "Definition": ["!0 ", " * ", "  !1 "],
        }
    }
}
```
This JSON is equivalent to the following C# script:
```csharp
using JSONEval.ExpressionEvaluation;

Evaluator.functions.Add(
    "square", new ExpressionFunction(
        "!0 * !0", FxParamType.PRIMITIVE
));

Evaluator.functions.Add("multiply", new ExpressionFunction(
    "!0 * !1", FxParamType.PRIMITIVE, FxParamType.PRIMITIVE
));
```
### Remarks:
* Each property inside `Functions` is assumed to be a function definition.
* The name of the property becomes the name of the function.
* The `Parameters` property specifies the types of the parameters.
* The `Definition` property specifies the function expression.
* Any properties besides `Parameters` and `Definition` are ignored.
* No variables are created when parsing the `Functions` property.