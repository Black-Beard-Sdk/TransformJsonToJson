using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bb.TransformJson.Services
{
    /// <summary>
    /// return the divided result of the value by the part value
    /// </summary>
    [DisplayName("div")]
    public class ServiceDiv : ITransformJsonService
    {

        public ServiceDiv()
        {

        }

        public float Value { get; set; }

        public float Part { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            var value = Value / Part;
            var v = (int)value;

            if (v < value)
                return new JValue(value);

            return new JValue(v);

        }

    }



}
