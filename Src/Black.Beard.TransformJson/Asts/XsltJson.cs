using System;

namespace Bb.TransformJson
{

    public abstract class XsltJson
    {

        public XsltKind Kind { get; internal set; }

        public abstract object Accept(IXsltJsonVisitor visitor);


        public XsltJson Source { get; internal set; }


    }

}
