﻿using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace Bb.TransformJson.Services
{
    /// <summary>
    /// return the cancatenated text of all the the terms.
    /// </summary>
    [DisplayName("concatall")]
    public class ServiceConcatAll : ITransformJsonService
    {

        public ServiceConcatAll()
        {

        }

        public string SplitChar { get; set; } = string.Empty;

        public string DelimitChar { get; set; } = string.Empty;


        public JToken Execute(RuntimeContext ctx, JToken token)
        {

            StringBuilder sb = new StringBuilder();

            if (token != null)
            {

                if (token is JArray a && a.Count > 0)
                {

                    bool t = false;
                    foreach (var item in a)
                    {

                        if (t)
                        {
                            sb.Append(SplitChar);
                            sb.Append(DelimitChar);
                        }

                        string i = item.Value<string>();
                        sb.Append(i);

                        if (t)
                        {
                            sb.Append(DelimitChar);
                        }

                        t = true;

                    }

                }

            }

            return new JValue(sb.ToString());

        }

    }

}
