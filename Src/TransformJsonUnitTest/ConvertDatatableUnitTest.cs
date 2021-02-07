using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Bb.ConvertToDatables;
using Bb.DifferenceJson;
using Bb.TransformJson;
using Bb.TransformJson.Asts;
using Bb.TransformJson.Parsers;
using Bb.TransformJson.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace TransformJsonUnitTest
{


    [TestClass]
    public class ConvertDatatableUnitTest
    {

        /// <summary>
        /// Compare string value
        /// </summary>
        [TestMethod]
        public void ComparePropertyValue()
        {

            JToken schema = new JObject()
            {
                new JProperty("$new", new JArray() { "user" } ),

                new JProperty("Name", new JObject()
                    {
                        new JProperty("$path", "user"), new JProperty("$type", "System.String")
                    }) ,

                 new JProperty("Civility",new JObject()
                    {
                        new JProperty("Born", new JObject()
                        {
                            new JProperty("$path", "user"), new JProperty("$type", "System.DateTime")
                        })
                    }),

                 new JProperty("Events",new JArray()
                    {
                        new JObject()
                        {
                            new JProperty("$new", new JArray() { "events" } ),
                            new JProperty("Name", new JObject()
                            {
                                new JProperty("$path", "events.name"), new JProperty("$type", "System.String")
                            })
                        },
                    })

            };

            var builder = new BuildSchema();
            var parser = builder.ParseTemplate(schema);

            JToken obj = new JObject()
            {
                new JProperty("Name", new JValue("name1")) ,
                new JProperty("Civility",new JObject() { new JProperty("Born", new JValue(DateTime.Now)) }),
                new JProperty("Events",  new JArray() 
                {  
                    new JObject() { new JProperty("Name", new JValue("name1")), },
                    new JObject() { new JProperty("Name", new JValue("name2")), },
                })
            };

            parser.Import(obj);

        }

    }

}
