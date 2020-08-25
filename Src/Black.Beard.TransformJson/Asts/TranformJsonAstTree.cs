using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.TransformJson
{

    public class TranformJsonAstTree
    {

        public TranformJsonAstTree()
        {
           
        }

        public Func<RuntimeContext, JToken, JToken> Rules { get; internal set; }

        public JToken Transform(StringBuilder payload)
        {
            JObject obj = JObject.Parse(payload.ToString());
            RuntimeContext ctx = new RuntimeContext();
            return Rules(ctx, obj);
        }


    }



}
