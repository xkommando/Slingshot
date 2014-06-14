
using System;
using System.Linq;
using Slingshot.Objects;
using Slingshot.Compiler;
using Slingshot.Interpretor;

namespace Slingshot
{
    namespace BuiltIn
    {
        public partial struct Functions
        {
            public struct Misc
            {
                /// <summary>
                /// (test to-print to-eval )
                /// </summary>
                public static SSObject Debug(SSExpression[] exps, SSScope scope)
                {
                    var ret = exps.Evaluate(scope);
                    var oput = scope.Output;
                    oput.WriteLine(" ---> Debug {0}".Fmt(DateTime.Now.ToString("h:mm:ss")));
                    exps.ForEach(a =>
                    {
                        oput.Write(a.Evaluate(scope));
                        oput.Write("    ");
                        //var c = a;
                        //var ct = 4;
                        //while (ct-- != 0 && c.Parent != null)
                        //{
                        //    c = c.Parent;
                        //}
                        //c.Print(scope.Output);
                        //scope.Output.WriteLine();
                    });
                    oput.WriteLine("---------------");
                    return "";
                }

                /// <summary>
                /// (alias old-name new-name)
                /// </summary>
                public static SSObject Alias(SSExpression[] exps, SSScope scope)
                {
                    string oname = exps[0].Token.Value;
                    string nname = exps[1].Token.Value;
                    (!oname.Equals(nname)).OrThrows("same name {0}".Fmt(oname));
                    try
                    {
                        var obj = scope.Find(oname);
                        scope.Define(nname, obj);
                        return nname + "   "  + obj.ToString();
                    }
                    catch (Exception)
                    {
                        Func<SSExpression[], SSScope, SSObject> obj = null;
                        if (SSScope.BuiltinFunctions.TryGetValue(oname, out obj))
                        {
                           SSScope.BuiltinFunctions[nname] = obj;
                            return nname + "   " + obj.ToString();
                        }
                        throw new Exception("cannot find {0} in scope, and it is not a builtin".Fmt(nname));
                    }
                }

                /// <summary>
                /// (rename old-name new-name)
                /// </summary>
                public static SSObject Rename(SSExpression[] exps, SSScope scope)
                {
                    string oname = exps[0].Token.Value;
                    string nname = exps[1].Token.Value;
                    (!oname.Equals(nname)).OrThrows("same name {0}".Fmt(oname));
                    try
                    {
                        var obj = scope.Find(oname);
                        scope.Define(nname, obj);
                        scope.Undefine(oname);
                        return nname + "   " + obj.ToString();
                    }
                    catch (Exception)
                    {
                        Func<SSExpression[], SSScope, SSObject> obj = null;
                        if (SSScope.BuiltinFunctions.TryGetValue(oname, out obj))
                        {
                            SSScope.BuiltinFunctions[nname] = obj;
                            SSScope.BuiltinFunctions.Remove(oname);
                            return nname + "   " + obj.ToString();
                        }
                        throw new Exception("cannot find {0} in scope, and it is not a builtin".Fmt(nname));
                    }
                }

                public static SSObject Remove(SSExpression[] exps, SSScope scope)
                {
                    string oname = exps[0].Token.Value;
                    try
                    {
                        var obj = scope.Remove(oname);
                        return oname + "  removed  ";
                    }
                    catch (Exception)
                    {
                        if (SSScope.BuiltinFunctions.Remove(oname))
                        {
                            return oname + "  removed  ";
                        }
                        throw new Exception("cannot find {0} in scope, and it is not a builtin".Fmt(oname));
                    }
                }

                public static SSObject Swap(SSExpression[] exps, SSScope scope)
                {
                    var na = exps[0].Token.Value;
                    var va = exps[0].Evaluate(scope);
                    var nb = exps[1].Token.Value;
                    var vb = exps[1].Evaluate(scope);
                    scope.Undefine(nb);
                    scope.Undefine(na);
                    scope.Define(nb, va);
                    scope.Define(na, vb);
                    return true;
                }

                /// <summary>
                /// (rand)
                /// (rand 15) 
                /// </summary>
                public static SSObject Rand(SSExpression[] exps, SSScope scope)
                {
                    return exps.Length == 1
                        ? scope.Rand.Next((int)((SSInteger)exps[0].Evaluate(scope)))
                        : scope.Rand.Next();
                }

                /// <summary>
                /// (hash any-thing)
                /// </summary>
                public static SSObject Hash(SSExpression[] exps, SSScope scope)
                {
                    int h = 1;
                    exps.Evaluate(scope).ForEach(a => h = 31 * h + a.GetHashCode());
                    return h;
                }
            }

        }
    }
}