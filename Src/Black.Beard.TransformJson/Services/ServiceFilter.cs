namespace Bb.TransformJson.Services
{


    public class ServiceFilter : ITransformJsonService
    {


        public object Left { get; set; }

        public object Right { get; set; }


    }

    public class ServiceFilterEqual : ServiceFilter
    {



    }


}
