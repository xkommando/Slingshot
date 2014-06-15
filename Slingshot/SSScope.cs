using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Slingshot.Objects;
using Slingshot.BuiltIn;

namespace Slingshot
{
    namespace Compiler
    {
        public class SSScope
        {
            public readonly TextWriter Output;
            public readonly Random Rand;


            public SSScope Parent { get; private set; }
            public Dictionary<string, SSObject> VariableTable { get; private set; }

            public SSScope(SSScope parent)
            {
                this.Parent = parent;
                this.VariableTable = new Dictionary<string, SSObject>(32);

                this.Output = parent == null ? Console.Out : parent.Output;
                this.Output = Output ?? Console.Out;
                this.Rand = parent == null ? null : parent.Rand;
                this.Rand = Rand ?? new Random();
            }

            public SSObject Find(String name)
            {
                //Func<SSExpression[], SSScope, SSObject> func;
                //BUILTIN_FUNCTIONS.TryGetValue(name, out func);
                //if (func != null)
                //    return func;
                SSObject obj = null;
                SSScope current = this;
                while (current != null)
                {
                    if (current.VariableTable.TryGetValue(name, out obj))
                    {
                        return obj;
                    }
                    current = current.Parent;
                }
                throw new Exception(name + " is undefined.");
            }

            public SSObject Define(String name, SSObject value)
            {
                try
                {
                    this.VariableTable.Add(name, value);
                    return value;
                }
                catch (Exception)
                {
                    Output.WriteLine("Redefinition of symbol {0} value {1}".Fmt(name, VariableTable[name]));
                    return SSBool.False;
                }
            }

            public SSObject Undefine(String name)
            {
                return VariableTable.Remove(name);
            }

            public SSObject Remove(String name)
            {
                SSScope current = this;
                while (current != null)
                {
                    if (current.VariableTable.Remove(name))
                    {
                        return true;
                    }
                    current = current.Parent;
                }
                throw new Exception(name + " is undefined.");
            }

            public static readonly Dictionary<String, Func<SSExpression[], SSScope, SSObject>>
                BuiltinFunctions =
                new Dictionary<String, Func<SSExpression[], SSScope, SSObject>>(512);

            //public static Dictionary<String, Func<SSExpression[], SSScope, SSObject>> BuiltinFunctions
            //{
            //    get { return BUILTIN_FUNCTIONS; }
            //}

            // Dirty HACK for fluent API.
            public SSScope BuildIn(String name, Func<SSExpression[], SSScope, SSObject> builtinFuntion)
            {
                //Console.WriteLine("add [{0}]".Fmt(name));
                SSScope.BuiltinFunctions.Add(name, builtinFuntion);
                return this;
            }

            public SSScope SpawnScopeWith(CodeToken[] toks, SSObject[] values)
            {
                (toks.Length >= values.Length).OrThrows("Too many arguments.");
                var scope = new SSScope(this);
                for (var i = 0; i < values.Length; i++)
                {
                    scope.VariableTable.Add(toks[i].Value, values[i]);
                }
                return scope;
            }

            public SSObject FindInTop(String name)
            {
                SSObject obj = null;
                VariableTable.TryGetValue(name, out obj);
                return obj;
                //return VariableTable.ContainsKey(name) ? VariableTable[name] : null;
            }

        }

        public static partial class Extensions
        {
            public static IEnumerable<T> Evaluate<T>(this IEnumerable<SSExpression> expressions, SSScope scope)
            where T : SSObject
            {
                return expressions.Evaluate(scope).Cast<T>();
            }

            public static IEnumerable<SSObject> Evaluate(this IEnumerable<SSExpression> expressions, SSScope scope)
            {
                return expressions.Select(exp => exp.Evaluate(scope));
            }
        }
    }
}
