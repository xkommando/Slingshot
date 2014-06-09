using System;
using System.Collections.Generic;
using System.Linq;
using Slingshot.Objects;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace BuiltIn
    {
        public partial struct Functions
        {
            public struct Flow
            {

                public static SSObject Def(SSExpression[] exps, SSScope scope)
                {
                    return scope.Define(exps[0].Token.Value, exps[1].Evaluate(scope));
                }

                public static SSObject Undef(SSExpression[] exps, SSScope scope)
                {
                    return scope.Undefine(exps[0].Token.Value);
                }

                public static SSObject Set(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameters");
                    var b0 = args[0];
                    var b1 = args[1].Evaluate(scope);
                    scope.Undefine(b0.Token.Value);
                    scope.Define(b0.Token.Value, b1);
                    return b1;
                }

                public static SSObject Func(SSExpression[] exps, SSScope scope)
                {
                    var parameters = exps[0].Children.Select(exp => exp.Token).ToArray();
                    var body = exps[1];
                    var nsc = new SSScope(scope);
                    return new SSFunction(body, parameters, nsc);
                }


                public static SSObject If(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 3).OrThrows("expect three parameters");
                    return (SSBool) exps[0].Evaluate(scope) ? exps[1].Evaluate(scope) : exps[2].Evaluate(scope);
                }

                public static SSObject While(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 2).OrThrows("expect two parameters");
                    SSObject ret = null;
                    while ((SSBool) exps[0].Evaluate(scope))
                        ret = exps[1].Evaluate(scope);
                    return ret;
                }


                public static SSObject Loop(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 2).OrThrows("expect two parameters");
                    int num = (SSInteger) exps[0].Evaluate(scope);
                    SSObject ret = null;
                    while (num-- != 0)
                    {
                        ret = exps[1].Evaluate(scope);
                    }

                    return ret;
                }

                public static SSObject Error(SSExpression[] exps, SSScope scope)
                {
                    var ret = exps.Evaluate(scope);
                    ret.ForEach(a => scope.Output.WriteLine(a));
                    throw new SystemException(ret.Last().ToString());
                }
            
            }
        }
    }
}
