using Bb.CommandLines.Outs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Bb;

namespace Black.Beard.TransformJson.Processors.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        public UnitTest1()
        {

            this._directoryRoot = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"..\..\..\.."));
            this._directoryDocTests = new DirectoryInfo(Path.Combine(_directoryRoot.FullName, @"DocTests"));
            this._directoryout = new DirectoryInfo(Path.Combine(_directoryRoot.FullName, @"outTests"));

            if (!this._directoryout.Exists)
                this._directoryout.Create();

        }

        [TestMethod]
        public void TestEasyTransform()
        {

            var output = Output.GetStandardRedirection();

            var source = GetPathDoc("source1.json");
            var template = GetPathDoc("template1.json");

            Bb.Json.Program.Main("template", "execute",
                    Quote(template),
                    Quote(source)
                );

            var result = output.ConvertToString()
                               .ConvertToJson();

            Assert.AreEqual(result["bigName"], "toto");

        }

        [TestMethod]
        public void TestEasyTransformWithMerge()
        {

            var output = Output.GetStandardRedirection();

            var source = GetPathDoc("source1.json");
            var template = GetPathDoc("template1.json");

            Bb.Json.Program.Main("template", "execute",
                    Quote(template), "--m",
                    Quote(source)
                );

            var result = output.ConvertToString()
                               .ConvertToJson();

            Assert.AreEqual(result["bigName"], "toto");
            Assert.AreEqual(result["value1"], "toto");

        }

        [TestMethod]
        public void TestEasyImport()
        {

            var output = Output.GetStandardRedirection();

            var source = GetPathDoc("source2.json");

            var uniqueName = DateTime.UtcNow.ToString()
                .Replace(" ", "")
                .Replace("/", "")
                .Replace(":", "")
                .ToString()
                ;

            var indexName = "ind_" + uniqueName;

            Bb.Json.Program.Main("data", "import", "localsystem",
                    indexName,
                    Quote(this._directoryout),
                    Quote(source)
                );

            var content = new FileInfo(Path.Combine(this._directoryout.FullName, indexName, "k", "2", "data.json"))
                .FullName.LoadContentFromFile()
                .ConvertToJson(); 

            Assert.AreEqual(content["Name"], "n2");

        }

        [TestMethod]
        public void TestExportCsv()
        {

            string targetName = Guid.NewGuid().ToString()
                                .Replace("-", "")
                                .Trim(' ', '{', '}')
                                ;

            var source = GetPathDoc("source3.json");
            var template = GetPathDoc("templateExportDataset.json");

            // json export csv <template> <target folder> --source 'source file' --n 'nameRoot' --h --s ';' --q '"'
            Bb.Json.Program.Main("export", "csv",
                    Quote(template),
                    Quote(this._directoryout.FullName),
                    "--source", Quote(source),
                    "--name", Quote(targetName),
                    "--h", 
                    "--s", Quote(","),
                    "--q", "\""
                );

           

        }

        private FileInfo GetPathDoc(string filename)
        {
            return new FileInfo(Path.Combine(_directoryDocTests.FullName, filename));
        }

        private string Quote(FileInfo file)
        {
            return $"'{file.FullName}'";
        }

        private string Quote(DirectoryInfo file)
        {
            return $"'{file.FullName}'";
        }


        private string Quote(string text)
        {
            return $"'{text}'";
        }

        

        private readonly DirectoryInfo _directoryRoot;
        private readonly DirectoryInfo _directoryDocTests;
        private readonly DirectoryInfo _directoryout;
    }
}
