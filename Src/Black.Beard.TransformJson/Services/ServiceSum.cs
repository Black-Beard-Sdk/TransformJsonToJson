using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Bb.TransformJson.Services
{


    public class ServiceSum : ITransformJsonService
    {

        public JToken Execute(RuntimeContext ctx, JToken token)
        {
            if (token != null)
            {

                List<int> arr = new List<int>();

                if (token is JArray a)
                    foreach (var item in a)
                    {
                        var i = item.Value<int>();
                        arr.Add(i);
                    }

                else
                {

                }

                return new JValue(arr.Sum());

            }

            return null;

        }


    }


}
