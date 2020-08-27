using Bb.TransformJson.Asts;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Bb.TransformJson
{

    public class TemplateTransformProvider
    {

        public TemplateTransformProvider(TranformJsonAstConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public TranformJsonAstTree GetTemplate(StringBuilder sb)
        {

            JObject obj = null;

            if (sb.Length > 0)
                obj = JObject.Parse(sb.ToString());

            TranformJsonTemplateReader reader = new TranformJsonTemplateReader(obj, this._configuration);

            TranformJsonAstTree result = new TranformJsonAstTree()
            {
                Rules = reader.Get()
            };

            return result;

        }

        private TranformJsonAstConfiguration _configuration;

    }


}
