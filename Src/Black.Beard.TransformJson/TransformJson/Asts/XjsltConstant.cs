using System;

namespace Bb.TransformJson.Asts
{


    public class XjsltConstant : XjsltJson
    {

        public object Value { get; set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitConstant(this);
        }

    }

}
