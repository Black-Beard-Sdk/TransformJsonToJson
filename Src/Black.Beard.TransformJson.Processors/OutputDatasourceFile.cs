using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bb.TransformJson.Processors
{


    public class OutputDatasourceFile : ProcessorConfigLoader, IOutputDatasource, IExecuteProcessor
    {

        public OutputDatasourceFile()
        {

        }

        public override void Initialize()
        {

            this._dic = new Dictionary<string, string>();

            this._path = Path.Combine(DirectoryPath, IndexName);

            // Load file index
            this._fileIndex = new FileInfo(Path.Combine(this._path, "index.csv"));
            if (!this._fileIndex.Directory.Exists)
                this._fileIndex.Directory.Create();

            if (_fileIndex.Exists)
            {

                var content = this._fileIndex.LoadContentFromFile();
                var o = content.Split(Environment.NewLine);
                foreach (var item in o)
                    if (!string.IsNullOrEmpty(item) && !string.IsNullOrWhiteSpace(item))
                    {
                        var ar = item.Split(';');
                        if (_dic.ContainsKey(ar[0]))
                            _dic.Remove(ar[0]);
                        _dic.Add(ar[0], ar[1]);
                    }

            }

        }

        public JToken Execute(Sources payload)
        {
         
            int countMissing = 0;
            var array = payload.Source.Datas as JArray;
            foreach (var value in array)
            {

                string key = null;
                var item2 = value["$key"] as JToken;


                if (item2 == null)
                {
                    System.Diagnostics.Trace.WriteLine($"'{value.ToString(Formatting.None)}' have not property '$key'");
                    countMissing++;
                }
                else
                {

                    if (item2 is JValue v1)
                        key = v1.Value.ToString();

                    else if (item2 is JArray a1)
                        key = string.Join(',', a1.Values().Cast<JValue>().Select(c => c.ToString()));

                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"key '{item2.ToString(Formatting.None)}', must be a value or an array of value");
                        countMissing++;
                    }

                    if (key != null)
                        Push(key, value);

                }

            }

            if (countMissing == 1)
                System.Diagnostics.Trace.WriteLine($"missing 1 item");

            else if (countMissing > 1)
                System.Diagnostics.Trace.WriteLine($"missing {countMissing} items");


            if (_fileIndex.Exists)
                _fileIndex.Delete();

            var sb = new StringBuilder();
            foreach (var item in _dic)
                sb.AppendLine($"{item.Key};{item.Value}");
            ContentHelper.Save(_fileIndex.FullName, sb);


            return null;

        }

        public void Push(string key, JToken value)
        {

            string payload = value.ToString(Formatting.Indented);

            string newCrc = payload
                   .Calculate()
                   .ToString();

            if (_dic.TryGetValue(key, out string oldCrc))
            {
                if (oldCrc != newCrc)
                {
                    Write(key, payload);
                    _dic[key] = newCrc;
                }
            }
            else
            {
                Write(key, payload);
                _dic.Add(key, newCrc);
            }

        }

        private void Write(string key, string value)
        {
            // Build folders path by split the key.
            var p = key.ToCharArray().Select(c => c.ToString()).ToList();
            p.Insert(0, _path);
            p.Add("data.json");
            var path = Path.Combine(p.ToArray());

            var file = new FileInfo(path);

            if (file.Exists)
                file.Delete();

            ContentHelper.Save(file.FullName, value);

        }


        public string DirectoryPath { get; set; }

        public string IndexName { get; set; }

        private Dictionary<string, string> _dic;
        private string _path;
        private FileInfo _fileIndex;
    }

}