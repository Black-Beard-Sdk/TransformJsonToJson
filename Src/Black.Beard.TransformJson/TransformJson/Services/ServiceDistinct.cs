﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Bb.TransformJson.Services
{

    /// <summary>
    /// return a list of key with no duplicated values
    /// Display name is the key used in the template
    /// </summary>
    [DisplayName("distinct")]
    public class ServiceDistinct : ITransformJsonService
    {

        public ServiceDistinct()
        {

        }

        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            if (token != null && token is JValue j)
            {

                if (_index.Add(j.Value))
                    return new JValue(true);

            }

            return new JValue(false);

        }

        private HashSet<object> _index = new HashSet<object>();

    }

}
