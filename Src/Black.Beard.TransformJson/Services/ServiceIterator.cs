namespace Bb.TransformJson.Services
{

    public class ServiceIterator : ITransformJsonService
    {

        public ServiceIterator()
        {

        }

      
        public XsltJson Source { get; set; }

        public ITransformJsonService Filter { get; set; }

        public XsltJson Select { get; set; }


    }

}
