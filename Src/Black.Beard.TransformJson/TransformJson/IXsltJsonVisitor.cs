using Bb.TransformJson.Asts;

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

        object VisitJPath(JPath node);

        object VisitMapProperty(XsltMapProperty node);
    }

}