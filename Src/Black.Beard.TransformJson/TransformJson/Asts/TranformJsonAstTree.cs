using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.TransformJson.Asts
{

    public class TranformJsonAstTree
    {

        public TranformJsonAstTree()
        {
           
        }

        public Func<RuntimeContext, JToken, JToken> Rules { get; internal set; }

        public RuntimeContext LastExecutionContext { get; private set; }

        public JToken Transform(StringBuilder payload)
        {
            JObject obj = JObject.Parse(payload.ToString());
            this.LastExecutionContext = new RuntimeContext();
            return Rules(this.LastExecutionContext, obj);
        }


    }



}
