using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Objects;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Buildin
    {
        public partial struct Functions
        {
            public struct Numbers
            {
                public static SSObject Add(IEnumerable<SSExpression> args, SSScope scope)
                {
                    var iter = args.Select(obj => obj.Evaluate(scope)).GetEnumerator();
                    double db = args.Select(a => a.Evaluate(scope)).Where(a => a is SSFloat).Cast<SSFloat>().Sum(a => a);
                    Int64 i = args.Select(a => a.Evaluate(scope)).Where(a => a is SSInteger).Cast<SSInteger>().Sum(a => a);
                    return -0.000001 < db && db < 0.000001 ? i : db + i;
                }

                public static SSObject Sub(SSExpression[] args, SSScope scope)
                {
                    var first = args[0].Evaluate(scope);
                    var b = first is SSFloat ? (SSFloat)first : (SSInteger)first;
                    if (args.Length == 1)
                        return - b;
                    else
                    {
                        var sec = Add(args.Skip(1), scope);
                        var c = sec is SSFloat ? (SSFloat)sec : (SSInteger)sec;
                        return b - c;
                    }
                }


                public static SSObject Power(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var nums = args.Select(o => o.Evaluate(scope)).ToArray();
                    var i1 = nums[0] is SSFloat ? (SSFloat)nums[0] : (SSInteger)nums[0];
                    var i2 = nums[1] is SSFloat ? (SSFloat)nums[1] : (SSInteger)nums[1];
                    return Math.Pow(i1, i2);
                }

                public static SSObject Mul(SSExpression[] args, SSScope scope)
                {
                    var iter = args.Select(obj => obj.Evaluate(scope)).GetEnumerator();
                    double db = 1.0;
                    var dbE = args.Select(a => a.Evaluate(scope))
                                    .Where(a => a is SSFloat);
                    if (dbE.Count() > 0)
                        db = dbE.Cast<SSFloat>().
                                    Aggregate((a, b) => a * b);

                    Int64 i = 1;
                    var iE = args.Select(a => a.Evaluate(scope))
                                    .Where(a => a is SSInteger);
                    if (iE.Count() > 0)
                        i = iE.Cast<SSInteger>().
                                    Aggregate((a, b) => a * b);

                    return -0.0000001 < db && db < 0.0000001 ? i : db * i;
                }

                public static SSObject Mod(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var two = args.Evaluate<SSInteger>(scope).ToArray();
                    
                    return two[0] % two[1];
                }

                public static SSObject Abs(SSExpression[] args, SSScope scope)
                {
                    //(args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var ret = args.Evaluate(scope).First();
                    return ret is SSFloat ? Math.Abs((SSFloat)ret) : Math.Abs((SSInteger)ret);
                }

                public static SSObject Eq(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var nums = args.Select(o => o.Evaluate(scope)).ToArray();
                    return nums[0].Equals(nums[1]);
                }

                public static SSObject Set(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameters");
                    var b0 = args[0];
                    var b1 = args[1].Evaluate(scope);
                    scope.Undefine(b0.Token.Value);
                    scope.Define(b0.Token.Value, b1);
                    return SSBool.NSTrue;
                }

            }

            public struct Booleans
            {
                public static SSBool And(SSExpression[] args, SSScope scope)
                {
                    (args.Length > 1).OrThrows("expect two or more parameter");
                    return args.All(arg => (SSBool)args.Evaluate(scope));
                }

                public static SSObject Or(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameters");
                    return args.Any(arg => (SSBool)arg.Evaluate(scope));
                }

                public static SSObject Not(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 1).OrThrows("expect one parameter");
                    return !args.Any(arg => (SSBool)arg.Evaluate(scope));
                }

                public static SSObject Xor(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameter");
                    var ret = args.Evaluate<SSBool>(scope);
                    var b0 = ret.ElementAt(0);
                    var b1 = ret.ElementAt(1);
                    return !b0 && b1 || b0 && !b1;
                }

                public static SSObject Xnor(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 2).OrThrows("expect one or more parameter");
                    var ret = args.Evaluate<SSBool>(scope);
                    var b0 = ret.ElementAt(0);
                    var b1 = ret.ElementAt(1);
                    return b0 && b1 || !b0 && !b1;
                }
            }

        }
    }

}
