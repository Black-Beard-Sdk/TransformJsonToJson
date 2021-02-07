using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Bb.ConvertToDatables
{
    internal class MappingProperty
    {

        public MappingProperty(DataSet dataset)
        {
            this._dataset = dataset;
        }

        public string Name { get; set; }

        public DataColumn Column { get; set; }
        public DataTable Table { get; private set; }

        internal DataColumn GetTargetPath(JValue value, string name)
        {

            var valuePath = value.Value.ToString().Split('.');

            var tableName = valuePath[0];
            if (this._dataset.Tables.Contains(tableName))
                this.Table = this._dataset.Tables[tableName];

            else
            {
                Table = new DataTable() { TableName = tableName };
                var key = new DataColumn("technicalKey", typeof(Guid));
                Table.Columns.Add(key);
                Table.PrimaryKey = new DataColumn[] { key };
                this._dataset.Tables.Add(Table);
            }

            var nn = name;
            if (valuePath.Length == 2)
                nn = valuePath[1];
            if (!Table.Columns.Contains(nn))
            {
                Column = new DataColumn(nn);
                Table.Columns.Add(Column);
            }

            return Column;

        }


        private readonly DataSet _dataset;


    }


}
