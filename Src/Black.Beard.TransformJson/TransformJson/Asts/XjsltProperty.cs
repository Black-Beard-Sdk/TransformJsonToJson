namespace Bb.TransformJson.Asts
{

    public class XjsltProperty : XjsltJson
    {

        public XjsltProperty()
        {
            this.Kind = XjsltKind.Property;
        }

        public string Name{ get; set; }

        public XjsltJson Value { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitProperty(this);
        }

    }

    public class XsltMapProperty : XjsltJson
    {

        public XsltMapProperty()
        {
            this.Kind = XjsltKind.Property;
        }

        public string Name { get; set; }

        public XjsltJson Value { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitMapProperty(this);
        }

    }

}
