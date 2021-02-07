using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace Bb.TransformJson.Processors
{


    public abstract class ProcessorConfigLoader
    {


        public ProcessorConfigLoader()
        {
        }


        public void InitializeFromFile(FileInfo file)
        {

            var fileContent = ContentHelper.LoadContentFromFile(file.FullName);
            JsonConvert.PopulateObject(fileContent, this);
            
            this.Initialize();

        }

        public abstract void Initialize();


    }


}
