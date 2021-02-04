using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Bb.TransformJson.Services
{

    /// <summary>
    /// return the sum of the left and right terms values
    /// </summary>
    [DisplayName("add")]
    public class ServiceAdd : ITransformJsonService
    {

        public ServiceAdd()
        {

        }

        public float Left { get; set; }

        public float Right { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            var value = Left + Right;
            var v = (int)value;

            if (v < value)
                return new JValue(value);

            return new JValue(v);

        }

    }

}
