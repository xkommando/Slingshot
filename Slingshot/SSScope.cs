using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Slingshot.Interpretor;
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


            public static readonly Dictionary<String, Func<SSExpression[], SSScope, SSObject>>
                BuiltinFunctions =
                    new Dictionary<String, Func<SSExpression[], SSScope, SSObject>>(512);

            //public static readonly Dictionary<String, Func<SSExpression[], SSScope, SSObject>>
            //BuiltinFunctions =
            //new Dictionary<String, Func<SSExpression[], SSScope, SSObject>>(512)
            //{
            //    {"def",  Functions.Flow.Def},
            //        {"def", Functions.Flow.Def},
            //       {"undef", Functions.Flow.Undef},
            //       {"clear-scope", Functions.Flow.ClearScope},
            //       {"set!", Functions.Flow.Set},
            //       {"func", Functions.Flow.Func},
            //       {"if", Functions.Flow.If},
            //       {"while", Functions.Flow.While},
            //       {"loop", Functions.Flow.Loop},
            //       {"switch", Functions.Flow.Switch},
            //       {"try", Functions.Flow.TryCatch},
            //       {"error", Functions.Flow.Error},
            //                          {"open-file", Functions.IO.OpenFile},
            //       {"read-str", Functions.IO.ReadStr},
            //       {"load", Functions.IO.Load},
            //// misc functions
            //       {"debug", Functions.Misc.Debug},
            //       {"rand", Functions.Misc.Rand},
            //       {"hash", Functions.Misc.Hash},
            //       {"clone", (nsargs, scope) =>
            //           (scope.Define(nsargs[0].Token.Value, (SSObject)nsargs[1].Evaluate(scope).Clone()))},
            //       {"typeof", (nsargs, scope) => (SSString)nsargs[0].Evaluate(scope).GetType().FullName},
            //       {"alias", Functions.Misc.Alias},
            //       {"rename", Functions.Misc.Rename},
            //       {"remove", Functions.Misc.Remove},
            //       {"swap", Functions.Misc.Swap},

            //       // numeric
            //       {"+", Functions.Numbers.Add},
            //       {"-", Functions.Numbers.Sub},
            //       {"*", Functions.Numbers.Mul},
            //       {"**", Functions.Numbers.Power},
            //       {"/", (nsargs, scope)=>nsargs.Ops<SSNumber>(scope, (a, b)=>a.FloatVal() / b.FloatVal())},
            //       {"%", Functions.Numbers.Mod},
            //       {"abs", Functions.Numbers.Abs},
            //       {"log", Functions.Numbers.Log},
            //       {"exp", Functions.Numbers.Log},
            //       {"sin", Functions.Numbers.Sin},
            //       {"cos", Functions.Numbers.Cos},
            //       {"tan", Functions.Numbers.Tan)

            //       {"eq?", (nsargs, scope) => nsargs.Ops<SSObject>(scope, (a, b)=>a.Eq(b))},
            //       {"==", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.Eq(s2))},
            //       {"nq?", (nsargs, scope) => nsargs.Ops<SSObject>(scope, (a, b) => !a.Eq(b))},
            //       {"!=", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => !s1.Eq(s2))},

            //       {">", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() > s2.FloatVal())},
            //       {"<", (nsargs, scope) =>
            //           (((SSNumber)nsargs[0].Evaluate(scope)).FloatVal()
            //                < ((SSNumber)nsargs[1].Evaluate(scope)).FloatVal())},

            //       {">=", (nsargs, scope) => nsargs.ChainRelation(scope, (s1, s2) => s1.FloatVal() >= s2.FloatVal()))
            //       {"<=", (nsargs, scope) => nsargs.ChainRelation(scope,
            //                                        (s1, s2) => s1.FloatVal() <= s2.FloatVal())},

            //       {"&", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a & b)},
            //       {"|", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a | b)},
            //       {"^", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => a ^ b)},
            //       {"~", (nsargs, scope) => nsargs.BitOp(scope, a => ~a)},
            //       {"<<", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => ((int)a << (int)b))},
            //       {">>", (nsargs, scope) => nsargs.BitOps(scope, (a, b) => ((int)a >> (int)b))},

            //       {"and", (nsargs, scope) => nsargs.ChainRelation(scope,
            //                                                (s1, s2) => s1.IntVal() == 1 && s2.IntVal() == 1)},
            //       {"or", (nsargs, scope) => nsargs.ChainRelation(scope,
            //                                                (s1, s2) => s1.IntVal() == 1 || s2.IntVal() == 1)},
            //       {"not", (nsargs, scope) => nsargs.Op<SSBool>(scope, (a) => !a)},
            //       {"xor", (nsargs, scope) => nsargs.Ops<SSBool>(scope,
            //                                                (a, b) => (!a && b) || (!b && a))},

            //       {"xnor", (nsargs, scope) => nsargs.Ops<SSBool>(scope,
            //                                                (a, b) => (!a && !b) || (b && a))},
            //   // list processing
            //       {"car", (nsargs, scope) => nsargs.RetrieveSList(scope, "car").First()},
            //       {"cdr", (nsargs, scope) => new SSList(nsargs.RetrieveSList(scope, "cdr").Skip(1))},
            //       {"cons", Functions.Seqs.Cons},
            //       {"list?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSList},
            //       {"char?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSChar},
            //       {"string?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSString},
            //       {"integer?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSInteger},
            //       {"float?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSFloat},
            //       {"dict?", (nsargs, scope) => nsargs[0].Evaluate(scope) is SSDict},
            //       {"null?", Functions.Seqs.IsNull},

            //        // append atom + list || list + atom || list + list
            //       {"append", Functions.Seqs.Append},
            //       {"pop-back!", Functions.Seqs.Popback},
            //       {"pop-front!", Functions.Seqs.PopFront},
            //       {"set-at!", Functions.Seqs.SetAt},
            //       {"elem-at", Functions.Seqs.ElemAt},

            //       {"to-str", Functions.Seqs.ToStr},
            //       {"to-int", Functions.Seqs.ToInt},
            //       {"to-float", Functions.Seqs.ToFloat},
            //       {"to-list", Functions.Seqs.StrToLs},
            //        // (length a-list) || (length a-str},
            //       {"length", Functions.Seqs.Length}
            //};




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
