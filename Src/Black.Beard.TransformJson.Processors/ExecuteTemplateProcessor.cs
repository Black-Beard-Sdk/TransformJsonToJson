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

        public JToken Execute()
        {

            // Charge and evaluate source
            var filePathSource = new FileInfo(PathSource);
            if (!filePathSource.Exists)
                throw new FileNotFoundException(filePathSource.FullName);

            var fileContent = new StringBuilder(ContentHelper.LoadContentFromFile(filePathSource.FullName));
            var r = Execute(fileContent);

            return r;

        }

        public JToken Execute(StringBuilder sbPayload)
        {
            var result = _template.Transform(sbPayload);
            this.LastRuntimeContext = result.Item2;
            return result.Item1;
        }

        public JToken Execute(string payload)
        {
            return Execute(new StringBuilder(payload));
        }

        public RuntimeContext LastRuntimeContext { get; private set; }

        public string PathTemplate { get; set; }

        public string PathSource { get; set; }

        private TemplateTransformProvider _templateProvider;
        private XjsltTemplate _template;


    }


}
