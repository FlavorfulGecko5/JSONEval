# Property Naming Rules
The JSON Parser enforces it's own set of property naming rules, stricter than the Evaluator's rules for variable names:
* Property names cannot be empty
* Property names may only contain letters (A-Z) (a-z) and underscores ( _ )

## Comment Properties

Properties whose names start with an exclamation mark ( ! ), are ignored by the parser.

The following JSON produces only one variable:
```json
{
    "! I am a comment": 234,

    "!listComment": [1, 2, 3],

    "!objectComment": {
        "I am not parsed": true,
        "Because our object is a comment": true
    },

    "notComment": {
        "Value": 29,

        "! But I am a comment": 45
    }
}
```
| Name | Type | Value |
| :---: | :---: | :---:  |
| notComment | Integer | 29 |