using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.TransformJson
{

    public class ConfigurationBuilder : IXsltJsonVisitor
    {

        public ConfigurationBuilder()
        {

            this._ctorJProperty = typeof(JProperty).GetConstructor(new Type[] { typeof(string), typeof(object) });
            this._ctorJObject = typeof(JObject).GetConstructor(new Type[] { typeof(object) });
            this._ctorJArray = typeof(JArray).GetConstructor(new Type[] { typeof(object[]) });

            this.Argument = Expression.Parameter(typeof(JToken), "arg1");
            this.Context = Expression.Parameter(typeof(RuntimeContext), "arg0");
        }


        public object VisitArray(XsltArray node)
        {

            var source = (Expression)node.Source.Accept(this);

            List<Expression> _items = new List<Expression>();

            var o = (Expression)node.Item.Accept(this);
            var lbd = Expression.Lambda<Func<RuntimeContext, JToken, JToken>>(o, Context, Argument);
            
            var i = Expression.Call(RuntimeContext._getProjectionFromSource.Method, this.Context, source, lbd);

            return i;

        }

        public object VisitConstant(XsltConstant node)
        {
            return Expression.Constant(node.Value);
        }

        public object VisitObject(XsltObject node)
        {

            List<Expression> _properties = new List<Expression>();

            if (_parent == null)
                _parent = this.Argument;

            foreach (var item in node.Properties)
                _properties.Add((Expression)item.Accept(this));

            return Expression.New(_ctorJObject, _properties.ToArray());

        }

        public object VisitType(XsltType node)
        {
            throw new NotImplementedException();
        }

        public object VisitProperty(XsltProperty node)
        {

            var name = Expression.Constant(node.Name);
            var value = (Expression)node.Value.Accept(this);

            return Expression.New(_ctorJProperty, name, value);

        }

        public object VisitXPath(XPath node)
        {

            Expression resultChild = null;

            if (node.Child != null)
                resultChild = (Expression)node.Child.Accept(this);

            if (node.Type == "jpath")
                return Expression.Call(RuntimeContext._getContentByJPath.Method, this.Context, resultChild ?? _parent, Expression.Constant(node.Value));

            if (node.TypeObject != null)
            {
                var service = this.Configuration.Services.GetService(new XsltType(node.TypeObject) { Type = node.Type });
                if (service != null)
                    return Expression.Call(RuntimeContext._getContentFromService.Method, this.Context, resultChild ?? _parent, Expression.Constant(service));
            
            }

            throw new NotImplementedException(node.Type);

        }

        public TranformJsonAstConfiguration Configuration { get; set; }

        public ParameterExpression Argument { get; }
        public ParameterExpression Context { get; }

        public Expression _parent;

        private ConstructorInfo _ctorJArray;
        private readonly ConstructorInfo _ctorJObject;
        private readonly ConstructorInfo _ctorJProperty;



    }

}
