using Bb.TransformJson.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Bb.TransformJson
{
    public class RuntimeContext
    {

        static RuntimeContext()
        {
            _getContentByJPath = RuntimeContext.GetContentByJPath;
            _getProjectionFromSource = RuntimeContext.GetProjectionFromSource;
            _getContentFromService = RuntimeContext.GetContentFromService;
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

        
    }

}
