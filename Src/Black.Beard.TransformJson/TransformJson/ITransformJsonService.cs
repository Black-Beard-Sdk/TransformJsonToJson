using Newtonsoft.Json.Linq;

namespace Bb.TransformJson
{
    public interface ITransformJsonService
    {

        JToken Execute(RuntimeContext ctx, JToken source);

    }

}
