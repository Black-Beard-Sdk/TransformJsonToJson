using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bb.TransformJson.Services
{

    /// <summary>
    /// return a string with all values concatenated
    /// </summary>
    [DisplayName("concat")]
    public class ServiceConcat : ITransformJsonService
    {

        public ServiceConcat()
        {

        }

        public string Arg1 { get; set; }

        public string Arg2 { get; set; }

        public string Arg3 { get; set; }

        public string Arg4 { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {
            return new JValue(string.Concat(Arg1 ?? string.Empty, Arg2 ?? string.Empty, Arg3 ?? string.Empty, Arg4 ?? string.Empty));
        }

    }

}
