using Bb.TransformJson.Asts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.TransformJson
{

    public interface IExecuteProcessor
    {

        JToken Execute();

        JToken Execute(string payload);

    }

}
