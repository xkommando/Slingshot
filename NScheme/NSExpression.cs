using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    namespace compiler
    {
        public class NSExpression
        {
            public CodeToken Token { get; private set; }
            public List<NSExpression> Children { get; private set; }
            public NSExpression Parent { get; private set; }

            public NSExpression(CodeToken tok, NSExpression parent)
            {
                this.Token = tok;
                this.Parent = parent;
                this.Children = new List<NSExpression>(6);
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

            public NSObject Evaluate(NSScope scope)
            {
                NSExpression current = this;
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
                                return new NSChar(tok.Value[0]);

                            case TokenType.True:
                                return NSBool.NSTrue;
                            case TokenType.False:
                                return NSBool.NSFalse;

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
                                                    current.Children[2].Evaluate(new NSScope(scope)));

                            case TokenType.UnDef:
                                return scope.Undefine(current.Children[1].Token.Value);

                            case TokenType.Func:
                                NSExpression body = current.Children[2];
                                var parameters = current.Children[1].Children.Select(exp => exp.Token).ToArray();
                                NSScope newScope = new NSScope(scope);
                                return new NSFunction(body, parameters, newScope);

                            case TokenType.If:
                                NSBool condition = (NSBool)(current.Children[1].Evaluate(scope));
                                current = condition ? current.Children[2] : current.Children[3];
                                continue;

                            case TokenType.Begin:
                                NSObject ret = null;
                                foreach (NSExpression statement in current.Children.Skip(1))
                                {
                                    ret = statement.Evaluate(scope);
                                }
                                return ret;

                            case TokenType.List:
                        			return new NSList(current.Children.Skip(1).Select(exp => exp.Evaluate(scope)));

                            case TokenType.True:
                                    return NSBool.NSTrue;
                            case TokenType.False:
                                    return NSBool.NSFalse;
                        }

                        Func<NSExpression[], NSScope, NSObject> func;
                        NSScope.BuiltinFunctions.TryGetValue(tok.Value, out func);
                        if (func != null)
                        {
                            var arguments = current.Children.Skip(1).ToArray();
                            return func(arguments, scope);
                        }

                        NSFunction function = tok.Type == TokenType.LeftParentheses ?
                                             (NSFunction)first.Evaluate(scope)
                                             : (NSFunction)scope.Find(tok.Value);

                        var args = current.Children.Skip(1).Select(s => s.Evaluate(scope)).ToArray();
                        NSFunction newFunction = function.Update(args);
                        if (newFunction.IsPartial)
                        {
                            return newFunction.Evaluate();
                        }
                        else
                        {
                            current = newFunction.Body;
                            scope = newFunction.Scope;
                        }

                    }
                }
            }

            // Displays the lexes in a readable form.
            public static String PrettyPrint(String[] lexes)
            {
                return "[" + (", ".Join(lexes.Select(s => "'" + s + "'"))) + "]";
            }

        }
    }
}
