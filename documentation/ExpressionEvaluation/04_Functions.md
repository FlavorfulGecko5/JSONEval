# Functions
Two types of functions exist:
* Hardcoded functions that require a custom C# method to return their desired results
* Functions defined as an expression

Both types of functions are utilized the same way:
`FUNCTION_NAME(parameter 1, parameter 2, ...)`
* Function names are case-insensitive
* All functions have a fixed number of required parameters
* No whitespace can exist between the function name and the open parentheses.

# Standard Function Library
The following functions come readily-available with the evaluator for use in any expression.

## Conditional If/Else
Name: `if`

Parameters:
1. An expression that resolves to a Boolean
2. The expression to evaluate ONLY IF parameter #1 evaluates to `True`
3. The expression to evaluate ONLY IF parameter #1 evaluates to `False`

Examples:
```
if('hello' = 'world', 10, 100)
if(x = 0, 0, 400 / x)
```

## Short-Circuiting Logical AND
Name: `and`

Parameters
1. An expression that resolves to a Boolean
2. An expression that resolves to a Boolean. Evaluated ONLY IF parameter #1 resolves to `True`

Examples:
```
and(true, false)
and(5 < 7 , 8 ~= 10)
```

## Short-Circuiting Logical OR
Name: `or`

Parameters
1. An expression that resolves to a Boolean
2. An expression that resolves to a Boolean. Evaluated ONLY IF parameter #1 resolves to `False`

Examples:
```
or(true, false)
or(5 < 7 , 8 ~= 10)
```

## Integer Casting
Name: `int`

Parameters
1. An expression whose result can be converted to an integer

Examples:
```
// Truncates the decimal
int( -1.5 )

// Boolean 'True' converts to 1, and 'False' converts to 0
int(true)

// Exception is thrown if a string cannot be parsed to an integer
int('-234')
```

## Decimal Casting
Name: `decimal`

Parameters
1. An expression whose result can be converted to a decimal

Examples:
```
decimal( -1 )

// Boolean 'True' converts to 1.0, and 'False' converts to 0.0
decimal(true)

// Exception is thrown if a string cannot be parsed to a decimal
decimal('4.234')
```

## Boolean Casting
Name: `bool`

Parameters
1. An expression whose result can be converted to a Boolean

Examples:
```
// Numbers >= 1 are converted to 'True', all others converted to 'False'
bool( -1 )
bool( 1.45)
bool(3)

// Exception is thrown if a string cannot be parsed to a Boolean
bool('TRUE')
bool('False')
```

## String Casting
Name: `string`

Parameters
1. An expression whose result can be converted to a String

Examples:
```
string(-35)
string(true)
string('Already a string')
```


## Restricted For Loop
Name: `loop`

Parameters
1. The initial value of the incrementer - must resolve to an integer.
2. The final value of the incrementer - must resolve to an integer.
3. The inital value - the result if 0 loop iterations are performed.
4. The expression to loop - each iteration is added to parameter #3

Behavior
* The loop header is equivalent to `for(int i = parameter #1; i <= parameter #2; i++)`
* The special variable `!i` is useable in parameter #4 to represent the current incrementer value
    * If a `loop` function call is nested inside the expression of another `loop` function call, the variable `!ii` is used to represent the inner `loop` incrementer (and so on for however many nested `loop` functions exist)

Examples
```
loop(1, 3, 0, !i)   // Returns the Integer sum of 1 + 2 + 3
loop(1, 3, '', !i)  // Returns the String '123' (the concatenation of '' + 1 + 2 + 3)

loop(5, 4, 0, !i)   // Returns the Integer 0 (no loop iterations are performed because 5 > 4)

loop(1, 1, 0, loop(!i, !i, 0, !i + !ii)) // Returns the Integer 2 - take note of the inner loop's incrementer variable
```