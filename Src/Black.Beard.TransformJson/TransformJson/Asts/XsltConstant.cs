using System;

namespace Bb.TransformJson.Asts
{


    public class XsltConstant : XsltJson
    {

        public object Value { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitConstant(this);
        }

    }

}
