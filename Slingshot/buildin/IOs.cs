using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;
using Slingshot.Objects;
using Slingshot.Interpretor;

namespace Slingshot
{
    namespace BuildIn
    {

        partial struct Functions
        {

            public struct IO
            {

                public static SSObject Log(SSExpression[] exps, SSScope scope)
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

                public static SSObject OutStream(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }

                public static SSObject InStream(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }
                /// <summary>
                /// // read
                /// (file "a-path" 'r')
                /// 
                /// // read and write
                /// (file "a-path" 'r' 'w')
                /// 
                ///  create append delete...
                /// </summary>
                public static SSObject IOFile(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }

                public static SSObject LoadAndEval(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }
            }
        }

    }
}
