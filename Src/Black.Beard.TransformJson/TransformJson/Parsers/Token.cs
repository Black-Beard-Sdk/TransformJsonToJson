namespace Bb.TransformJson.Parsers
{
    public class Token
    {
        public string Text { get; internal set; }

        public KindToken Kind { get; internal set; }
        public int Index { get; internal set; }

        public override string ToString()
        {

            if (this.Kind == KindToken.Identifier)
                return Text;

            return Kind.ToString();

        }
    }

}
