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
                if (this.Token.Type == TokenType.LeftParentheses)
                {
                    return "(" + " ".Join(Children) + ")";
                }
                else
                {
                    return this.Token.Value;
                }
            }

            public SSObject Evaluate(SSScope scope)
            {
                SSExpression current = this;
                while(true)
                {
                    CodeToken tok = current.Token;
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
                        /// @see Code Token KEY_WORDS
                        switch(tok.Type)
                        {
                            case TokenType.Def:
                                return scope.Define(current.Children[1].Token.Value, 
                                                    current.Children[2].Evaluate(new SSScope(scope)));

                            case TokenType.UnDef:
                                return scope.Undefine(current.Children[1].Token.Value);

                            case TokenType.Func:
                                SSExpression body = current.Children[2];
                                var parameters = current.Children[1].Children.Select(exp => exp.Token).ToArray();
                                SSScope newScope = new SSScope(scope);
                                return new SSFunction(body, parameters, newScope);

                            case TokenType.If:
                                SSBool condition = (SSBool)(current.Children[1].Evaluate(scope));
                                current = condition ? current.Children[2] : current.Children[3];
                                continue;

                            case TokenType.Begin:
                                SSObject ret = null;
                                foreach (SSExpression statement in current.Children.Skip(1))
                                {
                                    ret = statement.Evaluate(scope);
                                }
                                return ret;

                            case TokenType.List:
                        			return new SSList(current.Children.Skip(1).Select(exp => exp.Evaluate(scope)));

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
                            else
                            {
                                current = newFunction.Body;
                                scope = newFunction.Scope;
                            }
                        }
                        catch(Exception e)
                        {
                            scope.Output.WriteLine( " ===== " + current.Children[0].Token.Value);
                            scope.Output.WriteLine(e);
                            //scope.Output.WriteLine(e.StackTrace);
                        }


                    }
                }
            }


            public static String PrettyPrint(String[] lexes)
            {
                return "[" + (", ".Join(lexes.Select(s => "'" + s + "'"))) + "]";
            }

        }
    }
}
