﻿
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
                public static SSObject Test(SSExpression[] exps, SSScope scope)
                {
                    var ret = exps.Evaluate(scope);
                    var oput = scope.Output;
                    scope.Output.WriteLine("----- LOG -----");
                    if (exps.Length > 0)
                    {
                        var exp = exps[0];
                        if (exp.Parent != null)
                            oput.WriteLine(exp.Parent.Token.Value);
                        oput.WriteLine("\t" + exp.Token.Value);
                        exp.Children.ForEach(a => oput.Write(a.Token.Value + "   "));
                    }
                    oput.WriteLine();
                    ret.ForEach(a => oput.Write("   " + a.ToString()));
                    oput.WriteLine();
                    return "---------------";
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