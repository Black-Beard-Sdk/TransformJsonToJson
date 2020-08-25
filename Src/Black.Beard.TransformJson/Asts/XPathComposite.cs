using System.Collections.Generic;

namespace Bb.TransformJson
{

    public class XPathComposite : XsltJson
    {

        public XPathComposite()
        {
            Kind = XsltKind.Path;
            this.Children = new List<XPathCompositeKey>();
        }

        public string Value { get; set; }


        public List<XPathCompositeKey> Children { get; }


        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitPathComposite(this);
        }

    }

}
