using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Bb.TransformJson
{
    public class XsltType : XsltJson
    {

        public XsltType(XsltObject o)
        {
            this.Kind = XsltKind.Type;
            this._items = new Dictionary<string, XsltProperty>();

            foreach (var item in o.Properties)
                if (item.Name == TransformJsonConstants.Type)
                    this.Type = (item.Value as XsltConstant).Value.ToString();
                else
                    this._items.Add(item.Name, item);

        }

        public IEnumerable<XsltProperty> Properties { get => _items.Values; }

        public string Type { get; internal set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitType(this);
        }

        private readonly Dictionary<string, XsltProperty> _items;

    }

}
