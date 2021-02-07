using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Bb.ConvertToDatables
{
    internal class Context
    {

        public Context(DataSet _dataset)
        {
            this._newRows = new Dictionary<string, DataRow>();
            this._dataset = _dataset;
        }

        public void NewLine(string tableName, List<(string, string)> parentKeys)
        {

            var r = this._dataset.Tables[tableName].NewRow();
            r["technicalKey"] = Guid.NewGuid();

            foreach (var item in parentKeys)
            {
                var table = this._newRows[item.Item1];
                r[item.Item2] = table["technicalKey"];
            }

            _newRows.Add(tableName, r);

        }

        internal void Close(string tableName)
        {
            this._dataset.Tables[tableName].Rows.Add(_newRows[tableName]);
            _newRows.Remove(tableName);
        }


        internal void AppendProperty(MappingProperty item, object value)
        {
            this._newRows[item.Table.TableName][item.Column] = value;
        }

        private Dictionary<string, DataRow> _newRows;

        private DataSet _dataset { get; }

    }
}