using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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
    public class CompareUnitTest
    {

        /// <summary>
        /// Compare string value
        /// </summary>
        [TestMethod]
        public void ComparePropertyValue()
        {

            JToken _from = new JObject() { new JProperty("p1", "old") };
            JToken _to = new JObject() { new JProperty("p1", "new") };

            var result = new DiffJsonComparer(new CompareJsonContext())
                    .Compare(_from, _to);

            Assert.AreEqual(result.Subs.Count , 1);
            Assert.AreEqual(result.Subs[0].Name, "p1");
            Assert.AreEqual(result.Subs[0].Old.ToString(), "old");
            Assert.AreEqual(result.Subs[0].New.ToString(), "new");
        }

        /// <summary>
        /// Compare string value
        /// </summary>
        [TestMethod]
        public void ComparePropertyValueType()
        {

            JToken _from = new JObject() { new JProperty("p1", 1) };
            JToken _to = new JObject() { new JProperty("p1", "1") };

            var result = new DiffJsonComparer(new CompareJsonContext())
                    .Compare(_from, _to);

            Assert.AreEqual(result.Subs.Count, 1);
            Assert.AreEqual(result.Subs[0].Name, "p1");
            Assert.AreEqual(result.Subs[0].Old.ToString(), "1");
            Assert.AreEqual(result.Subs[0].New.ToString(), "1");
        }

        /// <summary>
        /// Add & remove properties
        /// </summary>
        [TestMethod]
        public void ComparePropertyRenamed()
        {

            JToken _from = new JObject() { new JProperty("p1", "old") };
            JToken _to = new JObject() { new JProperty("p2", "new") };

            var result = new DiffJsonComparer(new CompareJsonContext())
                    .Compare(_from, _to);

            Assert.AreEqual(result.Subs.Count, 2);

            Assert.AreEqual(result.Subs[0].Name, "p1");
            Assert.AreEqual(result.Subs[0].Type,  CompareResultType.Removed);
            
            Assert.AreEqual(result.Subs[1].Name, "p2");
            Assert.AreEqual(result.Subs[1].Type, CompareResultType.Added);

        }

        /// <summary>
        /// Add & remove properties
        /// </summary>
        [TestMethod]
        public void ComparePropertyChangeType()
        {

            JToken _from = new JObject() { new JProperty("p1", new JObject() { new JProperty("p2", new JObject() { new JProperty("p3", "value") }) }) };
            JToken _to = new JObject() { new JProperty("p1", new JObject() { new JProperty("p2", new JArray() { new JValue("v1") } ) } ) };

            var result = new DiffJsonComparer(new CompareJsonContext())
                    .Compare(_from, _to);

            Assert.AreEqual(result.Subs.Count, 1);
            Assert.AreEqual(result.Subs[0].Subs.Count, 1);

            Assert.AreEqual(result.Subs[0].Name, "p1");
            Assert.AreEqual(result.Subs[0].Subs[0].Name, "p2");
            Assert.AreEqual(result.Subs[0].Subs[0].Type, CompareResultType.Changed);

        }

    }

}
