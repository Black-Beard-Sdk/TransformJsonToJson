using Bb.Exceptions;
using Bb.TransformJson.Asts;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Bb.TransformJson
{

    public class TemplateTransformProvider
    {

        public TemplateTransformProvider(TranformJsonAstConfiguration configuration = null)
        {
            
            if (configuration == null)
                configuration = new TranformJsonAstConfiguration();

            this._configuration = configuration;

        }

        public XjsltTemplate GetTemplate(StringBuilder sb)
        {

            JToken obj = null;

            for (int i = 0; i < sb.Length; i++)
            {
                char ii = sb[i];
                if (ii == '\r' || ii == '\n')
                {
                    ii = ' ';
                    sb[i] = ii;
                }
            }

            try
            {

                if (sb.Length > 0)
                    obj = JToken.Parse(sb.ToString());

            }
            catch (Exception e)
            {

                throw new ParsingJsonException("Failed to parse Json. " + e.Message, e);

            }


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
