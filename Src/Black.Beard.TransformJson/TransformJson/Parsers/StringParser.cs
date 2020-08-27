using Bb.TransformJson.Asts;
using System.Collections.Generic;
using System.Text;

namespace Bb.TransformJson.Parsers
{
    public class StringParser
    {

        public StringParser(string rule)
        {
            this._rule = rule;
            this._stack = new Stack<JPath>();
        }

        public XsltJson Get()
        {

            this._lexer = new Lexer(_rule);

            var tokens = Prepare();
            if (tokens.Count > 1)
            {
                var p = ParseRoot(tokens);
                return p;
            }

            return new XsltConstant() { Value = _rule };
        }

        private Queue<Token> Prepare()
        {
            int i = 0;
            List<Token> tokens = new List<Token>();

            StringBuilder sb = new StringBuilder();

            while (_lexer.GetNext() != Kind.EOF)
                switch (_lexer.Current)
                {
                    case Kind.Left:
                        tokens.Add(new Token() { Kind = KindToken.Left, Index = _lexer.Index });
                        i++;
                        break;

                    case Kind.Right:
                        if (sb.Length > 0)
                        {
                            var txt = sb.ToString().Trim().Trim('"');
                            if (!string.IsNullOrEmpty(txt))
                            {
                                tokens.Add(new Token() { Text = txt, Kind = KindToken.Identifier, Index = _lexer.Index });
                                sb.Clear();
                            }
                        }
                        tokens.Add(new Token() { Kind = KindToken.Right, Index = _lexer.Index });
                        i--;
                        break;

                    case Kind.Pipe:
                        tokens.Add(new Token() { Kind = KindToken.Pipe, Index = _lexer.Index });
                        break;

                    case Kind.Colon:
                        if (i == 0)
                        {
                            tokens.Add(new Token() { Text = sb.ToString().Trim().Trim('"'), Kind = KindToken.Identifier, Index = _lexer.Index });
                            sb.Clear();
                        }
                        else
                            sb.Append(_lexer.CurrentChar);

                        break;

                    case Kind.DblQuote:
                    case Kind.Ponctuation:
                    case Kind.Letter:
                    case Kind.Other:
                    case Kind.Digit:
                        sb.Append(_lexer.CurrentChar);
                        break;

                    default:
                        break;

                }

            if (sb.Length > 0)
                tokens.Add(new Token() { Text = sb.ToString().Trim().Trim('"'), Kind = KindToken.Identifier, Index = _lexer.Index });

            return new Queue<Token>(tokens);

        }

        private JPath ParseRoot(Queue<Token> tokens)
        {

            JPath prop = null;

            while (tokens.Count > 0)
            {
                var token = tokens.Dequeue();
                switch (token.Kind)
                {
                    case KindToken.Identifier:
                        if (prop != null)
                            throw new ParsingException(token.Text, token.Index);
                        prop = new JPath() { Type = token.Text.ToLower() };
                        break;

                    case KindToken.Left:
                        if (prop == null)
                            throw new ParsingException(token.Text, token.Index);
                        if (prop.Type == "jpath")
                            ParseJpath(tokens, prop);
                        else
                            ParseFunction(tokens, prop);
                        break;

                    case KindToken.Pipe:
                        ParseFunction(tokens, prop);
                        break;

                    case KindToken.Right:
                        break;

                    default:
                        break;
                }


            }

            return prop;

        }

        private void ParseJpath(Queue<Token> tokens, JPath prop)
        {

            while (tokens.Count > 0)
            {
                var token = tokens.Dequeue();
                switch (token.Kind)
                {
                    case KindToken.Identifier:
                        prop.Value = token.Text;
                        break;

                    case KindToken.Right:
                        return;

                    default:
                        throw new ParsingException(token.Text, token.Index);

                }

            }

        }

        private void ParseFunction(Queue<Token> tokens, JPath propRoot)
        {

            XsltObject o = null;
            string type = string.Empty;

            while (tokens.Count > 0)
            {
                var token = tokens.Dequeue();
                switch (token.Kind)
                {
                    case KindToken.Identifier:
                        type = token.Text;
                        o = new XsltObject() { };
                        break;

                    case KindToken.Left:
                        if (o == null)
                            throw new ParsingException(token.Text, token.Index);
                        ParseObject(tokens, o);
                        propRoot.Child = new XsltType(o) { Type = type };
                        break;

                    case KindToken.Pipe:
                        throw new ParsingException(token.Text, token.Index);

                    case KindToken.Right:
                        return;

                    default:
                        break;
                }

            }

        }

        private void ParseObject(Queue<Token> tokens, XsltObject o)
        {
            XsltProperty prop = null;
            while (tokens.Count > 0)
            {
                var token = tokens.Dequeue();
                switch (token.Kind)
                {
                    case KindToken.Identifier:
                        prop = new XsltProperty() { Name = token.Text };
                        o.AddProperty(prop);
                        ParseProperty(tokens, prop);
                        break;

                    case KindToken.Left:
                        break;

                    case KindToken.Pipe:
                        break;

                    case KindToken.Right:
                        break;

                    default:
                        break;
                }

            }

        }

        private void ParseProperty(Queue<Token> tokens, XsltProperty prop)
        {
            JPath p = null;

            while (tokens.Count > 0)
            {
                var token = tokens.Dequeue();
                switch (token.Kind)
                {
                    case KindToken.Identifier:
                        p = new JPath() { Type = token.Text.ToLower() };
                        prop.Value = p;
                        break;

                    case KindToken.Left:
                        if (p == null)
                            throw new ParsingException(token.Text, token.Index);
                        if (p.Type == "jpath")
                            ParseJpath(tokens, p);
                        else
                            ParseFunction(tokens, p);
                        break;

                    case KindToken.Pipe:
                        ParseFunction(tokens, p);
                        break;

                    case KindToken.Right:
                        break;

                    default:
                        break;
                }

            }
        }

        private Lexer _lexer;
        private readonly string _rule;
        private readonly Stack<JPath> _stack;
    }

}
