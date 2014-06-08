using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Objects;
using Slingshot.BuildIn;

namespace Slingshot
{
    namespace Compiler
    {
        public class SSExpression
        {
            public CodeToken Token { get; private set; }
            public List<SSExpression> Children { get; private set; }
            public SSExpression Parent { get; private set; }

            public SSExpression(CodeToken tok, SSExpression parent)
            {
                this.Token = tok;
                this.Parent = parent;
                this.Children = new List<SSExpression>(6);
            }

            public override String ToString()
            {
                if (Token.Type == TokenType.LeftCurlyBracket)
                    return "{" + " ".Join(Children) + "}";
                else if (Token.Type == TokenType.LeftParentheses)
                    return "(" + " ".Join(Children) + ")";
                else
                    return Token.Value;
            }

            public SSObject Evaluate(SSScope scope)
            {
                var current = this;
                while(true)
                {
                    var tok = current.Token;

                    //Console.WriteLine("=====>>>>" + tok.Value + "  " 
                    //+ current.Children.Count);//+ "    " + current.Children[0].Token.Value);
                    switch (current.Token.Type)
                    {
                        case TokenType.LeftCurlyBracket:
                            SSObject ret = null;
                            current.Children.ForEach(a => ret = a.Evaluate(scope));
                            return ret;
                        case TokenType.LeftBracket:
                            return new SSList(current.Children.Select(a => a.Evaluate(scope)));
                    }
                    if (current.Children.Count == 0)
                        switch (tok.Type)
                        {
                            case TokenType.Integer:
                                return Int64.Parse(tok.Value);

                            case TokenType.SciNumber:
                            case TokenType.Float:
                                return double.Parse(tok.Value);

                            case TokenType.String:
                                return tok.Value;
                            case TokenType.Char:
                                return new SSChar(tok.Value[0]);

                            case TokenType.True:
                                return SSBool.NSTrue;
                            case TokenType.False:
                                return SSBool.NSFalse;

                            case TokenType.Identifier:
                                return scope.Find(tok.Value);
                        }
                    else
                    {
                        var first = current.Children[0];
                        tok = first.Token;

                        // @see Code Token KEY_WORDS
                        switch(tok.Type)
                        {
                            case TokenType.Def:
                                return scope.Define(current.Children[1].Token.Value, 
                                                    current.Children[2].Evaluate(new SSScope(scope)));

                            case TokenType.UnDef:
                                return scope.Undefine(current.Children[1].Token.Value);

                            case TokenType.Func:
                                var parameters = current.Children[1].Children.Select(exp => exp.Token).ToArray();
                                var body = current.Children[2];
                                var newScope = new SSScope(scope);
                                return new SSFunction(body, parameters, newScope);

                            //case TokenType.QMark:
                            case TokenType.If:
                                var condition = (SSBool)(current.Children[1].Evaluate(scope));
                                current = condition ? current.Children[2] : current.Children[3];
                                continue;

                            case TokenType.True:
                                    return SSBool.NSTrue;
                            case TokenType.False:
                                    return SSBool.NSFalse;
                        }
      
                        Func<SSExpression[], SSScope, SSObject> func;
                        SSScope.BuiltinFunctions.TryGetValue(tok.Value, out func);
                        if (func != null)
                        {
                            var arguments = current.Children.Skip(1).ToArray();
                            return func(arguments, scope);
                        }
                        try
                        {
                            SSFunction function = tok.Type == TokenType.LeftParentheses ?
                                                (SSFunction)first.Evaluate(scope)
                                                : (SSFunction)scope.Find(tok.Value);

                            var args = current.Children.Skip(1).Select(s => s.Evaluate(scope)).ToArray();
                            SSFunction newFunction = function.Update(args);
                            if (newFunction.IsPartial())
                            {
                                return newFunction.Evaluate();
                            }
                            current = newFunction.Body;
                            scope = newFunction.Scope;
                        }
                        catch(Exception e)
                        {
                          //  scope.Output.WriteLine( " ===== " + current.Children[0].Token.Value);
                            current.Children.ForEach(a => Console.WriteLine(a.Token.Value + "   "));
                            scope.Output.WriteLine(e);
                            return SSBool.NSFalse;
                        }


                    }
                }
            }


            //public static String PrettyPrint(String[] lexes)
            //{
            //    return "[" + (", ".Join(lexes.Select(s => "'" + s + "'"))) + "]";
            //}

        }
    }
}
