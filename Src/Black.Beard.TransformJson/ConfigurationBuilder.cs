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

            this.Argument = Expression.Parameter(typeof(JToken), "arg0");

        }


        public object VisitArray(XsltArray node)
        {

            var source = (Expression)node.Source.Accept(this);

            List<Expression> _items = new List<Expression>();

            var o = (Expression)node.Item.Accept(this);
            var lbd = Expression.Lambda<Func<JToken, JToken>>(o, Argument);
            
            var i = Expression.Call(RuntimeContext._getProjectionFromSource.Method, source, lbd);

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

            var item = Expression.Call(RuntimeContext._getContentByName.Method, resultChild ?? _parent, Expression.Constant(node.Value));

            return item;

        }

        //public object VisitIndice(XPathIndice node)
        //{
        //    _parent = Expression.Call(RuntimeContext._getIndice.Method, _parent, Expression.Constant(node.Indice), Expression.Constant(node.Value == "last"));
        //    return _parent;
        //}

        //public object VisitPathComposite(XPathComposite node)
        //{
        //    Expression result = null;
        //    _parent = this.Argument;
        //    foreach (var item in node.Children)
        //        result = item.Accept(this) as Expression;
        //    return result;
        //}

        //public object VisitPathKey(XPathCompositeKey node)
        //{
        //    if (node.Kind == XsltKind.PathParent)
        //        _parent = Expression.Call(RuntimeContext._getParent.Method, _parent, Expression.Constant(node.Value));
        //    else
        //        _parent = Expression.Call(RuntimeContext._getContentByName.Method, _parent, Expression.Constant(node.Value));
        //    return _parent;
        //}

        //public object VisitPathCoalesce(XPathCoalesce node)
        //{
        //    var l = node.ChildLeft.Accept(this) as Expression;
        //    var r = node.ChildRight.Accept(this) as Expression;
        //    return Expression.Coalesce(l, r);
        //}


        public TranformJsonAstConfiguration Configuration { get; set; }

        public ParameterExpression Argument { get; }
        public Expression _parent;

        private ConstructorInfo _ctorJArray;
        private readonly ConstructorInfo _ctorJObject;
        private readonly ConstructorInfo _ctorJProperty;



    }

}
