using Bb.TransformJson.Asts;
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

            var root = _parent;


            Expression expressionCreatetService = Expression.Call(Expression.Constant(node.ServiceProvider), TransformJsonServiceProvider.Method);

            foreach (var property in node.Properties)
            {
                _parent = root;
                var value = (Expression)property.Value.Accept(this);
                expressionCreatetService = Expression.Call(null, RuntimeContext._mapPropertyService.Method, this.Context, expressionCreatetService, Expression.Constant(property.Name), value);
            }

            _parent = root;

            var result = Expression.Call(RuntimeContext._getContentFromService.Method, this.Context, _parent, expressionCreatetService);

            return result;

        }

        public object VisitMapProperty(XsltMapProperty node)
        {

            var value = (Expression)node.Value.Accept(this);

            Expression.Call(Context, null, Expression.Constant(node.Name), value);

            return null;

        }


        public object VisitProperty(XsltProperty node)
        {

            var name = Expression.Constant(node.Name);
            var value = (Expression)node.Value.Accept(this);

            return Expression.New(_ctorJProperty, name, value);

        }

        public object VisitJPath(JPath node)
        {

            if (node.Type == "jpath")
                _parent = Expression.Call(RuntimeContext._getContentByJPath.Method, this.Context, _parent, Expression.Constant(node.Value));

            else
            {

            }

            if (node.Child != null)
                _parent = (Expression)node.Child.Accept(this);

            return _parent;

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
