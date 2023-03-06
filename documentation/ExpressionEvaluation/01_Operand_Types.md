# Operand Types
When evaluated, an expression and any operand inside it must resolve to one of four basic types:

## Integers
32-bit signed integers.

```csharp
Evaluate("123");  // Resolves to the Integer value of 123
Evaluate("-456"); // Resolves to the Integer value of -456
```

Behavior defined in `JSONEval/ExpressionEvaluation/operands/IntOperand.cs`

## Decimals
64-bit floating point numbers.

```csharp
Evaluate("1.25"); // Resolves to the Decimal value of 1.25
Evaluate("-.5");  // Resolves to the Decimal value of -0.5
```

Behavior defined in `JSONEval/ExpressionEvaluation/operands/DecimalOperand.cs`

## Booleans
Accessed using `true` and `false` with any variation in capitalization allowed.

```csharp
Evaluate("TRUE");  // Resolves to the Boolean value of 'True'
Evaluate("False"); // Resolves to the Boolean value of 'False'
```

Behavior defined in `JSONEval/ExpressionEvaluation/operands/BoolOperand.cs`

## Strings
String literals can be defined in expressions using single-quotes.

```csharp
Evaluate(" 'Hello World!'   "); // Resolves to the string literal "Hello World!"
```

Behavior defined in `JSONEval/ExpressionEvaluation/operands/StringOperand.cs`

### Escape Sequences
The evaluator uses backticks to identify escape sequences in string literals:
* ``` `n ``` represents a newline character (commonly seen as `\n`).
* ``` `t ``` represents a tab character (commonly seen as `\t`).
* ``` `' ``` represents a single quote

Backticks prefacing any other character are treated as a normal backtick.

```csharp
Evaluate(" '`n' "); // A string literal containing only a newline
Evaluate(" '`t' "); // A string literal containing only a tab character
Evaluate(" '`'' "); // A string literal containing only a single quote
Evaluate(" '`c' "); // The string literal "`c"
```

> Why backticks? Simply put, escape sequences become a nuisance to use if the evaluator uses backslashes. String operands will most likely have their backslashed escape sequences preprocessed before reaching the evaluator, such as when using the JSON parser or when passing the expression as a string in a C# program. To prevent inconsistent and confusing behavior, it's best if the evaluator uses a different character for it's escape sequences.


