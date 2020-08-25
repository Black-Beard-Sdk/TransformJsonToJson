using System.Collections.Generic;

namespace Bb.TransformJson
{
    public class XsltArray : XsltJson
    {

        public XsltArray(int count)
        {
            this.Kind = XsltKind.Array;
        }


        internal XsltJson Append(XsltJson item)
        {
            _items.Add(item);
            return item;
        }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitArray(this);
        }

        public XsltJson Item { get; internal set; }

        private readonly List<XsltJson> _items;

    }

}
