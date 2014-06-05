using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NScheme.compiler;

namespace NScheme
{

    class Interpretor
    {
        static void Main(string[] args)
        {
            GLOBAL_SCOPE
               .LoadLib("stdfunc.ns")
               .InterpretingInConsole((code, scope) => code.ParseAsIScheme().Evaluate(scope));
        }


        public static readonly NSScope GLOBAL_SCOPE = new NSScope(null)
               .BuildIn("+", Functions.Numbers.Add)
               .BuildIn("**", Functions.Numbers.Power)
               .BuildIn("-", Functions.Numbers.Sub)
               .BuildIn("*", Functions.Numbers.Mul)
               .BuildIn("%", Functions.Numbers.Mod)
               .BuildIn("abs", Functions.Numbers.Abs)
               .BuildIn("set!", Functions.Numbers.Set)

               .BuildIn("==", Functions.Numbers.Eq)
            //.BuildIn(">", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1 > s2))
               .BuildIn(">", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1 > s2))
               .BuildIn("<", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1 < s2))
               .BuildIn(">=", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1 >= s2))
               .BuildIn("<=", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1 <= s2))

               .BuildIn("&&", Functions.Booleans.And)
               .BuildIn("||", Functions.Booleans.Or)
               .BuildIn("!", Functions.Booleans.Not)
               .BuildIn("^", Functions.Booleans.Xor)
               .BuildIn("~", Functions.Booleans.Xnor)
               .BuildIn("and", Functions.Booleans.And)
               .BuildIn("or", Functions.Booleans.Or)
               .BuildIn("not", Functions.Booleans.Not)
               .BuildIn("xor", Functions.Booleans.Xor)
               .BuildIn("xnor", Functions.Booleans.Xnor)

               .BuildIn("car", (nsargs, scope) => nsargs.RetrieveSList(scope, "car").First())
               .BuildIn("cdr", (nsargs, scope) => new NSList(nsargs.RetrieveSList(scope, "cdr").Skip(1)))
               .BuildIn("cons", Functions.Lists.Cons)
               .BuildIn("list?", (nsargs, scope) => nsargs[0].Evaluate(scope) is NSList)

               .BuildIn("clone", (nsargs, scope) => 
                   (scope.Define(nsargs[0].Token.Value, nsargs[1].Evaluate(scope).Clone())))
                .BuildIn("typeof", (nsargs, scope) => (NSString)nsargs[0].Evaluate(scope).GetType().FullName)
            /// <summary>
            /// append atom + list || list + atom || list + list
            /// </summary>
               .BuildIn("append", Functions.Lists.Append)
               .BuildIn("pop-back!", Functions.Lists.Popback)
               .BuildIn("pop-front!", Functions.Lists.PopFront)
               .BuildIn("set-at!", Functions.Lists.SetAt)
               .BuildIn("get-at", Functions.Lists.GetAt)
            /// <summary>
            /// (length a-list) || (length a-str)
            /// </summary>
               .BuildIn("length", Functions.Lists.Length)
               .BuildIn("null?", (nsargs, scope) => nsargs.RetrieveSList(scope, "null?").Count() == 0);
               
    }
}
