using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Slingshot.Objects;
using Slingshot.BuildIn;

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
                this.Output = parent == null ? null : parent.Output;
                this.Output = Output ?? Console.Out;
                this.Rand = new Random();
            }

            public SSObject Find(String name)
            {
                //Func<SSExpression[], SSScope, SSObject> func;
                //BUILTIN_FUNCTIONS.TryGetValue(name, out func);
                //if (func != null)
                //    return func;
                SSScope current = this;
                while (current != null)
                {
                    if (current.VariableTable.ContainsKey(name))
                    {
                        return current.VariableTable[name];
                    }
                    current = current.Parent;
                }
                throw new Exception(name + " is undefined.");
            }

            public SSObject Define(String name, SSObject value)
            {
                this.VariableTable.Add(name, value);
                return value;
            }

            public SSObject Undefine(String name)
            {
                return VariableTable.Remove(name);
            }

            private static readonly Dictionary<String, Func<SSExpression[], SSScope, SSObject>> 
                BUILTIN_FUNCTIONS =
                new Dictionary<String, Func<SSExpression[], SSScope, SSObject>>(64);

            public static Dictionary<String, Func<SSExpression[], SSScope, SSObject>> BuiltinFunctions
            {
                get { return BUILTIN_FUNCTIONS; }
            }

            // Dirty HACK for fluent API.
            public SSScope BuildIn(String name, Func<SSExpression[], SSScope, SSObject> builtinFuntion)
            {
                //Console.WriteLine("add [{0}]".Fmt(name));
                SSScope.BUILTIN_FUNCTIONS.Add(name, builtinFuntion);
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
                return VariableTable.ContainsKey(name) ? VariableTable[name] : null;
            }

        }
    }
}
