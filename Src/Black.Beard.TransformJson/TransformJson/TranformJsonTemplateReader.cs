using Bb.TransformJson.Asts;
using Bb.TransformJson.Parsers;
using Bb.TransformJson.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
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

        public Func<RuntimeContext, JToken, JToken> Get()
        {

            Func<RuntimeContext, JToken, JToken> fnc;
            Expression e;
            Expression<Func<RuntimeContext, JToken, JToken>> lbd;

            if (_root != null)
            {
                var result = Read(_root);

                var builder = new ConfigurationBuilder()
                {
                    Configuration = this._configuration,
                };

                e = result.Accept(builder) as Expression;
                lbd = Expression.Lambda<Func<RuntimeContext, JToken, JToken>>(e, builder.Context, builder.Argument);

            }
            else
            {
                var arg = Expression.Parameter(typeof(RuntimeContext), "arg0");
                var arg1 = Expression.Parameter(typeof(JToken), "arg1");
                lbd = Expression.Lambda<Func<RuntimeContext, JToken, JToken>>(arg1, arg, arg1);
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
            var parser = new StringParser(value);
            var result = parser.Get();
            result = ConvertChildToType(result);
            return result;
        }

        private XsltJson ConvertChildToType(XsltJson node)
        {

            if (node is JPath jp)
            {
                if (jp.Type != "jpath")
                {

                    var service = this._configuration.Services.GetService(jp.Type);
                    if (service == null)
                        throw new MissingServiceException(jp.Type);

                    return new XsltType(jp.TypeObject) { Type = jp.Type, ServiceProvider = service };

                }

                if (jp.Child != null)
                    jp.Child = ConvertChildToType(jp.Child);
            }
            else if (node is XsltType t)
            {

                var service = this._configuration.Services.GetService(t.Type);
                if (service == null)
                    throw new MissingServiceException(t.Type);
                t.ServiceProvider = service;

            }
            return node;

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

            if (result.Source != null)
                if (result.Source is XsltConstant c)
                    if (c.Value is string v)
                    {
                        var service = this._configuration.Services.GetService(v);
                        if (service != null)
                            return new XsltType(result) { ServiceProvider = service };
                        else
                            throw new MissingServiceException(v);
                    }

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
