using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bb.TransformJson.Services
{
    /// <summary>
    /// return a sub part of the initial Text from the 'start' position and from length 'Lenght'
    /// </summary>
    [DisplayName("subString")]
    public class ServiceSubStr : ITransformJsonService
    {

        public ServiceSubStr()
        {

        }

        public string Text { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {
            return new JValue( Text.Substring(Start, Length) );
        }

    }

}
