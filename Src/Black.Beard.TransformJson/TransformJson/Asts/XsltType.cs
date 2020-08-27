using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.TransformJson.Asts
{
    public class XsltType : XsltJson
    {

        public XsltType(XsltObject o)
        {
            this.Kind = XsltKind.Type;
            this._items = new Dictionary<string, XsltMapProperty>();

            foreach (var item in o.Properties.Where(c=> c.Name.StartsWith("$")))
                if (item.Name == TransformJsonConstants.Source)
                    this.Type = (item.Value as XsltConstant).Value.ToString();
                else
                    this._items.Add(item.Name, new XsltMapProperty() { Name = item.Name, Value = item.Value });

        }

        public IEnumerable<XsltMapProperty> Properties { get => _items.Values; }

        public string Type { get; internal set; }
        public TransformJsonServiceProvider ServiceProvider { get; internal set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitType(this);
        }

        private readonly Dictionary<string, XsltMapProperty> _items;

    }

}
