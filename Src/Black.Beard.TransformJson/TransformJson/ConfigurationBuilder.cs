using Bb.Expresssions;
using Bb.TransformJson.Asts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

namespace Bb.TransformJson
{

    public class ConfigurationBuilder : IXsltJsonVisitor
    {

        public ConfigurationBuilder()
        {

            this._ctorJProperty = typeof(JProperty).GetConstructor(new Type[] { typeof(string), typeof(object) });
            this._ctorJValue = typeof(JValue).GetConstructor(new Type[] { typeof(object) });
            this._ctorJObject = typeof(JObject).GetConstructor(new Type[] { });
            //this._AddJObject = typeof(JObject).GetMethod("Add", new Type[] { typeof(object) });
            this._AddJObject2 = typeof(RuntimeContext).GetMethod("AddProperty", new Type[] { typeof(JObject), typeof(JProperty) });
            this._ctorJArray = typeof(JArray).GetConstructor(new Type[] { typeof(object[]) });
            this._AddJArray = typeof(JArray).GetMethod("Add", new Type[] { typeof(object) });
            this._propJArray_Item = typeof(JArray).GetProperty("Item", new Type[] { typeof(Int32) });

            this.source = new MethodCompiler();

            var Context = this.source.AddParameter(typeof(RuntimeContext), "argContext");
            var Argument = this.source.AddParameter(typeof(JToken), "argToken");

            _stack.Push(new BuildContext()
            {
                Argument = Argument,
                Context = Context,
                RootSource = Argument,
                RootTarget = null,
                Source = this.source

            }); ;

        }



        public object VisitArray(XjsltArray node)
        {

            using (var ctx = NewContext())
            {

                var src = ctx.Current.Source;

                var targetArray = src.AddVar(typeof(JArray), null, typeof(JArray).CreateObject());
                ctx.Current.RootTarget = targetArray;

                if (node.Source != null)
                {

                    var resultToken = src.AddVar((typeof(JToken)), null, (Expression)node.Source.Accept(this));

                    // Case when result is array
                    var i = src.If(resultToken.TypeIs(typeof(JArray)));
                    var listArray = i.Then.AddVar(typeof(JArray), null, resultToken.ConvertIfDifferent(typeof(JArray)));

                    var _if = i.Then.For(Expression.Constant(0), listArray.Property("Count"));
                    _if.Where = Expression.LessThan(_if.Index, listArray.Property("Count"));
                    ctx.Current.Source = _if.Body;

                    var itemList = _if.Body.AddVar((typeof(JToken)), null, Expression.Property(listArray, _propJArray_Item, _if.Index));
                    ctx.Current.RootSource = itemList;

                    var b = (Expression)node.Item.Accept(this);
                    _if.Body.Add(targetArray.Call(this._AddJArray, b));


                    // Case when else
                    ctx.Current.Source = i.Else;
                    ctx.Current.RootSource = resultToken;
                    b = (Expression)node.Item.Accept(this);
                    i.Else.Add(targetArray.Call(this._AddJArray, b));

                }

                return targetArray;

            }


        }

        internal Func<RuntimeContext, JToken, JToken> Compile(XjsltJson tree)
        {
            Expression e;

            e = tree.Accept(this) as Expression;
            source.Add(e);

            var result = source.Compile<Func<RuntimeContext, JToken, JToken>>();

            return result;

        }

        public object VisitConstant(XjsltConstant node)
        {
            return Expression.New(_ctorJValue, Expression.Constant(node.Value));
        }

        public object VisitObject(XjsltObject node)
        {


            using (var ctx = this.NewContext())
            {

                var v1 = ctx.Current.Source.AddVar(typeof(JObject), null, _ctorJObject.CreateObject());
                ctx.Current.RootTarget = v1;

                if (node.Source != null)
                {

                    var src = ctx.Current.Source;

                    var resultToken = src.AddVar((typeof(JToken)), null, (Expression)node.Source.Accept(this));
                    ctx.Current.RootSource = resultToken;

                    foreach (var item in node.Properties)
                    {
                        var prop = (Expression)item.Accept(this);
                        //ctx.Current.Source.Add(v1.Call(_AddJObject, prop));
                        ctx.Current.Source.Add(_AddJObject2.Call(v1, prop));
                    }
                   
                }
                else
                {

                    foreach (var item in node.Properties)
                    {
                        var prop = (Expression)item.Accept(this);
                        //ctx.Current.Source.Add(v1.Call(_AddJObject, prop));
                        ctx.Current.Source.Add(_AddJObject2.Call(v1, prop));
                    }
                }

                return v1;

            }

        }

        public object VisitProperty(XjsltProperty node)
        {
            var name = Expression.Constant(node.Name);
            var getValue = (Expression)node.Value.Accept(this);
            return this._ctorJProperty.CreateObject(name, getValue);
        }

        public object VisitType(XjsltType node)
        {

            using (var ctx = NewContext())
            {

                Expression expressionCreatetService = Expression.Call(Expression.Constant(node.ServiceProvider), TransformJsonServiceProvider.Method);

                foreach (var property in node.Properties)
                {
                    var value = (Expression)property.Value.Accept(this);
                    expressionCreatetService = Expression.Call(null, RuntimeContext._mapPropertyService.Method, ctx.Current.Context, expressionCreatetService, Expression.Constant(property.Name), value);
                }

                var result = Expression.Call(RuntimeContext._getContentFromService.Method, ctx.Current.Context, ctx.Current.RootSource, expressionCreatetService);

                return result;

            }

        }

        public object VisitMapProperty(XsltMapProperty node)
        {

            var ctx = BuildCtx;

            var value = (Expression)node.Value.Accept(this);

            Expression.Call(ctx.Context, null, Expression.Constant(node.Name), value);

            return null;

        }

        public object VisitJPath(XjPath node)
        {

            using (var ctx = NewContext())
            {


                if (node.Type == "jpath")
                    ctx.Current.RootSource = Expression.Call(RuntimeContext._getContentByJPath.Method, ctx.Current.Context, ctx.Current.RootSource, Expression.Constant(node.Value));

                else
                {

                }

                if (node.Child != null)
                    ctx.Current.RootSource = (Expression)node.Child.Accept(this);

                return ctx.Current.RootSource;

            }

        }

        public TranformJsonAstConfiguration Configuration { get; set; }

        //public ParameterExpression Argument { get; }
        //public ParameterExpression Context { get; }


        private ConstructorInfo _ctorJArray;
        private readonly MethodInfo _AddJArray;
        private readonly PropertyInfo _propJArray_Item;
        private readonly MethodCompiler source;
        private readonly ConstructorInfo _ctorJProperty;
        private readonly ConstructorInfo _ctorJValue;
        private readonly ConstructorInfo _ctorJObject;
        //private readonly MethodInfo _AddJObject;
        private readonly MethodInfo _AddJObject2;

        private BuildContext BuildCtx
        {
            get => _stack.Peek();
        }

        private CurrentContext NewContext()
        {

            var ctx = _stack.Peek();

            var cts = new BuildContext()
            {
                Argument = ctx.Argument,
                Context = ctx.Context,
                RootSource = ctx.RootSource,
                RootTarget = ctx.RootTarget,
                Source = ctx.Source,
            };

            _stack.Push(cts);

            Action act = () =>
            {
                _stack.Pop();
            };

            return new CurrentContext(act, cts);

        }

        private class CurrentContext : IDisposable
        {

            public CurrentContext(Action act, BuildContext current)
            {
                this.action = act;
                this.Current = current;
            }

            public void Dispose()
            {
                action();
            }

            private Action action;

            public BuildContext Current { get; }

        }

        private Stack<BuildContext> _stack = new Stack<BuildContext>();

        private class BuildContext
        {

            public ParameterExpression Argument;

            public ParameterExpression Context;

            public Expression RootSource;

            public Expression RootTarget;

            public SourceCode Source;
        }

    }

}
