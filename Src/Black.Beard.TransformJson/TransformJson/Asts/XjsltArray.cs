using System.Collections.Generic;

namespace Bb.TransformJson.Asts
{
    public class XjsltArray : XjsltJson
    {

        public XjsltArray(int count)
        {
            this.Kind = XjsltKind.Array;
        }


        internal XjsltJson Append(XjsltJson item)
        {
            _items.Add(item);
            return item;
        }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitArray(this);
        }

        public XjsltJson Item { get; internal set; }

        private readonly List<XjsltJson> _items;

    }

}
