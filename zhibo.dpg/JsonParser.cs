using System;
using System.Collections.Generic;
using System.Reflection;

public class SimpleJsonParser
{
    private readonly string _json;
    private int _position;

    public SimpleJsonParser(string json)
    {
        _json = json;
    }

    private char Peek()
    {
        if (_position >= _json.Length) return '\0';
        return _json[_position];
    }

    private char Read()
    {
        if (_position >= _json.Length) return '\0';
        return _json[_position++];
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(Peek())) Read();
    }

    public object ParseValue()
    {
        SkipWhitespace();
        var ch = Peek();

        if (ch == '"') return ParseString();
        if (ch == '{') return ParseObject();
        if (ch == '[') return ParseArray();
        if (char.IsDigit(ch) || ch == '-') return ParseNumber();
        if (ch == 't' || ch == 'f') return ParseBool();
        if (ch == 'n') return ParseNull();

        throw new Exception($"Unexpected character '{ch}' at position {_position}");
    }

    private string ParseString()
    {
        var start = ++_position;

        while (true)
        {
            var ch = Read();
            if (ch == '\\') _position++;  // Skip escaped characters
            else if (ch == '"') break;
        }

        var len = _position - start - 1;
        return _json.Substring(start, len);
    }

    private Dictionary<string, object> ParseObject()
    {
        var result = new Dictionary<string, object>();

        Read();  // '{'

        while (true)
        {
            SkipWhitespace();
            if (Peek() == '}') break;

            var key = ParseString();

            SkipWhitespace();
            Read();  // ':'

            var value = ParseValue();
            result[key] = value;

            SkipWhitespace();
            if (Peek() == '}') break;
            Read();  // ','
        }

        Read();  // '}'
        return result;
    }

    private List<object> ParseArray()
    {
        var result = new List<object>();

        Read();  // '['

        while (true)
        {
            SkipWhitespace();
            if (Peek() == ']') break;

            var value = ParseValue();
            result.Add(value);

            SkipWhitespace();
            if (Peek() == ']') break;
            Read();  // ','
        }

        Read();  // ']'
        return result;
    }

    private object ParseNumber()
    {
        var start = _position;

        while (char.IsDigit(Peek()) || Peek() == '-') Read();

        var len = _position - start;
        var numStr = _json.Substring(start, len);

        if (int.TryParse(numStr, out var intVal)) return intVal;
        if (long.TryParse(numStr, out var longVal)) return longVal;
        return double.Parse(numStr);  // you might want to handle floats, decimals...
    }

    private bool ParseBool()
    {
        if (_json.Substring(_position, 4) == "true")
        {
            _position += 4;
            return true;
        }

        if (_json.Substring(_position, 5) == "false")
        {
            _position += 5;
            return false;
        }

        throw new Exception($"Unexpected token at position {_position}");
    }

    private object ParseNull()
    {
        if (_json.Substring(_position, 4) == "null")
        {
            _position += 4;
            return null;
        }

        throw new Exception($"Unexpected token at position {_position}");
    }

    public static T DeserializeObject<T>(string json) where T : new()
    {
        var parser = new SimpleJsonParser(json);
        var result = parser.ParseValue();

        if (result is Dictionary<string, object> dict)
        {
            return MapTo<T>(dict);
        }

        throw new Exception("Expected JSON object");
    }

    private static T MapTo<T>(Dictionary<string, object> dict) where T : new()
    {
        var obj = new T();
        var type = typeof(T);

        foreach (var kv in dict)
        {
            var prop = type.GetProperty(kv.Key);
            if (prop != null && prop.CanWrite)
            {
                if (kv.Value is Dictionary < string, object> subDict)
                {
                    var valueType = prop.PropertyType;
                    var valueMethod = typeof(SimpleJsonParser).GetMethod("MapTo", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(valueType);
                    var valueObj = valueMethod.Invoke(null, new object[] { subDict });
                    prop.SetValue(obj, valueObj);
                }
                else if (kv.Value is List<object> list)
                {
                    var elementType = prop.PropertyType.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    var newList = (List<object>)Activator.CreateInstance(listType);

                    var addMethod = newList.GetType().GetMethod("Add");

                    foreach (var item in list)
                    {
                        if (item is Dictionary<string, object> listItemDict)
                        {
                            var itemMethod = typeof(SimpleJsonParser).GetMethod("MapTo", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(elementType);
                            var itemObj = itemMethod.Invoke(null, new object[] { listItemDict });
                            addMethod.Invoke(newList, new[] { itemObj });
                        }
                        else
                        {
                            addMethod.Invoke(newList, new[] { item });
                        }
                    }

                    prop.SetValue(obj, newList);
                }
                else
                {
                    // Handle enum properties
                    if (prop.PropertyType.IsEnum && kv.Value is int intValue)
                    {
                        prop.SetValue(obj, Enum.ToObject(prop.PropertyType, intValue));
                    }
                    else
                    {
                        prop.SetValue(obj, Convert.ChangeType(kv.Value, prop.PropertyType));
                    }
                }
            }
        }

        return obj;
    }
}

