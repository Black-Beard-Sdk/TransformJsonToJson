using Bb.TransformJson.Asts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Bb.TransformJson
{

    public class ServiceContainer
    {

        public ServiceContainer()
        {
            this._dictionary = new Dictionary<string, Func<ITransformJsonService>>();
        }

        public ServiceContainer AddService(string typeName, Func<ITransformJsonService> provider)
        {
            this._dictionary.Add(typeName, provider);
            return this;
        }

        public TransformJsonServiceProvider GetService(string type)
        {

            if (_dictionary.TryGetValue(type, out Func<ITransformJsonService> serviceProvider))
                return new TransformJsonServiceProvider(serviceProvider);

            return null;

        }

        private readonly Dictionary<string, Func<ITransformJsonService>> _dictionary;

    }

    public class TransformJsonServiceProvider
    {

        static TransformJsonServiceProvider()
        {
            TransformJsonServiceProvider.Method = typeof(TransformJsonServiceProvider).GetMethod("Get", BindingFlags.Instance | BindingFlags.Public);
        }

        public TransformJsonServiceProvider(Func<ITransformJsonService> generator)
        {
            this._generator = generator;
        }

        public ITransformJsonService Get()
        {
            return this._generator();
        }

        private readonly Func<ITransformJsonService> _generator;

        public static MethodInfo Method { get; private set; }

    }

}
