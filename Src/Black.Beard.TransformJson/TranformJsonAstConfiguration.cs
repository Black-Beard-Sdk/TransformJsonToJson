using Bb.TransformJson.Services;

namespace Bb.TransformJson
{
    public class TranformJsonAstConfiguration
    {

        public TranformJsonAstConfiguration()
        {

            this.Services = new ServiceContainer();

        }

        public ServiceContainer Services { get; }

    }
}