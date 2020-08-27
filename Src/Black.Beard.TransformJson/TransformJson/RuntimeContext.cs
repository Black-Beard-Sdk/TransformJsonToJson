using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.TransformJson
{
    public class RuntimeContext
    {

        static RuntimeContext()
        {
            _getContentByJPath = RuntimeContext.GetContentByJPath;
            _getProjectionFromSource = RuntimeContext.GetProjectionFromSource;
            _getContentFromService = RuntimeContext.GetContentFromService;
            _mapPropertyService = RuntimeContext.MapPropertyService;
            _properties = new Dictionary<Type, Dictionary<string, (PropertyInfo, Action<object, object>)>>();

        }


        public static ITransformJsonService MapPropertyService(RuntimeContext ctx, ITransformJsonService service, string propertyName, JToken value)
        {

            (PropertyInfo, Action<object, object>) writer = GetWriter(service.GetType(), propertyName);

            if (writer.Item1 != null)
            {

                var type = writer.Item1.PropertyType;
                if (type == typeof(string))
                    writer.Item2(service, value.ToString());

                else if (type.IsValueType)
                    writer.Item2(service, Convert.ChangeType(value.ToString(), type));
                
                else if(value is JObject o)
                    writer.Item2(service, o.ToObject(type));
                
                else
                {

                }
            }

            return service;

        }

        private static (PropertyInfo, Action<object, object>) GetWriter(Type componentType, string propertyName)
        {

            if (!_properties.TryGetValue(componentType, out Dictionary<string, (PropertyInfo, Action<object, object>)> properties))
                lock (_lock)
                    if (!_properties.TryGetValue(componentType, out properties))
                        _properties.Add(componentType, properties = new Dictionary<string, (PropertyInfo, Action<object, object>)>());

            if (!properties.TryGetValue(propertyName, out (PropertyInfo, Action<object, object>) action))
                lock (_lock)
                    if (!properties.TryGetValue(propertyName, out action))
                    {
                        var ___properties = componentType.GetProperties().Where(c => c.Name.ToLower() == propertyName.ToLower()).ToList();
                        var property = ___properties.Count ==1 
                            ? ___properties[0]
                            : ___properties.Single(c => c.Name == propertyName)
                            ;

                        if (property != null && property.CanWrite)
                        {
                            var m = property.GetMethod ?? property.SetMethod;
                            var isStatic = m != null ? (m.Attributes & MethodAttributes.Static) == MethodAttributes.Static : false;
                            var targetObjectParameter = Expression.Parameter(typeof(object), "i");
                            var convertedObjectParameter = Expression.ConvertChecked(targetObjectParameter, componentType);
                            var valueParameter = Expression.Parameter(typeof(object), "value");
                            var convertedValueParameter = Expression.ConvertChecked(valueParameter, property.PropertyType);
                            var propertyExpression = Expression.Property(isStatic ? null : convertedObjectParameter, property);

                            var assignValue = Expression.Lambda<Action<object, object>>
                            (
                                Expression.Assign
                                (
                                    propertyExpression,
                                    convertedValueParameter
                                ),
                                targetObjectParameter,
                                valueParameter
                            ).Compile();

                            properties.Add(propertyName, action = (property, assignValue));

                        }
                        else
                        {
                            properties.Add(propertyName, action = (property, (arg1, arg2) => { }));
                        }

                    }

            return action;

        }

        // ITransformJsonService
        public static JToken GetContentFromService(RuntimeContext ctx, JToken token, ITransformJsonService service)
        {
            return service.Execute(ctx, token);
        }

        public static JToken GetContentByJPath(RuntimeContext ctx, JToken token, string path)
        {

            JToken result = null;

            if (token != null)
            {

                if (token is JObject o)
                {
                    var h = o.SelectTokens(path).ToList();
                    if (h.Count == 1)
                        result = h[0];

                    else
                        result = new JArray(h);
                }

                else if (token is JArray a)
                {

                    var h = a.SelectTokens(path).ToList();
                    if (h.Count == 1)
                        result = h[0];

                    else
                        result = new JArray(h);

                }
                else if (token is JValue v)
                    return null;
                else
                {

                }

            }

            return result;

        }

        public static JToken GetProjectionFromSource(RuntimeContext ctx, JToken token, Func<RuntimeContext, JToken, JToken> delegateObject)
        {

            if (token != null)
            {

                var arr = new JArray();

                if (token is JArray a)
                    foreach (var item in a)
                    {
                        var i = delegateObject(ctx, item);
                        arr.Add(i);
                    }

                else if (token is JObject o)
                {
                    var i = delegateObject(ctx, o);
                    arr.Add(i);
                }
                else
                {

                }

                return arr;

            }

            return null;

        }

        public static readonly Func<RuntimeContext, JToken, Func<RuntimeContext, JToken, JToken>, JToken> _getProjectionFromSource;
        public static readonly Func<RuntimeContext, JToken, string, JToken> _getContentByJPath;
        public static readonly Func<RuntimeContext, JToken, ITransformJsonService, JToken> _getContentFromService;
        public static readonly Func<RuntimeContext, ITransformJsonService, string, JToken, ITransformJsonService> _mapPropertyService;
        private static readonly Dictionary<Type, Dictionary<string, (PropertyInfo, Action<object, object>)>> _properties;

        private static object _lock = new object();

    }

}
