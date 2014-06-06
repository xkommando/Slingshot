using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Slingshot.Objects;
using Slingshot.Buildin;

namespace Slingshot
{
    namespace Compiler
    {
        public class SSScope
        {
            public TextWriter StdOut;
            public SSScope Parent { get; private set; }
            public Dictionary<string, SSObject> variableTable { get; private set; }

            public SSScope(SSScope parent)
            {
                this.Parent = parent;
                this.variableTable = new Dictionary<string, SSObject>(32);
                this.StdOut = parent == null ? null : parent.StdOut;
                this.StdOut = StdOut ?? Console.Out;
            }

            public SSObject Find(String name)
            {
                SSScope current = this;
                while (current != null)
                {
                    if (current.variableTable.ContainsKey(name))
                    {
                        return current.variableTable[name];
                    }
                    current = current.Parent;
                }
                throw new Exception(name + " is undefined.");
            }

            public SSObject Define(String name, SSObject value)
            {
                this.variableTable.Add(name, value);
                return value;
            }

            public SSObject Undefine(String name)
            {
                return variableTable.Remove(name);
            }

            public static Dictionary<String, Func<SSExpression[], SSScope, SSObject>> builtinFunctions =
                new Dictionary<String, Func<SSExpression[], SSScope, SSObject>>(64);

            public static Dictionary<String, Func<SSExpression[], SSScope, SSObject>> BuiltinFunctions
            {
                get { return builtinFunctions; }
            }

            // Dirty HACK for fluent API.
            public SSScope BuildIn(String name, Func<SSExpression[], SSScope, SSObject> builtinFuntion)
            {
                //Console.WriteLine("add [{0}]".Fmt(name));
                SSScope.builtinFunctions.Add(name, builtinFuntion);
                return this;
            }

            public SSScope SpawnScopeWith(CodeToken[] toks, SSObject[] values)
            {
                (toks.Length >= values.Length).OrThrows("Too many arguments.");
                var scope = new SSScope(this);
                for (var i = 0; i < values.Length; i++)
                {
                    scope.variableTable.Add(toks[i].Value, values[i]);
                }
                return scope;
            }

            public SSObject FindInTop(String name)
            {
                return variableTable.ContainsKey(name) ? variableTable[name] : null;
            }

        }
    }
}
