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

        public XjsltJson Tree()
        {
            if (_root != null)
                return Read(_root);
            return null;
        }

        public Func<RuntimeContext, JToken, JToken> Get(XjsltJson tree)
        {

            Func<RuntimeContext, JToken, JToken> fnc;

            if (tree != null)
            {
                var builder = new ConfigurationBuilder() { Configuration = this._configuration, };
                fnc = builder.Compile(tree);
            }
            else // Template empty
            {
                var arg = Expression.Parameter(typeof(RuntimeContext), "arg0");
                var arg1 = Expression.Parameter(typeof(JToken), "arg1");
                var lbd = Expression.Lambda<Func<RuntimeContext, JToken, JToken>>(arg1, arg, arg1);

                if (lbd.CanReduce)
                    lbd.ReduceAndCheck();

                fnc = lbd.Compile();
            }

            return fnc;

        }

        private XjsltJson Read(JToken n)
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

        private XjsltJson ReadTimeSpan(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.TimeSpan };
        }

        private XjsltJson ReadUri(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Uri };
        }

        private XjsltJson ReadGuid(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Guid };
        }

        private XjsltJson ReadBytes(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Bytes };
        }

        private XjsltJson ReadDate(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Date };
        }

        private XjsltJson ReadNull(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Null };
        }

        private XjsltJson ReadBoolean(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Boolean };
        }

        private XjsltJson ReadString(JValue n)
        {
            var value = n.Value?.ToString();
            var parser = new StringParser(value);
            var result = parser.Get();
            result = ConvertChildToType(result);
            return result;
        }

        private XjsltJson ConvertChildToType(XjsltJson node)
        {

            if (node is XjPath jp)
            {
                if (jp.Type != "jpath")
                {

                    var service = this._configuration.Services.GetService(jp.Type);
                    if (service == null)
                        throw new MissingServiceException(jp.Type);

                    return new XjsltType(jp.TypeObject) { Type = jp.Type, ServiceProvider = service };

                }

                if (jp.Child != null)
                    jp.Child = ConvertChildToType(jp.Child);
            }
            else if (node is XjsltType t)
            {

                var service = this._configuration.Services.GetService(t.Type);
                if (service == null)
                    throw new MissingServiceException(t.Type);
                t.ServiceProvider = service;

            }
            return node;

        }

        private XjsltJson ReadFloat(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Float };
        }

        private XjsltJson ReadInteger(JValue n)
        {
            return new XjsltConstant() { Value = n.Value, Kind = XjsltKind.Integer };
        }

        private XjsltJson ReadConstructor(JConstructor n)
        {
            throw new NotImplementedException();
        }

        private XjsltJson ReadArray(JArray n)
        {

            var arr = new XjsltArray(n.Count);

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

                if (it.Where != null)
                {
                    arr.Where = it.Where;
                    it.Where = null;
                }

            }

            return arr;

        }

        private XjsltJson ReadObject(JObject n)
        {

            string name = string.Empty;

            var result = new XjsltObject() { Name = name };

            foreach (var item in n.Properties())
                result.Append(Read(item) as XjsltProperty);

            if (result.Source != null)
                if (result.Source is XjsltConstant c)
                    if (c.Value is string v)
                    {
                        var service = this._configuration.Services.GetService(v);
                        if (service != null)
                            return new XjsltType(result) { ServiceProvider = service };
                        else
                            throw new MissingServiceException(v);
                    }

            if (result.Where != null)
                if (result.Where is XjsltConstant c)
                    if (c.Value is string v)
                    {
                        var service = this._configuration.Services.GetService(v);
                        if (service != null)
                            return new XjsltType(result) { ServiceProvider = service };
                        else
                            throw new MissingServiceException(v);
                    }

            return result;

        }

        private XjsltJson ReadProperty(JProperty n)
        {

            var name = n.Name;

            var result = new XjsltProperty()
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
