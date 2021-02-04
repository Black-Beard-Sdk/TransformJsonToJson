using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bb.TransformJson.Services
{
    /// <summary>
    /// return the rest of the division the value by the part value
    /// </summary>
    [DisplayName("modulo")]
    public class ServiceModulo : ITransformJsonService
    {

        public ServiceModulo()
        {

        }

        public float Value { get; set; }

        public float Part { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            var value = Value % Part;
            var v = (int)value;

            if (v < value)
                return new JValue(value);

            return new JValue(v);

        }

    }



}
