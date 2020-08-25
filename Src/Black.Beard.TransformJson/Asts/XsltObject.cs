using System.Collections.Generic;

namespace Bb.TransformJson
{


    public class XsltObject : XsltJson
    {

        public XsltObject()
        {
            this.Kind = XsltKind.Object;
            this._items = new Dictionary<string, XsltProperty>();
        }


        internal void Append(XsltProperty property)
        {
            if (property.Name == TransformJsonConstants.Source)
                this.Source = property.Value;
            
            else 
                _items.Add(property.Name, property);
        }

        internal bool IsType { get => _items.ContainsKey(TransformJsonConstants.Type); }

        public IEnumerable<XsltProperty> Properties { get => _items.Values; }

        public string Name { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitObject(this);
        }

        private readonly Dictionary<string, XsltProperty> _items;

    }

}
