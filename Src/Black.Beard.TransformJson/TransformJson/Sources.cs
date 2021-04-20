using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bb.TransformJson
{
    public class Sources
    {

        public Sources(SourceJson source)
        {
            this.Source = source;
            this._datas = new Dictionary<string, SourceJson>();
        }

        public SourceJson Source { get; }


        public Sources Add(SourceJson source)
        {
            this._datas.Add(source.Name, source);
            return this;
        }

        public SourceJson this [string name]
        {
            get
            {
                if (name == null)
                    return this.Source;

                return _datas[name];
            }
        }


        public static implicit operator Sources (StringBuilder sb)
        {
            return new Sources(sb);
        }

        public static implicit operator Sources(string payload)
        {
            return new Sources(payload);
        }

        public static implicit operator Sources(JToken datas)
        {
            return new Sources(datas);
        }

        public static implicit operator Sources(FileInfo file)
        {
            return new Sources(file);
        }

        public static implicit operator Sources(SourceJson datas)
        {
            return new Sources(datas);
        }


        private readonly Dictionary<string, SourceJson> _datas;

    }

}