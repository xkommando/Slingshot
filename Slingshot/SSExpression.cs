using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Interpretor;
using Slingshot.Objects;
using Slingshot.BuiltIn;

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
                while (true)
                {
                    var tok = current.Token;
                    try
                    {
                        switch (current.Token.Type)
                        {
                            case TokenType.LeftCurlyBracket:// {
                                SSObject ret = null;
                                foreach (var exp in current.Children)
                                {
                                    if (exp.Token.Type == TokenType.Continue)
                                        break;
                                    else if (exp.Token.Type == TokenType.Break)
                                        return SSSignal.Break;

                                    ret = exp.Evaluate(scope);
                                }
                                return ret;

                            case TokenType.LeftBracket: // [
                                return new SSList(current.Children.Select(a => a.Evaluate(scope)));

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
                                return SSBool.True;
                            case TokenType.False:
                                return SSBool.False;
                            case TokenType.Slingshot:
                                Console.WriteLine(current.Token.Value);
                                Console.WriteLine(current.Parent.Children.Count);
                                current.Parent.Children.ForEach(a=>Console.WriteLine(a.Token.Value));
                                return true;//scope.Cmd(current.Children);
                            case TokenType.Identifier:
                                return scope.Find(tok.Value);
                            default:

                                var first = current.Children[0];
                                tok = first.Token;

                                Func<SSExpression[], SSScope, SSObject> func;
                                SSScope.BuiltinFunctions.TryGetValue(tok.Value, out func);
                                if (func != null)
                                {
                                    var arguments = current.Children.Skip(1).ToArray();
                                    return func(arguments, scope);
                                }
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
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        var c = current;
                        var cc = ConsoleColor.Green;
                        var console = false;
                        if (scope.Output == Console.Out)
                        {
                            console = true;
                            cc =  Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        c.Print(scope.Output);
                        scope.Output.WriteLine();
                        if (console)
                            Console.ForegroundColor = ConsoleColor.Red;
                        if (console)
                            Console.ForegroundColor = cc;

                        Console.WriteLine("=====================" + current.Token.Value);
                        throw;
                    }
                }
            }


            //public static String PrettyPrint(String[] lexes)
            //{
            //    return "[" + (", ".Join(lexes.Switch(s => "'" + s + "'"))) + "]";
            //}

        }
    }
}
