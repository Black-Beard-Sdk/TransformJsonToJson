using System.Data;
using System.IO;
using System.Text;

namespace Bb.ConvertToDatables
{
    public static class CsvWriterHelper
    {


        public static void Write(this DataSet self
                                    , DirectoryInfo target
                                    , string rootName
                                    , Encoding encoding
                                    , bool writeHeader = true
                                    , char fieldSeparator = ';'
                                    , char quoteField = '\0'
                                )
        {

            if (!target.Exists)
                target.Create();

            foreach (DataTable table in self.Tables)
            {

                var file = new FileInfo(Path.Combine(target.FullName, rootName.Trim('_') + "_" + table.TableName + ".csv"));

                if (file.Exists)
                    file.Delete();

                using (var writer = new CsvWriter(file)
                {
                    Encoding = encoding,
                    FieldSeparator = fieldSeparator.ToString(),
                    QuoteField = quoteField.ToString(),
                    WriteQuote = quoteField != '\0',
                    WriteHeader = writeHeader,
                })
                {
                    writer.Open();
                    writer.WriteToStream(table);
                }

            }

        }

    }

}
