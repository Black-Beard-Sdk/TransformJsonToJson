using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bb.DifferenceJson
{

    public class DiffJsonComparer
    {

        public DiffJsonComparer(CompareJsonContext context)
        {
            this._context = context;
        }

        public CompareResult Compare(JToken fromJson, JToken toJson)
        {

            return CompareJToken(fromJson, toJson);

        }

        private CompareResult CompareJToken(JToken fromJson, JToken toJson)
        {

            CompareResult result = null;

            switch (fromJson.Type)
            {

                case JTokenType.Object:
                    result = CompareObject(fromJson as JObject, toJson);
                    break;

                case JTokenType.Property:
                    result = CompareProperty(fromJson as JProperty, toJson);
                    break;

                case JTokenType.Array:
                    result = CompareArray(fromJson as JArray, toJson);
                    break;

                case JTokenType.Bytes:
                case JTokenType.Float:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                case JTokenType.Date:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Integer:
                case JTokenType.String:
                    result = CompareValue(fromJson as JValue, toJson);
                    break;

                case JTokenType.Raw:
                    break;

                case JTokenType.None:
                case JTokenType.Comment:
                case JTokenType.Undefined:
                case JTokenType.Constructor:
                default:
                    break;
            }

            return result;

        }

        private CompareResult CompareValue(JValue fromJson, JToken toJson)
        {

            if (toJson.Type == fromJson.Type)
                return CompareValue(fromJson, toJson as JValue);

            return new CompareResult()
            {
                Path = fromJson.Path,
                Old = fromJson,
                New = toJson,
                Type = CompareResultType.Changed,
            };

        }

        private CompareResult CompareValue(JValue fromJson, JValue toJson)
        {

            if (fromJson.Value != toJson.Value)
            {
                return new CompareResult()
                {
                    Path = fromJson.Path,
                    Old = fromJson,
                    New = toJson,
                    Type = CompareResultType.Changed,
                };
            }

            return null;

        }

        private CompareResult CompareProperty(JProperty fromJson, JToken toJson)
        {

            if (toJson.Type == JTokenType.Property)
                return CompareProperty(fromJson, toJson as JProperty);

            return new CompareResult()
            {
                Path = fromJson.Path,
                Old = fromJson,
                New = toJson,
                Type = CompareResultType.Changed,
            };

        }

        private CompareResult CompareProperty(JProperty fromJson, JProperty toJson)
        {
            var result = CompareJToken(fromJson.Value, toJson.Value);
            if (result != null)
                result.Name = fromJson.Name;
            return result;
        }

        private CompareResult CompareObject(JObject fromJson, JToken toJson)
        {

            if (toJson.Type == JTokenType.Object)
                return CompareObject(fromJson, toJson as JObject);

            return new CompareResult()
            {
                Path = fromJson.Path,
                Old = fromJson,
                New = toJson,
                Type = CompareResultType.Changed,
            };

        }

        private CompareResult CompareObject(JObject fromJson, JObject toJson)
        {

            List<CompareResult> r1 = CompareProperties(fromJson, toJson);
            List<CompareResult> r2 = CompareProperties(toJson, fromJson);

            if (r1.Count > 0 || r2.Count > 0)
            {

                CompareResult result = new CompareResult()
                {
                    Path = fromJson.Path,
                    Old = fromJson,
                    New = toJson,
                    Type = CompareResultType.Changed,
                };

                result.Subs.AddRange(r1);
                foreach (var item in r2.Where(c => c.Type == CompareResultType.Removed))
                {
                    item.Type = CompareResultType.Added;
                    result.Subs.Add(item);
                }

                return result;
            }

            return null;

        }

        private List<CompareResult> CompareProperties(JObject fromJson, JObject toJson)
        {
            List<CompareResult> r = new List<CompareResult>();

            var o = toJson
                .Properties()
                .ToDictionary(c => c.Name);

            foreach (var item in fromJson.Properties())
            {

                CompareResult resultSub = null;

                if (o.TryGetValue(item.Name, out JProperty p1))
                    resultSub = CompareJToken(item, p1);

                else
                {
                    resultSub = new CompareResult()
                    {
                        Path = fromJson.Path,
                        Name = item.Name,
                        Old = fromJson,
                        New = null,
                        Type = CompareResultType.Removed,
                    };
                }

                if (resultSub != null)
                    r.Add(resultSub);

            }

            return r;
        }

        private CompareResult CompareArray(JArray fromJson, JToken toJson)
        {

            if (toJson.Type == JTokenType.Array)
                return CompareArray(fromJson, toJson as JArray);

            return new CompareResult()
            {
                Path = fromJson.Path,
                Old = fromJson,
                New = toJson,
                Type = CompareResultType.Changed,
            };

        }

        private CompareResult CompareArray(JArray fromJson, JArray toJson)
        {

            throw new NotImplementedException();

        }

        private readonly CompareJsonContext _context;


    }

}
