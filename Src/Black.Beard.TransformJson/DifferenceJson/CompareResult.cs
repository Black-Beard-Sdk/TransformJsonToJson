using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Bb.DifferenceJson
{

    public class CompareResult
    {
        public JToken Old { get; internal set; }
        public JToken New { get; internal set; }
        public CompareResultType Type { get; internal set; }
        public string Path { get; internal set; }
        public string Name { get; internal set; }

        public List<CompareResult> Subs { get; } = new List<CompareResult>();


    }

    public enum CompareResultType
    {
        Changed,
        Removed,
        Added,
    }


}
