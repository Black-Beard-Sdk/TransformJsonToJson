using System;

namespace Bb.TransformJson.Asts
{

    public abstract class XjsltJson
    {

        public XjsltKind Kind { get; internal set; }

        public abstract object Accept(IXsltJsonVisitor visitor);


        public XjsltJson Source { get; internal set; }


    }

}
