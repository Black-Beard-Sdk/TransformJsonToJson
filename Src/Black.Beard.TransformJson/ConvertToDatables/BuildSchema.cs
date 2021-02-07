using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Bb.ConvertToDatables
{

    public class BuildSchema
    {

        public BuildSchema()
        {

        }

        public Parser ParseTemplate(JToken schema)
        {
            var p = new Parser(new DataSet(), null);
            if (schema is JObject o)
                ParseObjectProperties(o, p);

            p.Initialize();

            return p;
        }

        private void ParseObjectProperties(JObject o, Parser parser)
        {

            foreach (var item in o.Properties())
            {
                if (item.Value is JObject oo)
                {
                    if (oo.Properties().Any(c => c.Name == "$path"))
                        ParseMapping(parser, item, oo);

                    else
                    {
                        var parser2 = parser.AddSub(item.Name);
                        ParseObjectProperties(oo, parser2);
                    }
                }
                else if (item.Value is JArray arr)
                {

                    if (item.Name.ToLower() == "$new")
                    {
                        foreach (var item3 in arr)
                            parser.AppendNewLine(item3.ToString());
                    }
                    else
                    {
                        var parser2 = parser.AddSub(item.Name);
                        parser2.IsArray = true;

                        var item1 = arr[0];
                        if (item1 is JObject oo2)
                        {
                            ParseObjectProperties(oo2, parser2);
                        }
                        else
                        {

                        }
                    }
                }

            }

        }

        private static void ParseMapping(Parser parser, JProperty item, JObject oo)
        {
            var property = parser.AddProperty(item.Name);
            DataColumn column = null;
            Type type = typeof(object);
            bool nullable = true;
            bool isUnique = false;
            int maxLength = -1;
            object defaultValue = null;

            foreach (var item2 in oo.Properties())
            {
                switch (item2.Name)
                {

                    case "$path":
                        column = property.GetTargetPath(item2.Value as JValue, item.Name);
                        break;

                    case "$type":
                        type = Type.GetType(item2.Value.ToString());
                        if (type == null)
                            throw new InvalidExpressionException($"the type cant't be solved from '{item2.Value}'");
                        break;

                    case "$nullable":
                        nullable = item2.Value<bool>();
                        break;

                    case "$isunique":
                        isUnique = item2.Value<bool>();
                        break;

                    case "$maxlength":
                        maxLength = item2.Value<int>();
                        break;

                    case "$defaultvalue":
                        defaultValue = item2.Value<object>();
                        break;

                    default:
                        break;
                }
            }

            column.DataType = type;
            column.AllowDBNull = nullable;
            column.Unique = isUnique;
            if (maxLength > -1)
                column.MaxLength = maxLength;

            if (defaultValue != null)
                column.DefaultValue = Convert.ChangeType(defaultValue, type);
        }
    
    }

}
