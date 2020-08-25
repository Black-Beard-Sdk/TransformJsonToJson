using Newtonsoft.Json.Linq;

namespace Bb.TransformJson.Services
{
    public interface ITransformJsonService
    {

        JToken Execute(RuntimeContext ctx, JToken source);

    }

}
