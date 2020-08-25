using System;
using System.Text;
using Bb.TransformJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace TransformJsonUnitTest
{

    /*
     
    value from node in the parent of the current object source
    "name":"$../name"

    "items": { "$type":"foreach",    "source": "$/persons",    "filter": { }, "item": { "name":"$name" } }

    "name":  { "$type":"MonSrvData", "arg1": "$/person/name",  "filter": { } }
     
             { "$type": "filterEqual",     "left": "$name", "right": { "_type": "filterGreatThan", } }

     */

    [TestClass]
    public class UnitTest1
    {

        /// <summary>
        /// If the template is empty, the source doc is returned in the target.
        /// </summary>
        [TestMethod]
        public void TestEmptyTemplate()
        {

            string payloadTemplate = @"";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'name' : 'name111' }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result.ToString(Newtonsoft.Json.Formatting.None), @"{""name"":""name111""}");

        }

        /// <summary>
        /// Constant
        /// "name":"mame1"
        /// </summary>
        [TestMethod]
        public void TestMapConstantString()
        {

            string payloadTemplate = @" { 'name' : 'name1' }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'name' : 'name111' }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["name"], "name1");

        }

        /// <summary>
        /// value from node in the current object source
        /// "name":"$name"
        /// </summary>
        [TestMethod]
        public void TestCompositeMapValue()
        {

            string payloadTemplate = @" { 'person': { 'name' : 'xpath:$.person.identity.name' } }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'person': { 'identity': { 'name' : 'name111' } } }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["person"]["name"], "name111");

        }

        /// <summary>
        /// value from node in the current object source
        /// "name":"$name"
        /// </summary>
        [TestMethod]
        public void TestCompositeMapArrayIndiceValue()
        {

            string payloadTemplate = @" { 'person': { 'name' : 'xpath:$.persons[1].name' } }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'persons': [{'name' : 'name1'}, {'name' : 'name2'}, {'name' : 'name3'}] }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["person"]["name"], "name2");

        }

        /// <summary>
        /// value from node in the current object source
        /// "name":"$name"
        /// </summary>
        [TestMethod]
        public void TestCompositeMapArrayIndiceLast2Value()
        {

            string payloadTemplate = @" { 'person': { 'name' : 'xpath:$.persons[-1:].name' } }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'persons': [{'name' : 'name1'}, {'name' : 'name2'}, {'name' : 'name3'}] }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["person"]["name"], "name3");

        }

        /// <summary>
        /// value from node in the current object source
        /// </summary>
        [TestMethod]
        public void TestCompositeMapArray()
        {

            string payloadTemplate = @"{ 'persons': [{'$source' : 'xpath:{$.persons}', 'name' : 'xpath:{$.n}'}] }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'persons': [{'n' : 'name1'}, {'n' : 'name2'}, {'n' : 'name3'}] }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["persons"][2]["name"], "name3");

        }

        /// <summary>
        /// select 2 rd item in the list by index
        /// </summary>
        [TestMethod]
        public void TestCompositeMapArray2()
        {

            string payloadTemplate = @"{ 'persons': [{'$source' : 'xpath:{$.persons[1]}', 'name' : 'xpath:{$.n}'}] }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'persons': [{'n' : 'name1'}, {'n' : 'name2'}, {'n' : 'name3'}] }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["persons"][0]["name"], "name2");

        }

        /// <summary>
        /// select 2 rd item in the list by filter in field
        /// </summary>
        [TestMethod]
        public void TestCompositeMapArray3()
        {

            string payloadTemplate = @"{ 'persons': [{'$source' : 'xpath:{$.persons[?(@.n == §name2§)]}', 'name' : 'xpath:{$.n}'}] }";
            TranformJsonAstTree template = GetProvider(payloadTemplate);

            string payloadSource = @"{ 'persons': [{'n' : 'name1'}, {'n' : 'name2'}, {'n' : 'name3'}] }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["persons"][0]["name"], "name2");

        }



        private static TranformJsonAstTree GetProvider(string payloadTemplate)
        {

            StringBuilder sb = new StringBuilder(payloadTemplate.Replace('\'', '"').Replace('§', '\''));

            var configuration = new TranformJsonAstConfiguration();
            TemplateTransformProvider Templateprovider = new TemplateTransformProvider(configuration);

            TranformJsonAstTree template = Templateprovider.GetTemplate(sb);

            return template;

        }
    
    }

}
