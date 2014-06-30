using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Compiler
    {
        public class SyntaxAnalyzer
        {
            public List<CodeToken> TokenList;
            public List<CodeError> ErrorList;
            public List<SSExpression> Expressions {get; private set; }

            public SyntaxAnalyzer()
            {
                TokenList = new List<CodeToken>();
                //ErrorList = new List<CodeError>();
                Expressions = new List<SSExpression>();
            }

            public void Take(List<CodeToken> toks)
            {
                this.TokenList = toks;
                this.Parse();
            }

            public void Parse()
            {
                var program = new SSExpression(null, null);
                var current = program;
                foreach (var tok in TokenList)
                {
                    //if (lex.Type == TokenType.Char)
                    //Console.WriteLine(lex.Type + "      [" + lex.Value+"]");
                    switch (tok.Type)
                    {
                        case TokenType.LeftBracket:
                        case TokenType.LeftCurlyBracket:
                        case TokenType.LeftParentheses:
                            var newNode = new SSExpression(tok: tok, parent: current);
                            current.Children.Add(newNode);
                            current = newNode;
                            break;

                        case TokenType.RightBracket:
                        case TokenType.RightParenthese:
                        case TokenType.RightCurlyBracket:
                            current = current.Parent;
                            break;

                        default:
                            //Console.WriteLine("adding {0} {1}".Fmt(tok.Type, tok.Value));
                            current.Children.Add(new SSExpression(tok, current));
                            break;
                    }

                }
                Expressions = program.Children;
            }


            public void ReSet()
            {
                this.TokenList.Clear();
                this.Expressions.Clear();
                if (ErrorList.NotEmpty()) 
                    ErrorList.Clear();
            }

            public void PrintErrors(TextWriter writer)
            {
                writer.WriteLine("{0} Errors:".Fmt(ErrorList.Count));
                this.ErrorList.ForEach(
                    ce => writer.WriteLine(" {0} ", ce.ToString()));
            }
        }
    }
}
