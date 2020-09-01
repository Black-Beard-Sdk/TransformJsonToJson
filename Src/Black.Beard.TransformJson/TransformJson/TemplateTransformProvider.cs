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

        public XjsltTemplate GetTemplate(StringBuilder sb)
        {

            JObject obj = null;

            if (sb.Length > 0)
                obj = JObject.Parse(sb.ToString());

            TranformJsonTemplateReader reader = new TranformJsonTemplateReader(obj, this._configuration);

            var tree = reader.Tree();
            XjsltTemplate result = new XjsltTemplate()
            {
                Rule = sb,
                Configuration = this._configuration,
                Template = tree,
                Rules = reader.Get(tree)
            };

            return result;

        }

        private TranformJsonAstConfiguration _configuration;

    }


}
