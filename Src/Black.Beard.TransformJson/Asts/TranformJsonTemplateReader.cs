//using Bb.TransformJson.Parser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Bb.TransformJson
{
    public class TranformJsonTemplateReader
    {

        public TranformJsonTemplateReader(JToken root, TranformJsonAstConfiguration configuration)
        {
            this._root = root;
            this._configuration = configuration;
        }

        public Func<JToken, JToken> Get()
        {

            Func<JToken, JToken> fnc;
            Expression e;
            Expression<Func<JToken, JToken>> lbd;

            if (_root != null)
            {
                var result = Read(_root);

                var builder = new ConfigurationBuilder()
                {
                    Configuration = this._configuration,
                };

                e = result.Accept(builder) as Expression;
                lbd = Expression.Lambda<Func<JToken, JToken>>(e, builder.Argument);

            }
            else
            {
                var arg = Expression.Parameter(typeof(JToken), "arg0");
                lbd = Expression.Lambda<Func<JToken, JToken>>(arg, arg);
            }

            fnc = lbd.Compile();

            return fnc;

        }

        private XsltJson Read(JToken n)
        {

            switch (n.Type)
            {

                case JTokenType.Object:
                    return ReadObject(n as JObject);

                case JTokenType.Array:
                    return ReadArray(n as JArray);

                case JTokenType.Constructor:
                    return ReadConstructor(n as JConstructor);

                case JTokenType.Property:
                    return ReadProperty(n as JProperty);

                case JTokenType.Integer:
                    return ReadInteger(n as JValue);

                case JTokenType.Float:
                    return ReadFloat(n as JValue);

                case JTokenType.String:
                    return ReadString(n as JValue);

                case JTokenType.Boolean:
                    return ReadBoolean(n as JValue);

                case JTokenType.Null:
                    return ReadNull(n as JValue);

                case JTokenType.Date:
                    return ReadDate(n as JValue);

                case JTokenType.Bytes:
                    return ReadBytes(n as JValue);

                case JTokenType.Guid:
                    return ReadGuid(n as JValue);

                case JTokenType.Uri:
                    return ReadUri(n as JValue);

                case JTokenType.TimeSpan:
                    return ReadTimeSpan(n as JValue);

                case JTokenType.Comment:
                case JTokenType.None:
                case JTokenType.Undefined:
                case JTokenType.Raw:
                default:
                    break;
            }

            throw new System.NotImplementedException();

        }

        private XsltJson ReadTimeSpan(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.TimeSpan };
        }

        private XsltJson ReadUri(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Uri };
        }

        private XsltJson ReadGuid(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Guid };
        }

        private XsltJson ReadBytes(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Bytes };
        }

        private XsltJson ReadDate(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Date };
        }

        private XsltJson ReadNull(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Null };
        }

        private XsltJson ReadBoolean(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Boolean };
        }

        private XsltJson ReadString(JValue n)
        {

            var value = n.Value?.ToString();

            Regex reg = new Regex(@"xpath:\{[^}]*");

            var o = reg.Matches(value);

            if (o.Count > 0)
            {

                XPath first = null;

                //for (int i = o.Count - 1; i >= 0; i--)
                //    first = new XPath() { Value = o[i].Value.Substring(7), Kind = XsltKind.Xpath, Child = first };

                for (int i = 0; i < o.Count; i++)
                    first = new XPath() { Value = o[i].Value.Substring(7), Kind = XsltKind.Xpath, Child = first };

                return first;

            }

            return new XsltConstant() { Value = value, Kind = XsltKind.String };

        }

        private XsltJson ReadFloat(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Float };
        }

        private XsltJson ReadInteger(JValue n)
        {
            return new XsltConstant() { Value = n.Value, Kind = XsltKind.Integer };
        }

        private XsltJson ReadConstructor(JConstructor n)
        {
            throw new NotImplementedException();
        }

        private XsltJson ReadArray(JArray n)
        {

            var arr = new XsltArray(n.Count);

            foreach (var item in n)
            {

                var it = arr.Item = Read(item);
                if (it.Source != null)
                {
                    arr.Source = it.Source;
                    it.Source = null;
                }
                else
                    throw new MissingFieldException("$source");

            }

            return arr;

        }

        private XsltJson ReadObject(JObject n)
        {

            string name = string.Empty;

            var result = new XsltObject() { Name = name };

            foreach (var item in n.Properties())
                result.Append(Read(item) as XsltProperty);


            if (result.IsType)
                return new XsltType(result);

            return result;

        }

        private XsltJson ReadProperty(JProperty n)
        {

            var name = n.Name;

            var result = new XsltProperty()
            {
                Name = name,
                Value = Read(n.Value)
            };

            return result;

        }


        private readonly JToken _root;
        private readonly TranformJsonAstConfiguration _configuration;
        //private Stack<object> _path;

    }



}
