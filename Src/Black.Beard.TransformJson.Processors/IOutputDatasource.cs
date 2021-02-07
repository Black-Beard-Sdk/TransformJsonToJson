using Newtonsoft.Json.Linq;

namespace Bb.TransformJson.Processors
{


    public interface IOutputDatasource
    {

        void Initialize();

        //void Push(string key, JToken value);

    }

}