
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
                var optEscape = validator.Option("--escape", "specifiy the escape charset. by default the value is '\'");

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
                        !optHeader.HasValue(),
                        optSeparator.HasValue()
                            ? optSeparator.Value()
                            : ";",
                        optQuote.HasValue()
                            ? optQuote.Value()
                            : "\"",
                        optEscape.HasValue()
                            ? optEscape.Value()
                            : "\\"
                        );

                    var result = items.ToString(optNoIndent.HasValue() ? Formatting.None : Formatting.Indented);

                    if (!string.IsNullOrEmpty(argTarget.Value))
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

        private static JArray ReadCsv(string filename, bool hasHeader, string charsetSeparator, string quoteCharset, string escapeCharset)
        {
            JArray result = new JArray();

            var separator = charsetSeparator;
            if (separator.Length == 3)
                separator = separator.Trim(separator[0]);

            var quote = quoteCharset;
            if (quote.Length == 3)
                quote = quote.Trim(quote[0]);

            var escape = escapeCharset;
            if (escape.Length == 3)
                escape = escape.Trim(escape[0]);

            var _file = new FileInfo(filename);

            using (var _txt = _file.OpenText())
            using (CsvReader csv = new CsvReader(_txt, hasHeader, separator[0], quote[0], escape[0], '#', ValueTrimmingOptions.All, (int)_file.Length))
            {
                
                System.Data.IDataReader reader = csv;
                int line = 0;
                try
                {

                    while (reader.Read())
                    {

                        line++;

                        var o = new JObject();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {

                            string name = hasHeader
                                ? reader.GetName(i)
                                    .Replace(" ", "_")
                                    .Replace("'", "_")
                                    .Replace("___", "_")
                                    .Replace("__", "_")
                                    .Replace("é", "e")
                                    .Replace("è", "e")
                                    .Replace("ë", "e")
                                    .Replace("î", "i")
                                    .Replace("ï", "i")
                                    .Replace("ô", "o")
                                    .Replace("à", "a")
                                    .Replace("ê", "e")

                                : "Column" + i.ToString();

                            var value = reader.GetValue(i);

                            o.Add(new JProperty(name, value));

                        }

                        result.Add(o);

                    }

                }
                catch (Exception)
                {
                    Output.WriteLineError($"Failed to read after {result.Last.ToString(Formatting.Indented)}");
                    throw;
                }

            }

            return result;

        }


    }
}


//PrintDataExtensions.ClearBorder();
//        //ConvertToDatatable
//        //    .ConvertList(result.Result.Datas, "applications")
//        //    .Print();