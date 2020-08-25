using Bb.TransformJson.Services;
using System;

namespace Bb.TransformJson
{
    public class TranformJsonAstConfiguration
    {

        public TranformJsonAstConfiguration()
        {

            this.Services = new ServiceContainer();

        }

        public TranformJsonAstConfiguration AddService(string typeName, Func<ITransformJsonService> provider)
        {
            Services.AddService(typeName, provider);
            return this;
        }

        public TranformJsonAstConfiguration AddService<T>(string typeName, T provider)
            where T : ITransformJsonService
        {
            Services.AddService(typeName, () => provider);
            return this;
        }


        public ServiceContainer Services { get; }

    }
}