using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bb.TransformJson.Services
{
    /// <summary>
    /// return a list of key with no duplicated values
    /// Display name is the key used in the template
    /// </summary>
    [DisplayName("notnull")]
    public class ServiceNotNull : ITransformJsonService
    {

        public ServiceNotNull()
        {

        }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            if (token != null && token is JValue j)
            {
                return new JValue(j.Value != null);
            }

            return new JValue(true);

        }

        private HashSet<object> _index = new HashSet<object>();

    }

}
