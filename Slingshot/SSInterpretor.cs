using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Slingshot.Compiler;
using Slingshot.Objects;
using Slingshot.BuiltIn;

namespace Slingshot
{
    namespace Interpretor
    {
        class SSInterpretor
        {
            public static void Main(string[] args)
            {

               // Console.WriteLine(SSScope.BuiltinFunctions.Count);

                Console.WriteLine("\r\n\t\tSlingshot version 0.3\t\t\r\n\r\n");

                GLOBAL_SCOPE
                   .LoadLib("basic.ss")
                   .LoadLib("algo.ss")
                   .LoadLib("math.ss")
                   .LoadLib("statistic.ss")
                 //  .LoadLib("chinese.ss")
                   .InterpretingInConsole();

            }

            private static readonly SSScope GLOBAL_SCOPE = new SSScope(null)
                // control flow
                //                public static SSObject Def(SSExpression[] exps, SSScope scope)
                //{
                //    return scope.Define(exps[0].Token.Value, exps[1].Evaluate(scope));
                //}
                .BuildIn("def", (a, s)=>s.Define(a[0].Token.Value, a[1].Evaluate(s)))
                .BuildIn("undef", (a, s)=>s.Undefine(a[0].Token.Value))
                .BuildIn("clear-scope", (a, s) => { s.VariableTable.Clear();
                                                      return true;
                })
                .BuildIn("defined?", Functions.Flow.Defined)
                .BuildIn("require", Functions.Flow.Require)
                .BuildIn("set!", Functions.Flow.Set)
                .BuildIn("func", Functions.Flow.Func)
                .BuildIn("if", Functions.Flow.If)
                .BuildIn("while", Functions.Flow.While)
                .BuildIn("loop", Functions.Flow.Loop)
                .BuildIn("switch", Functions.Flow.Switch)
                .BuildIn("try", Functions.Flow.TryCatch)
                .BuildIn("error", Functions.Flow.Error)

                // IO
                .BuildIn("file", Functions.IO.OpenFile)
                .BuildIn("read-str", Functions.IO.ReadStr)
                .BuildIn("load", Functions.IO.Load)

                // misc functions
                .BuildIn("debug", Functions.Misc.Debug)
                .BuildIn("rand", Functions.Misc.Rand)
                .BuildIn("hash", Functions.Misc.Hash)
                .BuildIn("clone", (nsargs, scope) =>
                    (scope.Define(nsargs[0].Token.Value, (SSObject) nsargs[1].Evaluate(scope).Clone())))
                .BuildIn("typeof", (nsargs, scope) => (SSString) nsargs[0].Evaluate(scope).GetType().FullName)
                .BuildIn("alias", Functions.Misc.Alias)
                .BuildIn("rename", Functions.Misc.Rename)
                .BuildIn("remove", Functions.Misc.Remove)
                .BuildIn("swap", Functions.Misc.Swap)

                // numeric
                .BuildIn("+", Functions.Numbers.Add)
                .BuildIn("-", Functions.Numbers.Sub)
                .BuildIn("*", Functions.Numbers.Mul)
                .BuildIn("**",
                    (nsargs, scope) => nsargs.Ops<SSNumber>(scope, (a, b) => Math.Pow(a.FloatVal(), b.FloatVal())))
                .BuildIn("/", (nsargs, scope) => nsargs.Ops<SSNumber>(scope, (a, b) => a.FloatVal()/b.FloatVal()))
                .BuildIn("%", (nsargs, scope) => nsargs.Ops<SSInteger>(scope, (a, b) => a%b))
                .BuildIn("abs", Functions.Numbers.Abs)
                .BuildIn("log",
                    (nsargs, scope) => nsargs.Ops<SSNumber>(scope, (a, b) => Math.Log(b.FloatVal(), a.FloatVal())))
                .BuildIn("exp", (nsargs, scope) => nsargs.Op<SSInteger>(scope, a => Math.Exp(a.FloatVal())))
                .BuildIn("sin", (nsargs, scope) => nsargs.Op<SSInteger>(scope, a => Math.Sin(a.FloatVal())))
                .BuildIn("cos", (nsargs, scope) => nsargs.Op<SSInteger>(scope, a => Math.Cos(a.FloatVal())))
                .BuildIn("tan", (nsargs, scope) => nsargs.Op<SSInteger>(scope, a => Math.Tan(a.FloatVal())))
                .BuildIn("--", (nsargs, scope) => nsargs.Op<SSInteger>(scope, a =>
                {
                    a.Val--;
                    return a;
                }))
                .BuildIn("++", (nsargs, scope) => nsargs.Op<SSInteger>(scope, a =>
                {
                    a.Val++;
                    return a;
                }))
                .BuildIn("eq?", (nsargs, scope) => nsargs.Ops<SSObject>(scope, (a, b) => a.Eq(b)))
                .BuildIn("==", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.Eq(s2)))
                .BuildIn("nq?", (nsargs, scope) => nsargs.Ops<SSObject>(scope, (a, b) => !a.Eq(b)))
                .BuildIn("!=", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => !s1.Eq(s2)))

                .BuildIn(">", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() > s2.FloatVal()))
                .BuildIn("<", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() < s2.FloatVal()))

                .BuildIn(">=",
                    (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() >= s2.FloatVal()))
                .BuildIn("<=", (nsargs, scope) => nsargs.ChainRelation(scope,
                    (s1, s2) => s1.FloatVal() <= s2.FloatVal()))

                .BuildIn("&", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a & b))
                .BuildIn("|", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a | b))
                .BuildIn("^", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a ^ b))
                .BuildIn("~", (nsargs, scope) => nsargs.BitOp(scope, a => ~a))
                .BuildIn("<<", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => ((int) a << (int) b)))
                .BuildIn(">>", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => ((int) a >> (int) b)))

                .BuildIn("and", (nsargs, scope) => nsargs.ChainRelation(scope,
                    (s1, s2) => s1.IntVal() == 1 && s2.IntVal() == 1))
                .BuildIn("or", (nsargs, scope) => nsargs.ChainRelation(scope,
                    (s1, s2) => s1.IntVal() == 1 || s2.IntVal() == 1))
                .BuildIn("not", (nsargs, scope) => nsargs.Op<SSBool>(scope, (a) => !a))
                .BuildIn("xor", (nsargs, scope) => nsargs.Ops<SSBool>(scope,
                    (a, b) => (!a && b) || (!b && a)))

                .BuildIn("xnor", (nsargs, scope) => nsargs.Ops<SSBool>(scope,
                    (a, b) => (!a && !b) || (b && a)))
                // list processing
                .BuildIn("car", (nsargs, scope) => nsargs.RetrieveSList(scope, "car").First())
                .BuildIn("cdr", (nsargs, scope) => new SSList(nsargs.RetrieveSList(scope, "cdr").Skip(1)))
                .BuildIn("cons", Functions.Seqs.Cons)
                .BuildIn("list?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSList)
                .BuildIn("char?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSChar)
                .BuildIn("string?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSString)
                .BuildIn("integer?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSInteger)
                .BuildIn("float?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSFloat)
                .BuildIn("dict?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSDict)
                .BuildIn("null?", Functions.Seqs.IsNull)

                // append atom + list || list + atom || list + list
                .BuildIn("append", Functions.Seqs.Append)
                .BuildIn("pop-back!", Functions.Seqs.Popback)
                .BuildIn("pop-front!", Functions.Seqs.PopFront)
                .BuildIn("set-at!", Functions.Seqs.SetAt)
                .BuildIn("elem-at", Functions.Seqs.ElemAt)

                .BuildIn("to-str", Functions.Seqs.ToStr)
                .BuildIn("to-int", Functions.Seqs.ToInt)
                .BuildIn("to-float", Functions.Seqs.ToFloat)
                .BuildIn("to-list", Functions.Seqs.StrToLs)
                // (length a-list) || (length a-str)
                .BuildIn("length", Functions.Seqs.Length)
                ;

        }

    }
}
