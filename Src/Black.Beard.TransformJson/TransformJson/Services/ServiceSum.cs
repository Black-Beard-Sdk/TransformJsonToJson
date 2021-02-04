using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Bb.TransformJson.Services
{

    /// <summary>
    /// return the sum of the terms values
    /// if one of the term is not a number
    /// </summary>
    [DisplayName("sum")]
    public class ServiceSum : ITransformJsonService
    {

        public ServiceSum()
        {

        }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            double result = 0;
            if (token != null)
            {

                JTokenType type = JTokenType.None;

                if (token is JArray a && a.Count > 0)
                {

                    foreach (var item in a)
                    {

                        if (item.Type == JTokenType.Integer)
                        {
                            int i = item.Value<int>();
                            result += i;
                        }
                        else if (item.Type == JTokenType.Float)
                        {
                            float i = item.Value<float>();
                            result += i;
                        }
                        else
                            throw new InvalidOperationException("Interger or float expected value");

                    }

                }

            }

            var r = (int)result;
            if (r < result)
                return new JValue(result);

            return new JValue(r);

        }

    }

}
