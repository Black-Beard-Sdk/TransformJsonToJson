using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bb.TransformJson.Services
{
    /// <summary>
    /// return the multiplication product of left and right values
    /// </summary>
    [DisplayName("time")]
    public class ServiceTime : ITransformJsonService
    {

        public ServiceTime()
        {

        }

        public double Left { get; set; }

        public double Right { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            var value = Left * Right;
            var v = (int)value;

            if (v < value)
                return new JValue(value);

            return new JValue(v);

        }

    }



}
