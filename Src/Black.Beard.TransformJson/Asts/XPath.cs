namespace Bb.TransformJson
{
    public class XPath : XsltJson
    {

        public XPath()
        {
            Kind = XsltKind.Xpath;
        }

        //public XsltJson Child { get; internal set; }

        public string Value { get; internal set; }
        public XPath Child { get; internal set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitXPath(this);
        }


    }

}
