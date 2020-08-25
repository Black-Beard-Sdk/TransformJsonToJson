namespace Bb.TransformJson
{
    public class XPathIndice : XPathCompositeKey
    {

        public XPathIndice()
        {
            this.Kind = XsltKind.PathIndice;
        }

        public int Indice { get; internal set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitIndice(this);
        }

    }


}
