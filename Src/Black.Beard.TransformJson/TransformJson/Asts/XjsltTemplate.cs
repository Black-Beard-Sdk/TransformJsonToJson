using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.TransformJson.Asts
{

    public class XjsltTemplate
    {

        public XjsltTemplate()
        {

        }

        public Func<RuntimeContext, JToken, JToken> Rules { get; internal set; }

        public RuntimeContext LastExecutionContext { get; private set; }

        public XjsltJson Template { get; internal set; }

        public TranformJsonAstConfiguration Configuration { get; internal set; }

        internal StringBuilder Rule { get; set; }

        public (JToken, RuntimeContext) Transform(StringBuilder payload)
        {
            if (payload.Length > 0)
            {
                JToken obj = JToken.Parse(payload.ToString());
                return Transform(obj);
            }

            return Transform(new JObject());

        }

        public (JToken, RuntimeContext) Transform(JToken obj)
        {
            var ctx = new RuntimeContext()
            {
                TokenSource = obj,
            };
            var result = Rules(ctx, obj);
            return (result, ctx);
        }

        public override string ToString()
        {
            var t = new VisualiserBuilder();
            var o = (JObject)this.Template.Accept(t);
            return o.ToString();
        }

    }



}
