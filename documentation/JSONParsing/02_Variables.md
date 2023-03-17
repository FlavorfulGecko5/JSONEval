# Variable Properties
Shows how JSON properties get converted to variables.

## Numbers and Booleans
Integer, decimal and Boolean JSON properties are parsed into the appropriate types of variables

JSON Input:

```json
{
    "int": 5,
    "float": 2.5,
    "A_Boolean": true
}
```
Variables Produced:

| Name | Type | Value |
| :---: | :---: | :---:  |
| int | Integer | 5 |
| float | Decimal | 2.5 |
| A_Boolean | Boolean | True |

## Lists
Every element in a JSON list property gets parsed into variables.

```json
{
    "list": [456, true, 2.5]
}
```

The variable with the property's original name represents the number of elements in the list.
| Name | Type | Value |
| :---: | :---: | :---:  |
| list | Integer | 3 |
| list[0] | Integer | 456 |
| list[1] | Boolean | True |
| list[2] | Decimal | 2.5 |

## Objects
Every property in a JSON object can be parsed into variables.

```json
{
    "obj": {
        "x": 4,
        "y": 5,
        "z": 6
    }
}
```
Notice the object property's name is used to create a string variable.
| Name | Type | Value |
| :---: | :---: | :---:  |
| obj | String | UNDEFINED |
| obj.x | Integer | 4 |
| obj.y | Integer | 5 |
| obj.z | Integer | 6 |


### Value Property
An Object's `Value` property is used by the Parser to set a value for the object variable.

```json
{
    "obj": {
        "x": 4,
        "y": 5,
        "z": 6,

        "Value": false
    }
}
```

Notice how the `obj` variable's type and value changed in comparison to the previous example:
| Name | Type | Value |
| :---: | :---: | :---:  |
| obj | Boolean | False |
| obj.x | Integer | 4 |
| obj.y | Integer | 5 |
| obj.z | Integer | 6 |

## Nesting Objects and Lists

Objects and lists can be freely nested inside each other:

```json
{
    "bigList": [
        90, 
        [true, false], 
        {"Value": 45, "nested": -40}
    ],

    "largeObject": {
        "a": [true],
        "b": {
            "c": false,
            "Value": 2.5
        },
        "Value": [4.5]
    }
}
```

| Name | Type | Value |
| :---: | :---: | :---:  |
| bigList | Integer | 3 |
| bigList[0] | Integer | 90 |
| bigList[1] | Integer | 2 |
| bigList[1][0] | Boolean | True |
| bigList[1][1] | Boolean | False |
| bigList[2] | Integer | 45 |
| bigList[2].nested | Integer | -40 |
| largeObject | Integer | 1 |
| largeObject[0] | Decimal | 4.5 |
| largeObject.a | Integer | 1 |
| largeObject.a[0] | Boolean | True |
| largeObject.b | Decimal | 2.5 |
| largeObject.b.c | Boolean | False |

Note that an object's `Value` property cannot be an object.

## Strings and Expressions
JSON string properties can be parsed into either string or expression variables. As seen in `Parser_Objects.md` the type is determined by a Parser's instance property `StringExpressions` - this is set to `true` by default.

```json
{
    "propA": "3 + 4",
    "propB": " 'Hello World!' + propA "
}
```
A newly created `Parser` would generate the following variables:
| Name | Type | Value |
| :---: | :---: | :---:  |
| propA | Expression | 3 + 4 |
| propB | Expression | 'Hello World!' + propA |

The `StringExpressions` Boolean may be set by the JSON. Parsers reserve a JSON property of the same name for this function.

```json
{
    "propA": "3 + 4",
    "propB": " 'Hello World!' + propA ",

    "StringExpressions": false
}
```

Notice how these variable's types have changed:
| Name | Type | Value |
| :---: | :---: | :---:  |
| propA | String | "3 + 4" |
| propB | String | " 'Hello World!' + propA " |

Naturally, string properties may be included inside of lists and objects:

```json
{
    "StringExpressions": true,

    "expList": ["5 + 4 + 3", "expList[0]"],
    "expObject": {
        "Value": "'Hello World! ' + expList[1]"
    }
}
```
| Name | Type | Value |
| :---: | :---: | :---:  |
| expList | Integer | 2 |
| expList[0] | Expression | 5 + 4 + 3 |
| expList[1] | Expression | expList[0] |
| expObject | Expression | 'Hello World! ' + expList[1] |

String properties may have their individual types specified using the `Type` system discussed in the following section.

## Object Types
The Parser uses an object's `Type` property to determine how it's `Value` gets converted to variables.

The `Type` must be a string, or left undefined. The valid `Type` strings are:
| Type String | Description |
| :---:| :---|
| STANDARD | Processed like a normal property. <br>Used if `Type` is undefined. |
| STRING | `Value` is processed into a single string variable. |
| EXPRESSION | `Value` is processed into a single expression variable. |

The following example showcases the `STRING` and `EXPRESSION` types.
```json
{
    "stringA": {
        "Type": "string",
        "Value": "Hello World!"
    },

    "stringB": {
        "Type": "String",
        "Value": 234
    },

    "stringC": {
        "Type": "STRING",
        "Value": ["Line #1\n", "Line #2\n", "Line #3\n"]
    },

    "mixed": {
        "Type": "Expression",
        "Value": [5, " + ", 9],

        "nestedExp": {
            "Type": "Expression",
            "Value": true
        },

        "nestedString": {
            "Type": "String",
            "Value": false
        }
    }
}
```
Notice the list elements are merged into single variables. Consequently, these lists cannot contain nested lists or objects like `Standard` lists.
| Name | Type | Value |
| :---: | :---: | :---:  |
| stringA | String | "Hello World!" |
| stringB | String | "234"|
| stringC | String | "Line #1\nLine#2\nLine#3\n" |
| mixed | Expression | 5 + 9 |
| mixed.nestedExp | Expression | True |
| mixed.nestedString | String | "False" |