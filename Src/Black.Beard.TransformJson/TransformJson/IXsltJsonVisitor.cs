using Bb.TransformJson.Asts;

namespace Bb.TransformJson
{

    public interface IXsltJsonVisitor
    {


        TranformJsonAstConfiguration Configuration { get; set; }

        object VisitArray(XjsltArray node);
        
        object VisitConstant(XjsltConstant node);
        
        object VisitObject(XjsltObject node);
        
        object VisitProperty(XjsltProperty node);
        
        object VisitType(XjsltType node);

        object VisitJPath(XjPath node);

        object VisitMapProperty(XsltMapProperty node);
    }

}