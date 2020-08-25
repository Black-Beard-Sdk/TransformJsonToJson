using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Bb.TransformJson
{
    public class RuntimeContext
    {

        static RuntimeContext()
        {
            _getContentByName = RuntimeContext.GetContentByName;
            _getProjectionFromSource = RuntimeContext.GetProjectionFromSource;
        }

        public static JToken GetContentByName(JToken token, string path)
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

        public static JToken GetProjectionFromSource(JToken token, Func<JToken, JToken> delegateObject)
        {

            if (token != null)
            {

                var arr = new JArray();

                if (token is JArray a)
                    foreach (var item in a)
                    {
                        var i = delegateObject(item);
                        arr.Add(i);
                    }

                else if (token is JObject o)
                {
                    var i = delegateObject(o);
                    arr.Add(i);
                }
                else
                {

                }

                return arr;

            }

            return null;

        }

        public static readonly Func<JToken, Func<JToken, JToken>, JToken> _getProjectionFromSource;
        public static readonly Func<JToken, string, JToken> _getContentByName;

    }

}
