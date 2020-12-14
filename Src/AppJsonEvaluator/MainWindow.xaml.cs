using Bb.TransformJson;
using Bb.TransformJson.Asts;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppJsonEvaluator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();
            this._foldingStrategy = new BraceFoldingStrategy();

            //var o = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.HighlightingDefinitions;


            _templateFoldingManager = UpdateTemplate(TemplateEditor);
            _sourceFoldingManager = UpdateTemplate(SourceEditor);
            _targetFoldingManager = UpdateTemplate(TargetEditor);

            TemplateEditor.Load("template.txt");
            SourceEditor.Load("source.txt");

        }

        private FoldingManager UpdateTemplate(TextEditor textEditor)
        {
            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
            textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Json");
            textEditor.ShowLineNumbers = true;
            textEditor.Options.IndentationSize = 3;

            return FoldingManager.Install(textEditor.TextArea);

        }

        private void UpdateFolding(FoldingManager foldingManager, TextEditor textEditor)
        {
            _foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            TemplateEditor.Save("template.txt");
            SourceEditor.Save("source.txt");

            base.OnClosing(e);

        }

        private void Update()
        {

            Errors.Items.Clear();

            XjsltTemplate template = null;
            StringBuilder payloadSource;

            try
            {
                template = TemplateEditor.Text.GetTransformProvider();
            }
            catch (Exception e1)
            {
                Errors.Items.Insert(0, e1.Message);
            }

            if (template != null)
            {

                try
                {
                    payloadSource = new StringBuilder(SourceEditor.Text);
                    var result = template.Transform(payloadSource);
                    var value = result.Item1.ToString();
                    TargetEditor.Text = value;
                }
                catch (Exception e2)
                {
                    Errors.Items.Insert(0, e2.Message);
                }

            }


        }

        private void TemplateEditorTextChanged(object sender, EventArgs e)
        {
            UpdateFolding(_templateFoldingManager, TemplateEditor);
            Update();
        }

        private void SourceEditorTextChanged(object sender, EventArgs e)
        {
            UpdateFolding(_sourceFoldingManager, SourceEditor);
            Update();
        }

        private void TargetEditorTextChanged(object sender, EventArgs e)
        {
            UpdateFolding(_targetFoldingManager, TargetEditor);

        }


        private readonly BraceFoldingStrategy _foldingStrategy;
        private readonly FoldingManager _templateFoldingManager;
        private readonly FoldingManager _sourceFoldingManager;
        private readonly FoldingManager _targetFoldingManager;


    }



    public static class Extensions
    {


        public static void Load(this RichTextBox richTextBox, string filename)
        {
            TextRange textRange = richTextBox.GetRange();
            textRange.Text = System.IO.Path.Combine(Environment.CurrentDirectory, filename).LoadFile();
        }

        public static void Save(this RichTextBox richTextBox, string filename)
        {
            TextRange textRange = richTextBox.GetRange();
            System.IO.Path.Combine(Environment.CurrentDirectory, filename).WriteInFile(textRange.Text);
        }

        public static TextRange GetRange(this RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange;
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

        public static XjsltTemplate GetTransformProvider(this string self, params (string, ITransformJsonService)[] services)
        {

#if UNIT_TEST
            StringBuilder sb = new StringBuilder(self.Replace('\'', '"').Replace('§', '\''));
#else
            StringBuilder sb = new StringBuilder(self);
#endif

            var configuration = new TranformJsonAstConfiguration();
            foreach (var item in services)
                configuration.AddService(item.Item1, item.Item2);

            TemplateTransformProvider Templateprovider = new TemplateTransformProvider(configuration);

            XjsltTemplate template = Templateprovider.GetTemplate(sb);

            return template;

        }

    }

}
