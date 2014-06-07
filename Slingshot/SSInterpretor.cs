using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;
using Slingshot.Objects;
using Slingshot.BuildIn;

namespace Slingshot
{
    public class SSInterpretor
    {
        public static void Main(string[] args)
        {
            int a = 1;
            int b = 2;
            int c = b << a;

            GLOBAL_SCOPE
               .LoadLib("stdfunc.ss")
               .InterpretingInConsole((code, scope) => code.ParseAsIScheme().Evaluate(scope));
        }


        public static readonly SSScope GLOBAL_SCOPE = new SSScope(null)
               .BuildIn("+", Functions.Numbers.Add)
               .BuildIn("**", Functions.Numbers.Power)
               .BuildIn("-", Functions.Numbers.Sub)
               .BuildIn("*", Functions.Numbers.Mul)
               .BuildIn("%", Functions.Numbers.Mod)
               .BuildIn("abs", Functions.Numbers.Abs)
               .BuildIn("set!", Functions.Numbers.Set)
               .BuildIn("rand", Functions.Numbers.Rand)

               .BuildIn("eq?", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.Equals(s2)))
               .BuildIn("==", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.Equals(s2)))
               .BuildIn(">", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() > s2.FloatVal()))
               .BuildIn("<", (nsargs, scope) =>
                   (((ISSNumber)nsargs[0].Evaluate(scope)).FloatVal()
                        < ((ISSNumber)nsargs[1].Evaluate(scope)).FloatVal()))

               .BuildIn(">=", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() >= s2.FloatVal()))
               .BuildIn("<=", (nsargs, scope) => nsargs.ChainRelation(scope,
                                                (s1, s2) => s1.FloatVal() <= s2.FloatVal()))

               .BuildIn("&", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a & b))
               .BuildIn("|", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a | b))
               .BuildIn("^", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a ^ b))
               .BuildIn("~", (nsargs, scope) => nsargs.BitOp(scope, a => ~a))
               .BuildIn("<<", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => ((int)a << (int)b)))
               .BuildIn(">>", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => ((int)a >> (int)b)))

               .BuildIn("and", (nsargs, scope) => nsargs.ChainRelation(scope,
                                                        (s1, s2) => s1.IntVal() == 1 && s2.IntVal() == 1))
               .BuildIn("or", (nsargs, scope) => nsargs.ChainRelation(scope,
                                                        (s1, s2) => s1.IntVal() == 1 || s2.IntVal() == 1))
               .BuildIn("not", (nsargs, scope) => nsargs.Op(scope, (a) => !a))
               .BuildIn("xor", (nsargs, scope) => nsargs.Ops(scope,
                                                        (a, b) => (!a && b) || (!b && a)))

               .BuildIn("xnor", (nsargs, scope) => nsargs.Ops(scope,
                                                        (a, b) => (!a && !b) || (b && a)))

               .BuildIn("car", (nsargs, scope) => nsargs.RetrieveSList(scope, "car").First())
               .BuildIn("cdr", (nsargs, scope) => new SSList(nsargs.RetrieveSList(scope, "cdr").Skip(1)))
               .BuildIn("cons", Functions.Lists.Cons)
               .BuildIn("list?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSList)
               .BuildIn("char?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSChar)
               .BuildIn("string?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSString)
               .BuildIn("integer?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSInteger)
               .BuildIn("float?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSFloat)
               .BuildIn("dict?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSDict)

               .BuildIn("clone", (nsargs, scope) =>
                   (scope.Define(nsargs[0].Token.Value, (SSObject)nsargs[1].Evaluate(scope).Clone())))
               .BuildIn("log", Functions.IO.Log)
               .BuildIn("error", (nsargs, scope) => { var ret = nsargs.Evaluate(scope); ret.ForEach(a => scope.Output.WriteLine(a)); throw new SystemException(ret.Last().ToString()); })
               .BuildIn("typeof", (nsargs, scope) => (SSString)nsargs[0].Evaluate(scope).GetType().FullName)
            /// <summary>
            /// append atom + list || list + atom || list + list
            /// </summary>
               .BuildIn("append", Functions.Lists.Append)
               .BuildIn("pop-back!", Functions.Lists.Popback)
               .BuildIn("pop-front!", Functions.Lists.PopFront)
               .BuildIn("set-at!", Functions.Lists.SetAt)
               .BuildIn("get-at", Functions.Lists.GetAt)
               .BuildIn("to-str", Functions.Lists.LsToStr)
               .BuildIn("to-list", Functions.Lists.StrToLs)
            /// <summary>
            /// (length a-list) || (length a-str)
            /// </summary>
               .BuildIn("length", Functions.Lists.Length)
               .BuildIn("null?", Functions.Lists.IsNull);
               
    }
}
