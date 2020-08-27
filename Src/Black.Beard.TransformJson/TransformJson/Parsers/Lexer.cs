using System;

namespace Bb.TransformJson.Parsers
{
    public class Lexer
    {

        public Lexer(string rule)
        {
            this._rule = rule;
            this._index = 0;
            this._length = this._rule.Length;
        }

        public Kind GetNext()
        {

            if (_index == this._rule.Length)
                return Kind.EOF;

            this._currentChar = this._rule[_index++];

            switch (this._currentChar)
            {

                case '"':
                    return _current = Kind.DblQuote;

                case '{':
                    return _current = Kind.Left;

                case '}':
                    return _current = Kind.Right;

                case '|':
                    return _current = Kind.Pipe;

                case ':':
                    return _current = Kind.Colon;

                default:

                    if (char.IsLetter(_currentChar))
                        return _current = Kind.Letter;

                    if (char.IsDigit(_currentChar))
                        return _current = Kind.Digit;

                    if (char.IsPunctuation(_currentChar))
                        return _current = Kind.Ponctuation;

                    return _current = Kind.Other;

            }

        }

        public Char CurrentChar { get => _currentChar; }

        public int Index { get => _index - 1; }

        public Kind Current { get => _current; }

        private string _rule;
        private int _index;
        private readonly int _length;
        private char _currentChar;
        private Kind _current;
    }

}
