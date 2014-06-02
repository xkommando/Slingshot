using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    public class NSExpression
    {
        public String Value { get; private set; }
        public List<NSExpression> Children { get; private set; }
        public NSExpression Parent { get; private set; }

        public NSExpression(String value, NSExpression parent)
        {
            this.Value = value;
            this.Children = new List<NSExpression>();
            this.Parent = parent;
        }

        public override String ToString()
        {
            if (this.Value == "(")
            {
                return "(" + " ".Join(Children) + ")";
            }
            else
            {
                return this.Value;
            }
        }

        public NSObject Evaluate(NSScope scope)
        {
            NSExpression current = this;
            while (true)
            {
                if (current.Children.Count == 0)
                {
                    Int64 number;
                    if (Int64.TryParse(current.Value, out number))
                    {
                        return number;
                    }
                    else
                    {
                        return scope.Find(current.Value);
                    }
                }
                else
                {
                    NSExpression first = current.Children[0];
                    if (first.Value == "if")
                    {
                        NSBool condition = (NSBool)(current.Children[1].Evaluate(scope));
                        current = condition ? current.Children[2] : current.Children[3];
                    }
                    else if (first.Value == "def")
                    {
                        return scope.Define(current.Children[1].Value, current.Children[2].Evaluate(new NSScope(scope)));
                    }
                    else if (first.Value == "undef")
                    {
                        return scope.Undefine(current.Children[1].Value);
                    }
                    //else if (first.Value == "load")
                    //{
                    //    return scope.Undefine(current.Children[1].Value);
                    //}
                    //else if (first.Value == "error")
                    //{
                    //    return scope.Undefine(current.Children[1].Value);
                    //}
                    else if (first.Value == "begin")
                    {
                        NSObject result = null;
                        foreach (NSExpression statement in current.Children.Skip(1))
                        {
                            result = statement.Evaluate(scope);
                        }
                        return result;
                    }
                    else if (first.Value == "func")
                    {
                        NSExpression body = current.Children[2];
                        String[] parameters = current.Children[1].Children.Select(exp => exp.Value).ToArray();
                        NSScope newScope = new NSScope(scope);
                        return new NSFunction(body, parameters, newScope);
                    }
                    else if (first.Value == "list")
                    {
                        return new NSList(current.Children.Skip(1).Select(exp => exp.Evaluate(scope)));
                    }
                    else if (first.Value == "#t")
                    {
                        return NSBool.True;
                    }
                    else if (first.Value == "#f")
                    {
                        return NSBool.False;
                    }
                    else if (NSScope.BuiltinFunctions.ContainsKey(first.Value))
                    {
                        var arguments = current.Children.Skip(1).ToArray();
                        return NSScope.BuiltinFunctions[first.Value](arguments, scope);
                    }
                    else
                    {
                        NSFunction function = first.Value == "(" ? (NSFunction)first.Evaluate(scope) : (NSFunction)scope.Find(first.Value);
                        var arguments = current.Children.Skip(1).Select(s => s.Evaluate(scope)).ToArray();
                        NSFunction newFunction = function.Update(arguments);
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
        }

        // Displays the lexes in a readable form.
        public static String PrettyPrint(String[] lexes) {
            return "[" + (", ".Join(lexes.Select(s => "'" + s + "'"))) + "]";
        }

    }
}
