using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Bb.TransformJson;
using Bb.TransformJson.Asts;
using Bb.TransformJson.Parsers;
using Bb.TransformJson.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace TransformJsonUnitTest
{

    /*
     
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

            string payloadTemplate = @" { 'person': { 'name' : 'jpath:{$.person.identity.name}' } }";
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

            string payloadTemplate = @" { 'person': { 'name' : 'jpath:{$.persons[1].name}' } }";
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

            string payloadTemplate = @"{ 'person': { 'name' : 'jpath:{$.persons[-1:].name}' } }";
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

            string payloadTemplate = @"{ 'persons': [{'$source' : 'jpath:{$.persons}', 'name' : 'jpath:{$.n}'}] }";
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

            string payloadTemplate = @"{ 'persons': [{'$source' : 'jpath:{$.persons[1]}', 'name':'jpath:{$.n}'}] }";
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

            string payloadTemplate = @"{ 'persons': [{'$source' : 'jpath:{$.persons[?(@.n == §name2§)]}', 'name' : 'jpath:{$.n}'}] }";
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
        public void TestCompositeExecuteCustomRule()
        {

            string payloadTemplate = @"{ 'prices': 'jpath:{$..n} | sum:{}' }";
            TranformJsonAstTree template = GetProvider(payloadTemplate,
                ("sum", new ServiceSum())
                );

            string payloadSource = @"{ 'prices': [{'n' : 1}, {'n' : 2}, {'n' : 3}] }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["prices"], 6);

        }

        /// <summary>
        /// select 2 rd item in the list by filter in field
        /// </summary>
        [TestMethod]
        public void TestCompositeExecuteCustomRuleOnType()
        {

            string payloadTemplate = @"{ 'Person': { '$source':'data1', 'id':'jpath:{$.id}' } }";
            TranformJsonAstTree template = GetProvider(payloadTemplate,
                ("data1", new DataClass())
                );

            string payloadSource = @"{ 'Event':'e1', 'id':'FR345' }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["Person"]["Uuid"], "FR345");

        }

        /// <summary>
        /// select 2 rd item in the list by filter in field
        /// </summary>
        [TestMethod]
        public void TestCompositeExecuteCustomRuleOnType2()
        {

            string payloadTemplate = @"{ '$source':'data1', '$id':'jpath:{$.id}' }";
            TranformJsonAstTree template = GetProvider(payloadTemplate,
                ("data1", new DataClass())
                );

            string payloadSource = @"{ 'Event':'e1', 'id':'FR345' }";
            StringBuilder sb2 = new StringBuilder(payloadSource.Replace('\'', '"'));

            var result = template.Transform(sb2) as JObject;

            Assert.AreEqual(result["Person"]["Uuid"], "FR345");

        }


        /// <summary>
        /// select 2 rd item in the list by filter in field
        /// </summary>
        [TestMethod]
        public void Test1()
        {

            string rule = "jpath:{$..n} | sum:{ 'id':'jpath:{$..n}' }"
                .Replace('\'', '"')
                .Replace('§', '\'');

            var p = new StringParser(rule);
            var o = p.Get();


        }

        private static TranformJsonAstTree GetProvider(string payloadTemplate, params (string, ITransformJsonService)[] services)
        {

            StringBuilder sb = new StringBuilder(payloadTemplate.Replace('\'', '"').Replace('§', '\''));

            var configuration = new TranformJsonAstConfiguration();
            foreach (var item in services)
                configuration.AddService(item.Item1, item.Item2);

            TemplateTransformProvider Templateprovider = new TemplateTransformProvider(configuration);

            TranformJsonAstTree template = Templateprovider.GetTemplate(sb);

            return template;

        }

    }

    public class DataClass : ITransformJsonService
    {

        public string Id { get; set; }

        public JToken Execute(RuntimeContext ctx, JToken source)
        {

            return new JObject(

                    new JProperty("Uuid", new JValue(Id))

                );


        }

    }

}
