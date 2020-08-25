using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bb.TransformJson.Services
{

    public class ServiceContainer
    {

        public ServiceContainer()
        {
            this._dictionary = new Dictionary<string, Func<XsltType, ITransformJsonService>>();
        }

        public void AddService(string typeName, Func<ITransformJsonService> provider)
        {
            Func<XsltType, ITransformJsonService> srv = type => MapService(type, provider);
            this._dictionary.Add(typeName, srv);
        }

        public ITransformJsonService GetService(XsltType type)
        {

            if (_dictionary.TryGetValue(type.Type, out Func<XsltType, ITransformJsonService> serviceProvider))
                return serviceProvider(type);

            return null;

        }

        private ITransformJsonService MapService(XsltType type, Func<ITransformJsonService> provider)
        {

            var service = provider();

            foreach (var property in type.Properties)
                    MapProperty(service, property);

            return service;

        }

        private void MapProperty(ITransformJsonService service, XsltProperty value)
        {

        }

        private readonly Dictionary<string, Func<XsltType, ITransformJsonService>> _dictionary;

    }

    public interface ITransformJsonService
    {

    }

}
