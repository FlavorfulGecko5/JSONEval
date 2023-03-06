# Operators
Information on each operator recognized by the evaluator

## Arithmetic Operators

| Symbol | Operation                  | Examples|
| :---:  | :---                       | :---    |
| +      | Addition (Binary/Unary)    | `2 + 5` <br> `+2 + 5.5` <br> `'String concatenation ' + true + ' works.'`|
| -      | Subtraction (Binary/Unary) | `- 5 - 0.2 + -3`|
| *      | Multiplication             | `5.75 * 10`|
| /      | Division                   | `10 / 3` Truncated Integer Result <br> `10 / 3.0` Decimal Result|
| %      | Modulus (Remainder)        | `10 % 3` <br> `4.4 % 2`|

## Logical Operators

| Symbol | Operation                       | Examples|
| :---:  | :---                            | :---    |
| ~      | Bitwise/Logical NOT             | `~ -1` <br> `~True`|
| &      | Bitwise/Logical AND             | `3 & 4` <br> `True & False`|
| \|     | Bitwise/Logical OR              | `8 \| 7` <br> `True \| False`|
| =      | Equality Comparison             | `2.75 = 23` <br> `True = False` <br> `'Hello' = 'Helo'`|
| ~=     | Inequality Comparison           | `2.75 ~= 23` <br> `False ~= False` <br> `'Hello' ~= 'hello'`|
| <      | Less-Than Comparison            | `6.5 < 10`|
| <=     | Less-Than-Equals Comparison     | `10 <= 13245`|
| >      | Greater-Than Comparison         | `5.3 > 342`|
| >=     | Greater-Than-Equals Comparison  | `943 >= 4`|

## Precedence Groups
The lower an operator group is on this list, the lower it's precedence.
1. Function Calls and Variable Substitutions
2. Unary Operators (Bitwise/Logical NOT, Unary Addition, Unary Subtraction)
3. Multiplication, Division, Modulus
4. Binary Addition and Subtraction
5. Less-Than, Less-Than-Equals, Greater-Than, Greater-Than-Equals Comparisons
6. Equality and Inequality Comparisons
7. Bitwise/Logical AND
8. Bitwise/Logical OR


Clusters of unary operators are evaluated from right to left. Clusters of other same-precedence operators are evaluated from left to right.

## Parentheses
Parentheses function as they normally do in expressions by changing the order operators are evaluated in.

```csharp
Evaluate(" 6 / -(4 - 2) "); // Resolves to the integer -3
Evaluate(" (100 & 0) < 5 ");  // Resolves to the Boolean 'True'
```

## Operand Combinations
Generally speaking, this evaluator closely follows C# in determining what combinations of operands are valid for any given operator. Invalid operand combinations cause an exception to be thrown and evaluation of the expression to terminate. For instance, attempting to subtract a Boolean from a string will cause such an exception.
