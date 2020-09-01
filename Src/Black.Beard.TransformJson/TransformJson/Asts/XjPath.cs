using Newtonsoft.Json.Linq;
using System.Security.Principal;
using System.Threading;
using System.Xml;

namespace Bb.TransformJson.Asts
{

    public class XjPath : XjsltJson
    {

        public XjPath()
        {
            Kind = XjsltKind.Jpath;
        }

        //public XsltJson Child { get; internal set; }

        public string Value { get; internal set; }

        public XjsltJson Child { get; internal set; }

        public string Type { get; internal set; }

        public XjsltObject TypeObject { get; internal set; }

        public override object Accept(IXsltJsonVisitor visitor)
        {
            return visitor.VisitJPath(this);
        }

    }

}
