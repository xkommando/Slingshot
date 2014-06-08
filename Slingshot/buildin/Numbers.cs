using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Objects;
using Slingshot.Compiler;
using Slingshot.Interpretor;

namespace Slingshot.BuildIn
{
    public partial struct Functions
    {

        public struct Numbers
        {
            public static SSObject Add(IEnumerable<SSExpression> args, SSScope scope)
            {
                var iter = args.Select(obj => obj.Evaluate(scope)).GetEnumerator();

                double db = args.Select(exps => exps.Evaluate(scope))
                                                .Where(exps => exps is SSFloat)
                                                .Cast<SSFloat>()
                                                .Sum(exps => exps);

                Int64 i = args.Select(exps => exps.Evaluate(scope))
                                        .Where(exps => exps is SSInteger)
                                        .Cast<SSInteger>()
                                        .Sum(exps => exps);

                if (Math.Abs(db) < 0.000001)
                    return (SSInteger)i;
                else
                    return (SSFloat)i + db;
                //return -0.000001 < db && db < 0.000001 ? (SSInStream)i : (SSFloat)(db + i);
            }

            public static SSObject Sub(SSExpression[] args, SSScope scope)
            {
                var first = args[0].Evaluate(scope);
                //var b = first is SSFloat ? (SSFloat)first : (SSInteger)first;
                if (args.Length == 1)
                    return first is SSFloat ? -(SSFloat)first : -(SSInteger)first;
                else
                {
                    var sec = Add(args.Skip(1), scope);
                    if (sec is SSFloat || first is SSFloat)
                        return (SSFloat)first - (SSFloat)sec;
                    else
                        return (SSInteger)first - (SSInteger)sec;
                    //if (sec is SSFloat)
                    //    return b - sec;
                    //var c = sec is SSFloat ? (SSFloat)sec : (SSInteger)sec;
                    //return b - c;
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
                var dbE = args.Select(exps => exps.Evaluate(scope))
                                .Where(exps => exps is SSFloat);
                if (dbE.Count() > 0)
                    db = dbE.Cast<SSFloat>().
                                Aggregate((exps, b) => exps * b);

                Int64 i = 1;
                var iE = args.Select(exps => exps.Evaluate(scope))
                                .Where(exps => exps is SSInteger);

                if (iE.Count() > 0)
                    i = iE.Cast<SSInteger>().
                                Aggregate((exps, b) => exps * b);

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
                return nums[0].Eq(nums[1]);
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

            public static SSObject Rand(SSExpression[] args, SSScope scope)
            {
                return args.Length == 1 ? scope.Rand.Next((int)((SSInteger)args[0].Evaluate(scope)))
                    : scope.Rand.Next();
            }

        }

    }

    public static class RelationalExtension
    {


        public static SSObject Ops<T>(this SSExpression[] args, SSScope scope, Func<T, T, SSObject> ops)
            where T : SSObject
        {
            (args.Length == 2).OrThrows("expect two parameters");
            var ret = args.Evaluate<T>(scope);
            var b0 = ret.ElementAt(0);
            var b1 = ret.ElementAt(1);
            return ops(b0, b1);
        }

        public static SSObject Op<T>(this SSExpression[] args, SSScope scope, Func<T, SSObject> ops)
            where T : SSObject
        {
            (args.Length == 1).OrThrows("expect one parameter");
            var ret = args.Evaluate<T>(scope).First();
            return ops(ret);
        }



        public static SSInteger BitOp(this SSExpression[] args, SSScope scope,
                        Func<SSInteger, SSInteger> ops)
        {
            (args.Length == 1).OrThrows("expect one or more parameter");
            var ret = args.Evaluate<SSInteger>(scope).First();
            return ops(ret);
        }

        public static SSInteger BitOps(this SSExpression[] args, SSScope scope,
                       Func<SSInteger, SSInteger, SSInteger> ops)
        {
            (args.Length == 2).OrThrows("expect one or more parameter");
            var ret = args.Evaluate<SSInteger>(scope);
            var b0 = ret.ElementAt(0);
            var b1 = ret.ElementAt(1);
            return ops(b0, b1);
        }
    }

}
