using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    class NScheme
    {
        static void Main(string[] args)
        {
            int[] myList = { 1, 2, 3, 5, 9 };
            Console.WriteLine(myList.Sum(a=> a * a));

            new NSScope(parent: null)

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

               .BuildIn("car", (nsargs, scope) => nsargs.RetrieveSList(scope, "car").First())
               .BuildIn("cdr", (nsargs, scope) => new NSList(nsargs.RetrieveSList(scope, "cdr").Skip(1)))
               .BuildIn("cons", Functions.Lists.Cons)
               .BuildIn("list?", (nsargs, scope) => nsargs[0].Evaluate(scope) is NSList)
               .BuildIn("append", Functions.Lists.Append)
               .BuildIn("pop-back", Functions.Lists.Popback)
               .BuildIn("pop-front", Functions.Lists.PopFront)
               .BuildIn("set-at!", Functions.Lists.SetAt)
               .BuildIn("get-at", Functions.Lists.GetAt)

               .BuildIn("length", (nsargs, scope) => (nsargs.Evaluate<NSList>(scope).First().Length))
               .BuildIn("null?", (nsargs, scope) => nsargs.RetrieveSList(scope, "null?").Count() == 0)
               .LoadLib("stdfunc.ns")
               .KeepInterpretingInConsole((code, scope) => code.ParseAsIScheme().Evaluate(scope));
        }
    }
}
