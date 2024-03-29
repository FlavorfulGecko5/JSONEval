/*
Copyright (c) 2023 FlavorfulGecko5

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace JSONEval.JSONParsing;
using JSONEval.ExpressionEvaluation;
using Newtonsoft.Json.Linq;

/// <summary>
/// Parses JSON data into variables and functions for the Expression Evaluator
/// </summary>
public class Parser
{
    /// <summary>
    /// Parsed variables are saved to this dictionary
    /// </summary>
    public VarDictionary vars {get; set;} = new VarDictionary();

    /// <summary>
    /// If true, interpret JSON string properties as expressions by default.
    /// If false, interpret them as string literals
    /// </summary>
    public bool StringExpressions {get; set;} = true;

    /// <summary>
    /// Tracks nested property names
    /// </summary>
    private string preface = "";

    /// <summary>
    /// Parses JSON data into variables and functions
    /// </summary>
    /// <param name="rawJson">JSON data to parse</param>
    /// <param name="reservedNames">
    /// Names of JSON Properties to extract and return to the user
    /// instead of parsing into variables.
    /// </param>
    /// <returns>
    /// Dictionary containing any reserved properties found in the JSON data
    /// </returns>
    /// <throws cref="ParserException">
    /// Parsing the JSON fails for any predictable reason.
    /// </throws>
    public Dictionary<string, JToken> Parse(string rawJson, params string[] reservedNames)
    {
        preface = "";

        JObject parsedJson;
        try
        {
            JsonLoadSettings reportExactDuplicates = new JsonLoadSettings()
            {
                DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
            };
            parsedJson = JObject.Parse(rawJson, reportExactDuplicates);
        }
        catch(Newtonsoft.Json.JsonReaderException e)
        {
            throw new ParserException("Could not parse JSON:\n" + e.Message);
        }


        // Set the default string form interpretation
        const string PROPERTY_STRINGEXP = "StringExpressions";
        JToken? stringExpToken = parsedJson.GetValue(PROPERTY_STRINGEXP);
        if(stringExpToken != null)
        {
            if(stringExpToken.Type != JTokenType.Boolean)
                throw Error(PROPERTY_STRINGEXP, "This property must be defined as a Boolean, or left undefined");
            StringExpressions = (bool)stringExpToken;
        }

        // Parse Function definition property
        const string PROPERTY_FUNCTIONS = "Functions";
        const string PROPERTY_FUNCTION_PARAMS = "Parameters";
        const string PROPERTY_FUNCTION_EXP = "Definition";
        JToken? fxToken = parsedJson.GetValue(PROPERTY_FUNCTIONS);
        if(fxToken != null)
        {
            if(fxToken.Type != JTokenType.Object)
                throw Error(PROPERTY_FUNCTIONS, "This property must defined as an object, or left undefined");
            
            preface = PROPERTY_FUNCTIONS + '.';

            foreach(JProperty fxProp in fxToken)
            {
                // Validate function name
                if(fxProp.Name.Length == 0)
                    throw Error(fxProp.Name, "Function names cannot be empty");
                foreach (char c in fxProp.Name)
                    if (!(c <= 'z' && c >= 'a'))
                    if (!(c <= 'Z' && c >= 'A'))
                    if (!(c == '_'))
                        throw Error(fxProp.Name, "Invalid character '" + c + "' in function name.");
                if(Evaluator.functions.ContainsKey(fxProp.Name))
                    throw Error(fxProp.Name, "A function with this name is already defined");

                // Get function definition object
                if(fxProp.Value.Type != JTokenType.Object)
                    throw Error(fxProp.Name, "Function definitions must be objects");
                JObject fx = (JObject)fxProp.Value;

                // Extract parameter count and types
                string[] paramList = {};
                if(!ParseStringList(fx.GetValue(PROPERTY_FUNCTION_PARAMS), ref paramList, false))
                    throw Error(fxProp.Name, "Improper formatting of '" + PROPERTY_FUNCTION_PARAMS + "' property");
                if(paramList.Length == 0)
                    throw Error(fxProp.Name, "All functions must have at least one parameter");
                FxParamType[] parsedParams = new FxParamType[paramList.Length];
                for(int i = 0; i < parsedParams.Length; i++)
                    switch(paramList[i].Trim().ToUpper())
                    {
                        case "PRIMITIVE":
                        parsedParams[i] = FxParamType.PRIMITIVE;
                        break;

                        case "EXPRESSION":
                        parsedParams[i] = FxParamType.EXPRESSION;
                        break;

                        case "REFERENCE":
                        parsedParams[i] = FxParamType.REFERENCE;
                        break;

                        default:
                        throw Error(fxProp.Name, "Invalid parameter type '" + paramList[i] + "'");
                    }

                // Extract definition
                string[] definition = { };
                if(!ParseStringList(fx.GetValue(PROPERTY_FUNCTION_EXP), ref definition, true))
                    throw Error(fxProp.Name, "Improper formatting of '" + PROPERTY_FUNCTION_EXP + "' property");
                
                Evaluator.functions.Add(fxProp.Name, new ExpressionFunction(definition[0], parsedParams));
            }
            preface = "";
        }

        // Extract reserved property list
        // Allows any property (including properties reserved by Parser)
        Dictionary<string, JToken> reservedProps = new Dictionary<string, JToken>();
        foreach(string propName in reservedNames)
        {
            JProperty? p = parsedJson.Property(propName);
            if(p != null)
            {
                reservedProps.Add(propName, p.Value);
                parsedJson.Remove(propName);
            }
        }

        parsedJson.Remove(PROPERTY_STRINGEXP);
        parsedJson.Remove(PROPERTY_FUNCTIONS);
        objectParse(parsedJson);
        return reservedProps;
    }

    private void objectParse(JObject obj)
    {
        foreach(JProperty property in obj.Properties())
        {
            if(property.Name.Length == 0)
                throw Error("", "Property names cannot be empty");

            if(property.Name[0] == '!') // Signal for comment properties
                continue;
            
            foreach(char c in property.Name)
                if (!(c <= 'z' && c >= 'a'))
                if (!(c <= 'Z' && c >= 'A'))
                if (!(c == '_'))
                    throw Error(property.Name, "Invalid character '" + c + "' in property name.");
            
            TokenParse(property.Name, property.Value);
        }
    }

    private void TokenParse(string immediateName, JToken token)
    {
        string fullName = preface + immediateName;
        string oldPreface = "";

        if(vars.ContainsKey(fullName))
            throw Error(immediateName, "This variable is already defined in the Parser's variable dictionary");
        switch (token.Type)
        {
            case JTokenType.Integer:
            vars.AddIntVar(fullName, (int)token);
            break;

            case JTokenType.Float:
            vars.AddDecimalVar(fullName, (double)token);
            break;

            case JTokenType.Boolean:
            vars.AddBoolVar(fullName, (bool)token);
            break;

            case JTokenType.String:
            if(StringExpressions)
                vars.AddExpressionVar(fullName, token.ToString());
            else
                vars.AddStringVar(fullName, token.ToString());
            break;

            case JTokenType.Array:
            JArray list = (JArray)token;
            vars.AddIntVar(fullName, list.Count);
            oldPreface = preface;
            preface = fullName;
            for(int i = 0; i < list.Count; i++)
                TokenParse("[" + i + "]", list[i]);
            preface = oldPreface;
            break;

            case JTokenType.Object:
            parseObjValue();
            oldPreface = preface;
            preface = fullName + '.';
            objectParse((JObject)token);
            preface = oldPreface;
            break;

            default:
                throw Error(immediateName, "The property's value is not a valid type");
        }

        void parseObjValue()
        {
            const string PROPERTY_TYPE = "Type";
            const string PROPERTY_VALUE = "Value";
            JObject obj = (JObject)token;
            JToken? typeToken = obj.GetValue(PROPERTY_TYPE);
            JToken? valueToken = obj.GetValue(PROPERTY_VALUE);

            const string TYPESTRING_STANDARD = "STANDARD";
            const string TYPESTRING_STRING   = "STRING";
            const string TYPESTRING_EXP      = "EXPRESSION";
            string typeString = "";

            // Check Value token
            if(valueToken == null)
            {
                vars.AddStringVar(fullName, "UNDEFINED");
                obj.Remove(PROPERTY_TYPE);
                return;
            }
            if(valueToken.Type == JTokenType.Object)
                throw Error(immediateName, "An object's '" + PROPERTY_VALUE + "' property can't be an object.");

            // Check Type Token
            if(typeToken == null)
                typeString = TYPESTRING_STANDARD;
            else if(typeToken.Type == JTokenType.String)
                typeString = typeToken.ToString().ToUpper();
            else 
                throw Error(immediateName, "An object's '" + PROPERTY_TYPE + "' property must be a string, or undefined");
            
            // Parse Value token according to Type string
            string[] valueList = {};
            switch(typeString)
            {
                case TYPESTRING_STANDARD:
                TokenParse(immediateName, valueToken);
                break;

                case TYPESTRING_STRING:
                if (!ParseStringList(valueToken, ref valueList, true))
                    throw Error(immediateName, "Improper definition of a '" + TYPESTRING_STRING+ "' type");
                vars.AddStringVar(fullName, valueList[0]);
                break;

                case TYPESTRING_EXP:
                if(!ParseStringList(valueToken, ref valueList, true))
                    throw Error(immediateName, "Improper definition of a '" + TYPESTRING_EXP + "' type");
                vars.AddExpressionVar(fullName, valueList[0]);
                break;

                default:
                    throw Error(immediateName, typeString + " is an invalid type");
            }
            obj.Remove(PROPERTY_TYPE);
            obj.Remove(PROPERTY_VALUE);
        }
    }

    private ParserException Error(string immediateName, string msg)
    {
        string fullMessage = String.Format(
            "Failed to parse property '{0}'\n{1}", preface + immediateName, msg
        );
        return new ParserException(fullMessage);
    }

    /// <summary>
    /// Parses a JSON primitive or list of primitives into a string list.
    /// </summary>
    /// <param name="token">Token containing the primitive data</param>
    /// <param name="writeTo">
    /// List to set equal to the data if reading succeeds
    /// </param>
    /// <param name="aggregate">
    /// If true, concatenate all list values into one element
    /// </param>
    /// <returns>
    /// True if reading is successful
    /// False if parsing fails due to invalid data structuring
    /// </returns>
    public static bool ParseStringList(JToken? token, ref string[] writeTo, bool aggregate)
    {
        if (token == null)
            return false;
        
        switch(token.Type)
        {
            case JTokenType.Integer: case JTokenType.Float:
            case JTokenType.String: case JTokenType.Boolean:
            writeTo = new string[] {token.ToString()};
            return true;

            case JTokenType.Array:
            JArray list_token = (JArray)token;
            string[] values = new string[list_token.Count()];

            for(int i = 0; i < values.Length; i++)
            {
                switch (list_token[i].Type)
                {
                    case JTokenType.Integer: case JTokenType.Float:
                    case JTokenType.Boolean: case JTokenType.String:
                    values[i] = list_token[i].ToString();
                    break;

                    default:
                    return false;
                }
            }

            if(aggregate)
            {
                string[] aggregateList = { "" };
                foreach (string s in values)
                    aggregateList[0] += s;
                writeTo = aggregateList;
            }
            else
                writeTo = values;
            return true;

            default:
            return false;
        }
    }
}