namespace Bb.TransformJson
{

    public interface IXsltJsonVisitor
    {


        TranformJsonAstConfiguration Configuration { get; set; }


        object VisitArray(XsltArray node);
        object VisitConstant(XsltConstant node);
        object VisitObject(XsltObject node);
        object VisitProperty(XsltProperty node);
        object VisitType(XsltType node);

        object VisitXPath(XPath node);

        //object VisitIndice(XPathIndice node);
        //object VisitPathKey(XPathCompositeKey node);
        //object VisitPathComposite(XPathComposite node);
        //object VisitPathCoalesce(XPathCoalesce node);

    }

}