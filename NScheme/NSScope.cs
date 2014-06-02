using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    public class NSScope
    {
        public NSScope Parent { get; private set; }
        private Dictionary<String, NSObject> variableTable;

        public NSScope(NSScope parent)
        {
            this.Parent = parent;
            this.variableTable = new Dictionary<String, NSObject>(32);
        }

        public NSObject Find(String name)
        {
            NSScope current = this;
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

        public NSObject Define(String name, NSObject value)
        {
            this.variableTable.Add(name, value);
            return value;
        }

        public NSObject Undefine(String name)
        {
            return variableTable.Remove(name);
        }

        public static Dictionary<String, Func<NSExpression[], NSScope, NSObject>> builtinFunctions =
            new Dictionary<String, Func<NSExpression[], NSScope, NSObject>>(64);

        public static Dictionary<String, Func<NSExpression[], NSScope, NSObject>> BuiltinFunctions
        {
            get { return builtinFunctions; }
        }

        // Dirty HACK for fluent API.
        public NSScope BuildIn(String name, Func<NSExpression[], NSScope, NSObject> builtinFuntion)
        {
            //Console.WriteLine("add [{0}]".Fmt(name));
            NSScope.builtinFunctions.Add(name, builtinFuntion);
            return this;
        }

        public NSScope SpawnScopeWith(String[] names, NSObject[] values)
        {
            (names.Length >= values.Length).OrThrows("Too many arguments.");
            var scope = new NSScope(this);
            for (var i = 0; i < values.Length; i++)
            {
                scope.variableTable.Add(names[i], values[i]);
            }
            return scope;
        }

        public NSObject FindInTop(String name)
        {
            return variableTable.ContainsKey(name) ? variableTable[name] : null;
        }

    }
}
