using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bb.ConvertToDatables
{
    public class Parser
    {


        public Parser(System.Data.DataSet dataSet, Parser parent)
        {
            this._parent = parent;
            this.Dataset = dataSet;
            this._properties = new List<MappingProperty>();
            this._sub = new List<Parser>();
            this._newLines = new List<string>();
            this._parentKeys = new List<(string, string)>();

        }

        internal void Initialize()
        {

            if (this._parent != null && this._newLines.Any())
            {

                foreach (var tableName in this._newLines)
                {

                    var table = this.Dataset.Tables[tableName];
                    var key = table.Columns["technicalKey"];
                    var ordinal = key.Ordinal + 1;

                    foreach (var parentTableName in this._parent.GetTables())
                    {
                        string name = parentTableName + "_" + "technicalKey";
                        if (!table.Columns.Contains(name))
                        {
                            var c = new DataColumn(name, typeof(Guid));
                            table.Columns.Add(c);
                            c.SetOrdinal(ordinal++);
                            this._parentKeys.Add((parentTableName, name));
                        }
                    }
                }

            }

            foreach (var item in this._sub)
                item.Initialize();

        }

        public string Name { get; set; }

        public DataSet Dataset { get; }
        public bool IsArray { get; internal set; }

        public IEnumerable<string> GetTables()
        {

            if (_parent != null)
                foreach (var item in _parent.GetTables())
                    yield return item;

            foreach (var item in this._newLines)
                yield return item;

        }


        #region Build template

        internal MappingProperty AddProperty(string name)
        {
            var p = new MappingProperty(this.Dataset) { Name = name };
            this._properties.Add(p);
            return p;
        }

        internal Parser AddSub(string name)
        {
            var p = new Parser(Dataset, this) { Name = name };
            this._sub.Add(p);
            return p;
        }

        internal void AppendNewLine(string tableName)
        {
            this._newLines.Add(tableName);
        }

        #endregion Build template


        public void Import(JToken obj)
        {
            ImportCurrent(obj, new Context(this.Dataset));
        }

        private void ImportCurrent(JToken obj, Context ctx)
        {

            if (this.IsArray)
            {

                var a = obj as JArray;
                foreach (var item in a)
                {

                    foreach (var tableName in this._newLines)
                        ctx.NewLine(tableName, this._parentKeys);

                    ParseProperties(item, ctx);
                    ParseSub(item, ctx);

                    foreach (var tableName in this._newLines)
                        ctx.Close(tableName);

                }
            }
            else
            {

                foreach (var tableName in this._newLines)
                    ctx.NewLine(tableName, this._parentKeys);

                ParseProperties(obj, ctx);
                ParseSub(obj, ctx);

                foreach (var tableName in this._newLines)
                    ctx.Close(tableName);

            }

        }

        private void ParseSub(JToken obj, Context ctx)
        {
            foreach (var item in this._sub)
            {
                JToken t = obj[item.Name];
                if (t != null)
                    item.ImportCurrent(t, ctx);
            }
        }

        private void ParseProperties(JToken obj, Context ctx)
        {

            foreach (var item in _properties)
                ctx.AppendProperty(item, obj[item.Name]);

        }


        private readonly List<MappingProperty> _properties;
        private readonly List<Parser> _sub;
        private List<string> _newLines;
        private List<(string, string)> _parentKeys;
        private readonly Parser _parent;


    }






}
