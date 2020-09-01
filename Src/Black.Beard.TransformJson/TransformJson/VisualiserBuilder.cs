using Bb.TransformJson.Asts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bb.TransformJson
{

    internal class VisualiserBuilder : IXsltJsonVisitor
    {


        public object VisitArray(XjsltArray node)
        {
            throw new NotImplementedException();
        }

        public object VisitConstant(XjsltConstant node)
        {
            throw new NotImplementedException();
        }

        public object VisitJPath(XjPath node)
        {
            throw new NotImplementedException();
        }

        public object VisitMapProperty(XsltMapProperty node)
        {
            throw new NotImplementedException();
        }

        public object VisitObject(XjsltObject node)
        {
            throw new NotImplementedException();
        }

        public object VisitProperty(XjsltProperty node)
        {
            throw new NotImplementedException();
        }

        public object VisitType(XjsltType node)
        {
            throw new NotImplementedException();
        }


        public TranformJsonAstConfiguration Configuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    }
}
