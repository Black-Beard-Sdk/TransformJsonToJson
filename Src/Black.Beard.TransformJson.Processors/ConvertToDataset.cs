using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bb.TransformJson
{




    //public class ConvertToDataset
    //{

    //    public ConvertToDataset(DataSet dataset = null)
    //    {
    //        this._dataset = dataset ?? new DataSet();
    //        //this._key = 1;
    //    }

    //    public void Import(JToken token, DataTable table, Guid parentKey)
    //    {

    //        if (token is JArray jarray)
    //            foreach (JToken item in jarray)
    //                Import(item, table, parentKey);

    //        else if (token is JObject jobject)
    //            ImportObject(jobject, table, parentKey);

    //    }

    //    public void ImportObject(JObject jObject, DataTable table, Guid parentKey)
    //    {

    //        var props = jObject.Properties();
    //        List<(string, object)> values = new List<(string, object)>();
    //        List<(string, JObject)> objects = new List<(string, JObject)>();
    //        List<(string, JArray)> arrays = new List<(string, JArray)>();

    //        foreach (JProperty property in props)
    //        {

    //            if (property.Value is JValue value)
    //            {

    //                DataColumn column = null;
    //                var name = property.Name;
    //                if (!table.Columns.Contains(name))
    //                    table.Columns.Add(column = new DataColumn(name, ResolveType(value)));

    //                values.Add((name, value.Value));

    //            }
    //            else if (property.Value is JObject o)
    //            {
    //                GetDatatable(property)
    //            }
    //            else if (property.Value is JArray a)
    //            {

    //            }

    //        }

    //        var row = table.NewRow();
    //        foreach (var item in values)
    //        {

    //        }

    //    }

    //    private Type ResolveType(JValue value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private DataTable GetDatatable(string name)
    //    {

    //        if (this._dataset.Tables.Contains(name))
    //            return this._dataset.Tables[name];

    //        var table = new DataTable(name);

    //        var key = new DataColumn("technicalKey", typeof(Guid));
    //        table.Columns.Add(key);

    //        this._dataset.Tables.Add(table);

    //        return table;

    //    }

    //    private readonly DataSet _dataset;
    //    private readonly long _key;

    //}

}
