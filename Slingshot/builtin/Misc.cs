
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


                public static SSObject Rand(SSExpression[] exps, SSScope scope)
                {
                    return exps.Length == 1
                        ? scope.Rand.Next((int)((SSInteger)exps[0].Evaluate(scope)))
                        : scope.Rand.Next();
                }

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