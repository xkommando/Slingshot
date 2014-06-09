using System;
using System.Collections.Generic;
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
                ErrorList = new List<CodeError>();
                Expressions = new List<SSExpression>();
            }
            public void Take(CodeFile src)
            {
                src.Parse();
                if (src.ErrorList.Count > 0)
                    return;

                this.TokenList = src.TokenList;
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
                            current.Children.Add(new SSExpression(tok, current));
                            break;
                    }

                }
                Expressions = program.Children;
            }


            public void ReSet()
            {
                this.ErrorList.Clear();
                this.TokenList.Clear();
                this.Expressions.Clear();
            }
        }
    }
}
