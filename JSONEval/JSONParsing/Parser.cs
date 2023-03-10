namespace JSONEval.JSONParsing;
using JSONEval.ExpressionEvaluation;
using Newtonsoft.Json.Linq;
class Parser
{
    /// <summary>
    /// Parsed variables are saved to this dictionary
    /// </summary>
    public VarDictionary vars {get; set;} = new VarDictionary();

    /// <summary>
    /// If true, interpret JSON string properties as expressions by default.
    /// If false, interpret them as string literals
    /// </summary>
    public bool stringsAreExpressions {get; set;} = true;

    private string preface = "";

    public void Parse(string rawJson)
    {
        Parse(rawJson, new string[0]);
    }

    public List<JProperty> Parse(string rawJson, string[] reservedNames)
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
        catch(Newtonsoft.Json.JsonReaderException)
        {
            throw new Exception();
        }


        // TODO: Default String form interpretation
        const string PROPERTY_STRINGEXP = "StringExpressions";
        JToken? stringExpToken = parsedJson.GetValue(PROPERTY_STRINGEXP);
        if(stringExpToken != null)
        {
            if(stringExpToken.Type != JTokenType.Boolean)
                throw new Exception();
            stringsAreExpressions = (bool)stringExpToken;
        }

        // TODO: Parse Function definition property here
        const string PROPERTY_FUNCTIONS = "Functions";
        const string PROPERTY_FUNCTION_PARAMS = "Parameters";
        const string PROPERTY_FUNCTION_EXP = "Definition";
        JToken? fxToken = parsedJson.GetValue(PROPERTY_FUNCTIONS);
        if(fxToken != null)
        {
            // Note: use preface system for error messages?
            if(fxToken.Type != JTokenType.Object)
                throw new Exception();
            foreach(JProperty fxProp in fxToken)
            {
                if(fxProp.Type != JTokenType.Object)
                    throw new Exception();
                JObject fx = (JObject)fxProp.Value;

                string[] paramList = {};
                string[] definition = {};

                if(!ParseStringList(fx.GetValue(PROPERTY_FUNCTION_PARAMS), ref paramList, false))
                    throw new Exception();
                
                FxParamType[] parsedParams = new FxParamType[paramList.Length];
                for(int i = 0; i < parsedParams.Length; i++)
                    switch(paramList[i].ToUpper())
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
                        throw new Exception();
                    } 
                
                if(!ParseStringList(fx.GetValue(PROPERTY_FUNCTION_EXP), ref definition, true))
                    throw new Exception();
                
                // Duplicate function name check needed
                Evaluator.functions.Add(fxProp.Name, new ExpressionFunction(definition[0], parsedParams));
            }
        }

        // TODO: Extract reserved property list
        // Should allow any property (including properties reserved by function)
        List<JProperty> reservedProps = new List<JProperty>();
        foreach(string prop in reservedNames)
        {
            JProperty? p = parsedJson.Property(prop);
            if(p != null)
            {
                reservedProps.Add(p);
                parsedJson.Remove(prop);
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
                throw new Exception();

            if(property.Name[0] == '!') // Signal for comment properties
                continue;
            
            foreach(char c in property.Name)
                if (!(c <= 'z' && c >= 'a'))
                if (!(c <= 'Z' && c >= 'A'))
                if (!(c == '_'))
                    throw new Exception();
            
            TokenParse(property.Name, property.Value);
        }
    }

    private void TokenParse(string immediateName, JToken token)
    {
        string fullName = preface + immediateName;
        string oldPreface = "";
        switch (token.Type) // TODO: ADD TRY-CATCH FOR DUPLICATE VARIABLES (OR JUST DO IF-STATEMENT CHECK???)
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
            if(stringsAreExpressions)
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

            case JTokenType.Object: // Parse Type/Value here to eliminate conditional check
            parseObjValue();
            oldPreface = preface;
            preface = fullName + '.';
            objectParse((JObject)token);
            preface = oldPreface;
            break;

            default:
                throw new Exception();
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
                throw new Exception();

            // Check Type Token
            if(typeToken == null)
                typeString = TYPESTRING_STANDARD;
            else if(typeToken.Type == JTokenType.String)
                typeString = typeToken.ToString().ToUpper();
            else 
                throw new Exception();
            
            // Parse Value token according to Type string
            string[] valueList = {};
            switch(typeString)
            {
                case TYPESTRING_STANDARD:
                TokenParse(immediateName, valueToken);
                break;

                case TYPESTRING_STRING:
                if (!ParseStringList(valueToken, ref valueList, true))
                    throw new Exception();
                vars.AddStringVar(fullName, valueList[0]);
                break;

                case TYPESTRING_EXP:
                if(!ParseStringList(valueToken, ref valueList, true))
                    throw new Exception();
                vars.AddExpressionVar(fullName, valueList[0]);
                break;

                default:
                    throw new Exception();
            }
            obj.Remove(PROPERTY_TYPE);
            obj.Remove(PROPERTY_VALUE);
        }
    }

    private ParserException Error(string msg)
    {
        return new ParserException(msg);
    }

    /// <summary>
    /// Parses a JSON primitive or list of primitives into a string list.
    /// </summary>
    /// <param name="token">Token containing the primitive data</param>
    /// <param name="writeTo">
    /// List to set equal to the data if reading is successful
    /// </param>
    /// <param name="aggregate">
    /// If true, concatenate all list values into one element
    /// </param>
    /// <returns>
    /// True if reading is successful
    /// False if parsing fails due to invalid data structuring
    /// </returns>
    private static bool ParseStringList(JToken? token, ref string[] writeTo, bool aggregate)
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

class ParserException : Exception
{
    public ParserException(string msg) : base(msg) {}
}