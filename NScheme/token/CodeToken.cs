using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    namespace compiler
    {
        public class CodeToken
        {
            public TokenType Type { get; private set; }
            public String Value { get; private set; }
            private int hashCache;

            public int LineNumber = -1;
            public int ColumeNumber = -1;

            public new String ToString()
            {
                return String.Format("Ln[{0}] Col[{1}] type[{2}] value[{3}]\r\n",
                                        LineNumber, ColumeNumber, Type, Value);
            }

            protected CodeToken(TokenType type, String value) 
            {
                Type = type;
                Value = value;
                hashCache = type.GetHashCode();
                if (value != null)
                    hashCache = hashCache * value.GetHashCode();
            }

            public override int GetHashCode()
            {
                return hashCache;
            }

            public static CodeToken Build(TokenType type, String value)
            {
                TokenType tp;
                if (type == TokenType.Identifier)
                {
                    switch (value)
                    {
                        // reserved  keywords
                        case "and": tp = TokenType.And;     value = null; break;
                        case "or": tp = TokenType.Or;       value = null; break;
                        case "not": tp = TokenType.Not;     value = null; break;

                        case "def": tp = TokenType.Def; value = null; break;
                        case "undef": tp = TokenType.UnDef; break;
                        case "func": tp = TokenType.Func;   value = null; break;
                        case "if": tp = TokenType.If; break;
                        case "begin": tp = TokenType.Begin; break;
                        case "list": tp = TokenType.List; break;
                        case "#t": tp = TokenType.True; break;
                        case "#f": tp = TokenType.False; break;
                       // case "elif": tp = TokenType.Elif; break;
                        //case "else": tp = TokenType.Else; break;

                        default: tp = TokenType.Identifier; break;
                    }
                }
                else if (type == TokenType.String)
                {
                    tp = TokenType.String;
                    value = value.UnEscapeStr();
                }
                else
                {
                    tp = type;
                }
                return new CodeToken(tp, value);
            }

        }

        //public class ValueToken : CodeToken
        //{
        //    public double Fp;
        //    public BigInteger Int;
            
        //}


        sealed class CodeError : CodeToken
        {
            public String Message;

            public CodeError(String value, String message) 
                : base(TokenType.Unknown, value)
            {
                this.Message = message;
            }
            public new String ToString()
            {
                return " Error[" + Message + "]" + base.ToString();
            }
        }


        class CodeLine: List<CodeToken>
        {
            private readonly int lineNumber_;
            public int LineNum { get { return lineNumber_; } }

            public CodeLine(int number)
            {
                this.lineNumber_ = number;
            }

            public new String ToString()
            {
                var b = new StringBuilder(this.Count * 20);
                b.AppendFormat("Line:{0}\r\n", lineNumber_);
                foreach (var tk in this)
                {
                    b.AppendFormat("\t{0}", tk.ToString());
                }
                return b.ToString();
            }
        }

    }

    
    /// <summary>
    ///  extended string utils for lexical analyzer
    /// </summary>
    public static partial class Extensions
    {

        public static bool IsValidLeadingIdCh(this char ch)
        {
            return ('a' <= ch && ch <= 'z')
                    || ('A' <= ch && ch <= 'Z')
                    || (ch == '_')
                    || (ch == '-')
                    || (ch == '?');
        }

        public static bool IsValidTailingIdCh(this char ch)
        {
            return IsValidLeadingIdCh(ch)
                    || ch.IsDigit()
                    || ch == '!';
        }

        public static bool IsSkipChar(this char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\r';
        }

        public static String UnEscapeStr(this String str)
        {
            var sb = new StringBuilder(str.Count() * 3 / 2);
            var escaping = false;
            foreach (var c in str)
            {
                if (escaping)
                {
                    escaping = false;
                    switch (c)
                    {
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
                else
                {
                    if (c == '\\')
                        escaping = true;
                    else
                        sb.Append(c);
                }
            }
            return sb.ToString();
        } // unescape
    }
}
