
using Bb.CommandLines;
using Bb.CommandLines.Ins;
using Bb.CommandLines.Outs;
using Bb.CommandLines.Validators;
using Bb.Sdk.Csv;
using Bb.TransformJson.Processors;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bb.Json.Commands
{




    /// <summary>
    /// 
    /// </summary>
    public static partial class Command
    {


        public static CommandLineApplication CommandLoadCsv(this CommandLineApplication app)
        {

            var dataCmd = app.Command("csv", config =>
            {
                config.Description = "load csv and convert in json";
                config.HelpOption(HelpFlag);

                var validator = new GroupArgument(config);

                var optHeader = validator.OptionNoValue("--noheader", "specifiy the cav source have'nt no header");
                var optSeparator = validator.Option("--separator", "specifiy the sperator charset. by default the value is ';'");
                var optQuote = validator.Option("--quote", "specifiy the quote charset. by default the value is '\"'");

                var argSource = validator.Argument("<source file>", "csv source path that contains data source"
                   , ValidatorExtension.EvaluateFileExist
                   );

                var argTarget = validator.Argument("<target file>", "json file target path"
                   );

                var optNoIndent = validator.OptionNoValue("--noIndented", "format stream on one line");


                config.OnExecute(() =>
                {

                    if (!validator.Evaluate(out int errorNum))
                        return errorNum;

                    var items = ReadCsv(
                        argSource.Value,
                        optHeader.HasValue(),
                        optSeparator.HasValue()
                            ? optSeparator.Value()
                            : ";",
                        optQuote.HasValue()
                            ? optQuote.Value()
                            : "\""

                        ).ToList();

                    string result = new JArray(items)
                        .ToString(optNoIndent.HasValue() ? Formatting.None : Formatting.Indented);

                    if (string.IsNullOrEmpty(argTarget.Value))
                    {

                        if (File.Exists(argTarget.Value))
                            File.Delete(argTarget.Value);

                        ContentHelper.Save(argTarget.Value, result);

                    }
                    else
                        result.WriteLineStandard();

                    return 0;

                });

            });

            return app;

        }

        private static IEnumerable<JObject> ReadCsv(string filename, bool hasHeader, string charsetSeparator, string quoteCharset)
        {

            var _file = new FileInfo(filename);

            using (var _txt = _file.OpenText())
            using (CsvReader csv = new CsvReader(_txt, hasHeader, charsetSeparator[0], quoteCharset[0], '\\', '#', ValueTrimmingOptions.All, (int)_file.Length))
            {

                System.Data.IDataReader reader = csv;

                while (reader.Read())
                {

                    var o = new JObject();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {

                        string name = hasHeader
                            ? reader.GetName(i)
                            : "Column" + i.ToString();

                        var value = reader.GetValue(i);

                        o.Add(new JProperty(name, value));

                    }

                    yield return o;

                }

            }

        }


    }
}


//PrintDataExtensions.ClearBorder();
//        //ConvertToDatatable
//        //    .ConvertList(result.Result.Datas, "applications")
//        //    .Print();