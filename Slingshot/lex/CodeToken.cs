using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slingshot
{
    namespace Compiler
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

            public static readonly Dictionary<string, CodeToken> KEY_WORDS = new Dictionary<string, CodeToken> 
            {
                {"def" , new CodeToken(TokenType.Def, "def")},
                {"undef" , new CodeToken(TokenType.UnDef, "undef")},

                {"func" , new CodeToken(TokenType.Func, "func")},

                {"if" , new CodeToken(TokenType.If, "if")},
                {"?" , new CodeToken(TokenType.QMark, "?")},
                
              //  {"begin" , new CodeToken(TokenType.Begin, "begin")},
                {":" , new CodeToken(TokenType.Colon, ":")},
                
                {"list" , new CodeToken(TokenType.List, "list")},
                {"True" , new CodeToken(TokenType.True, "True")},
                {"False" , new CodeToken(TokenType.False, "False")},
                {"continue" , new CodeToken(TokenType.Continue, "continue")},
                {"break" , new CodeToken(TokenType.Break, "break")}
            };

            public static CodeToken Build(TokenType type, String value)
            {
                if (type == TokenType.Identifier)
                {
                    CodeToken tk;
                    KEY_WORDS.TryGetValue(value, out tk);
                    return tk == null ? new CodeToken(type, value) : tk;
                }
                else if (type == TokenType.String)
                    return new CodeToken(TokenType.String, value.UnEscapeStr());
                else
                    return new CodeToken(type, value);
            }

        }

        //public class ValueToken : CodeToken
        //{
        //    public double Fp;
        //    public BigInteger Int;
            
        //}


        public class CodeError : CodeToken
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


        public class CodeLine: List<CodeToken>
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
