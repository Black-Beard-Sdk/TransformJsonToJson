using Bb.TransformJson.Asts;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Bb.TransformJson.Processors
{


    public class ExecuteTemplateProcessor : ProcessorConfigLoader, IExecuteProcessor
    {


        public ExecuteTemplateProcessor(TemplateTransformProvider templateprovider)
        {
            this._templateProvider = templateprovider;
        }

        public override void Initialize()
        {

            var fileTemplate = new FileInfo(this.PathTemplate);
            if (!fileTemplate.Exists)
                throw new FileNotFoundException(fileTemplate.FullName);

            var sbPayloadTemplate = new StringBuilder(ContentHelper.LoadContentFromFile(fileTemplate.FullName));
            this._template = _templateProvider.GetTemplate(sbPayloadTemplate);

        }

        public JToken Execute(Sources sources)
        {
            var result = this._template.Transform(sources);
            this.LastRuntimeContext = result.Item2;
            return result.Item1;
        }

        public RuntimeContext LastRuntimeContext { get; private set; }

        public string PathTemplate { get; set; }

        private TemplateTransformProvider _templateProvider;
        private XjsltTemplate _template;


    }


}
