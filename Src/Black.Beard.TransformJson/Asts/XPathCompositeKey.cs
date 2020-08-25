namespace Bb.TransformJson
{

    public class XPathCompositeKey : XsltJson
    {

        public XPathCompositeKey()
        {
            this.Kind = XsltKind.PathKey;
        }

        public string Value { get; internal set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitPathKey(this);
        }

    }


}
