﻿using Bb.TransformJson;
using Bb.TransformJson.Asts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AppJsonEvaluator
{
    public static class Extensions
    {


        //public static void Load(this RichTextBox richTextBox, string filename)
        //{
        //    TextRange textRange = richTextBox.GetRange();
        //    textRange.Text = System.IO.Path.Combine(Environment.CurrentDirectory, filename).LoadFile();
        //}

        //public static void Save(this RichTextBox richTextBox, string filename)
        //{
        //    TextRange textRange = richTextBox.GetRange();
        //    System.IO.Path.Combine(Environment.CurrentDirectory, filename).WriteInFile(textRange.Text);
        //}

        public static TextRange GetRange(this RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange;
        }

        public static T Deserialize<T>(this string self)
        {

            return JsonConvert.DeserializeObject<T>(self);

        }

        public static string Serialize<T>(this T self)
        {

            return JsonConvert.SerializeObject(self);

        }

        public static string LoadFile(this string filename)
        {
            return File.ReadAllText(filename);
        }

        public static void WriteInFile(this string self, string content)
        {

            byte[] array = System.Text.UTF8Encoding.UTF8.GetBytes(content);

            using (var file = File.OpenWrite(self))
            {
                file.Write(array, 0, array.Length);
            }

        }

        public static XjsltTemplate GetTransformProvider(this string self, params Type[] services)
        {

#if UNIT_TEST
            StringBuilder sb = new StringBuilder(self.Replace('\'', '"').Replace('§', '\''));
#else
            StringBuilder sb = new StringBuilder(self);
#endif

            var configuration = new TranformJsonAstConfiguration();
            foreach (var item in services)
                configuration.AddService(item);

            TemplateTransformProvider Templateprovider = new TemplateTransformProvider(configuration);

            XjsltTemplate template = Templateprovider.GetTemplate(sb);

            return template;

        }

    }

}
