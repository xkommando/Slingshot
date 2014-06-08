using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;
using Slingshot.Objects;
using Slingshot.BuildIn;

namespace Slingshot
{
    namespace Interpretor
    {

        class SSInterpretor
        {
            public static void Main(string[] args)
            {
                GLOBAL_SCOPE
                   .LoadLib("stdfunc.ss")
                   .InterpretingInConsole();
            }


            private static readonly SSScope GLOBAL_SCOPE = new SSScope(null)
                   .BuildIn("+", Functions.Numbers.Add)
                   .BuildIn("**", Functions.Numbers.Power)
                   .BuildIn("-", Functions.Numbers.Sub)
                   .BuildIn("*", Functions.Numbers.Mul)
                   .BuildIn("%", Functions.Numbers.Mod)
                   .BuildIn("abs", Functions.Numbers.Abs)
                   .BuildIn("set!", Functions.Numbers.Set)
                   .BuildIn("rand", Functions.Numbers.Rand)

                   .BuildIn("eq?", (nsargs, scope) => nsargs.Ops<SSObject>(scope, (a, b)=>a.Eq(b)))
                   .BuildIn("==", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.Eq(s2)))
                   .BuildIn(">", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() > s2.FloatVal()))
                   .BuildIn("<", (nsargs, scope) =>
                       (((SSNumber)nsargs[0].Evaluate(scope)).FloatVal()
                            < ((SSNumber)nsargs[1].Evaluate(scope)).FloatVal()))

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
                   .BuildIn("not", (nsargs, scope) => nsargs.Op<SSBool>(scope, (a) => !a))
                   .BuildIn("xor", (nsargs, scope) => nsargs.Ops<SSBool>(scope,
                                                            (a, b) => (!a && b) || (!b && a)))

                   .BuildIn("xnor", (nsargs, scope) => nsargs.Ops<SSBool>(scope,
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
                   .BuildIn("error", (nsargs, scope) => { var ret = nsargs.Evaluate(scope); 
                                            ret.ForEach(a =>scope.Output.WriteLine(a)); 
                                            throw new SystemException(ret.Last().ToString()); })
                   
                   .BuildIn("typeof", (nsargs, scope) => (SSString)nsargs[0].Evaluate(scope).GetType().FullName)
                    // append atom + list || list + atom || list + list
                   .BuildIn("append", Functions.Lists.Append)
                   .BuildIn("pop-back!", Functions.Lists.Popback)
                   .BuildIn("pop-front!", Functions.Lists.PopFront)
                   .BuildIn("set-at!", Functions.Lists.SetAt)
                   .BuildIn("elem-at", Functions.Lists.ElemAt)

                   .BuildIn("to-str", Functions.Lists.LsToStr)
                   .BuildIn("to-list", Functions.Lists.StrToLs)
                    // (length a-list) || (length a-str)
                   .BuildIn("length", Functions.Lists.Length)
                   .BuildIn("null?", Functions.Lists.IsNull);

        }

    }
}
