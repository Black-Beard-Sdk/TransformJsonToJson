namespace Bb.TransformJson
{
    public class XPathCoalesce : XsltJson
    {

        public XPathCoalesce()
        {
            this.Kind = XsltKind.PathCoalesce;
        }

        public XsltJson ChildLeft { get; set; }

        public XsltJson ChildRight { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitPathCoalesce(this);
        }

    }

}
