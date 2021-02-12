using Bb.CommandLines;
using Bb.CommandLines.Ins;
using Bb.CommandLines.Outs;
using Bb.CommandLines.Validators;
using Bb.ConvertToDatables;
using Bb.TransformJson.Processors;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bb.Json.Commands
{


    /*
    .\Json.exe template execute C:\_perso\Src\Sdk\TransformJsonToJson\Src\DocTests\template1.json C:\_perso\Src\Sdk\TransformJsonToJson\Src\DocTests\source1.json
    */


    /// <summary>
    /// 
    /// </summary>
    public static partial class Command
    {


        public static CommandLineApplication CommandExport(this CommandLineApplication app)
        {

            // json template 

            var cmd = app.Command("export", config =>
            {

                config.Description = "export process";
                config.HelpOption(HelpFlag);

            });

            /*
                json export csv <template> <target folder> --source 'source file' --name 'nameRoot' --h --s ';' --q '"'
            */
            cmd.Command("csv", config =>
            {

                config.Description = "run template transformation with the specified template";
                config.HelpOption(HelpFlag);

                var validator = new GroupArgument(config);

                var argTemplatePath = validator.Argument("<template file>", "template path"
                    , ValidatorExtension.EvaluateFileExist
                    , ValidatorExtension.EvaluateRequired
                    );

                var argtarget = validator.Argument("<target folder>", "json source path that contains data source"
                     , ValidatorExtension.EvaluateRequired
                     );

                var argSource = validator.Option("--source", "json source file path that contains data source. if option is missing source is readed from stdin stream");
                var argTargetName = validator.Option("--name", "name of the output csv files. if the value is missing a randomized name is generated");
                var optWriteHeader = validator.OptionNoValue("--h", "Write header");
                var optSeparator = validator.Option("--s", "specify the charset separator. by default the value is ';'");
                var optQuote = validator.Option("--q", "specify the charset quote. by default the value is '\"'");

                config.OnExecute(() =>
                {

                    if (!validator.Evaluate(out int errorNum))
                        return errorNum;

                    var builder = new BuildSchema();

                    var template = argTemplatePath.Value.TrimPath()
                        .LoadContentFromFile()
                        .ConvertToJson();

                    var parser = builder.ParseTemplate(template);

                    var inPipe = Input.IsPipedInput;
                    JToken source = null;

                    string targetName = string.Empty;

                    if (argSource.HasValue())
                    {

                        var s = argSource.Value();
                        source = s.TrimPath()
                                 .LoadContentFromFile()
                                 .ConvertToJson();

                        targetName = Path.GetFileNameWithoutExtension(s);

                    }
                    else
                    {

                        if (!inPipe)
                        {
                            app.ShowHelp();
                            return ErrorEnum.MissingSource.Error("no source specified");
                        }

                        source = Input.ReadInput(Encoding.UTF8)
                            .ConvertToJson();

                        if (argTargetName.HasValue())
                            targetName = Path.GetFileNameWithoutExtension(argTargetName.Value().TrimPath());
                        else
                            targetName = Guid.NewGuid().ToString()
                                .Replace("-", "")
                                .Trim(' ', '{', '}')
                                ;

                    }

                    parser.Import(source);

                    var targetDir = new DirectoryInfo(argtarget.Value.TrimPath());

                    char separator = ';';
                    char quote = '"';

                    if (optSeparator.HasValue())
                    {
                        string se = optSeparator.Value();
                        if (se.Length > 1)
                            se = se.Trim(se[0]);
                        separator = se[0];
                    }

                    if (optQuote.HasValue())
                    {
                        string se = optQuote.Value();
                        if (se.Length > 1)
                            se = se.Trim(se[0]);
                        quote = se[0];
                    }

                    parser.Dataset.Write(
                            targetDir
                          , targetName
                          , Encoding.UTF8
                          , optWriteHeader.HasValue()
                          , separator
                          , quote
                          );

                    return 0;

                });
            });


            return app;

        }

    }
}
