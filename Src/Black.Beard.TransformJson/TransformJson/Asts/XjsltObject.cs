using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Bb.TransformJson.Asts
{


    public class XjsltObject : XjsltJson
    {

        public XjsltObject()
        {
            this.Kind = XjsltKind.Object;
            this._items = new Dictionary<string, XjsltProperty>();
        }


        internal void Append(XjsltProperty property)
        {
            if (property.Name == TransformJsonConstants.Source)
                this.Source = property.Value;
            
            else 
                _items.Add(property.Name, property);
        }

        public IEnumerable<XjsltProperty> Properties { get => _items.Values; }

        public string Name { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitObject(this);
        }

        private readonly Dictionary<string, XjsltProperty> _items;

        internal void AddProperty(XjsltProperty prop)
        {
            this._items.Add(prop.Name, prop);
        }

    }

}
