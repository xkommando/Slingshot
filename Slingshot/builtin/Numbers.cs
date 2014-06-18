using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Objects;
using Slingshot.Compiler;
using Slingshot.Interpretor;

namespace Slingshot.BuiltIn
{
    public partial struct Functions
    {

        public struct Numbers
        {
            public static SSObject Add(IEnumerable<SSExpression> exps, SSScope scope)
            {
                var iter = exps.Select(obj => obj.Evaluate(scope)).GetEnumerator();

                double db = exps.Select(exp => exp.Evaluate(scope))
                                                .Where(exp => exp is SSFloat)
                                                .Cast<SSFloat>()
                                                .Sum(v => v);

                Int64 i = exps.Select(exp => exp.Evaluate(scope))
                                        .Where(v => v is SSInteger)
                                        .Cast<SSInteger>()
                                        .Sum(v => v);

                if (Math.Abs(db) < 0.000001)
                    return (SSInteger)i;
                else
                    return (SSFloat)i + db;
                //return -0.000001 < db && db < 0.000001 ? (SSInStream)i : (SSFloat)(db + i);
            }

            public static SSObject Sub(SSExpression[] exps, SSScope scope)
            {
                var first = exps[0].Evaluate(scope);
                //var b = first is SSFloat ? (SSFloat)first : (SSInteger)first;
                if (exps.Length == 1)
                    return first is SSFloat ? -(SSFloat)first : -(SSInteger)first;
                else
                {
                    var sec = Add(exps.Skip(1), scope);
                    if (sec is SSFloat || first is SSFloat)
                        return (first as SSNumber).FloatVal() - (sec as SSFloat).FloatVal();
                    else
                        return (SSInteger)first - (SSInteger)sec;
                    //if (sec is SSFloat)
                    //    return b - sec;
                    //var c = sec is SSFloat ? (SSFloat)sec : (SSInteger)sec;
                    //return b - c;
                }
            }


            public static SSObject Power(SSExpression[] exps, SSScope scope)
            {
                (exps.Length == 2).OrThrows("expect 2 parameters, get " + exps.Length);
                var nums = exps.Select(o => o.Evaluate(scope)).ToArray();
                var i1 = (SSNumber)nums[0];//is SSFloat ? (SSFloat)nums[0] : (SSInteger)nums[0];
                var i2 = (SSNumber)nums[1];// is SSFloat ? (SSFloat)nums[1] : (SSInteger)nums[1];
                return Math.Pow(i1.FloatVal(), i2.FloatVal());
            }

            public static SSObject Mul(SSExpression[] exps, SSScope scope)
            {
                var iter = exps.Select(obj => obj.Evaluate(scope)).GetEnumerator();
                double db = 1.0;
                var dbE = exps.Select(exp => exp.Evaluate(scope))
                                .Where(v => v is SSFloat);
                if (dbE.Count() > 0)
                    db = dbE.Cast<SSFloat>().
                                Aggregate((exp, b) => exp * b);

                Int64 i = 1;
                var iE = exps.Select(exp => exp.Evaluate(scope))
                                .Where(exp => exp is SSInteger);

                if (iE.Count() > 0)
                    i = iE.Cast<SSInteger>().
                                Aggregate((exp, b) => exp * b);

                return -0.0000001 < db && db < 0.0000001 ? i : db * i;
            }

            public static SSObject Abs(SSExpression[] exps, SSScope scope)
            {
                //(exps.Length == 2).OrThrows("expect 2 parameters, get " + exps.Length);
                var ret = exps.Evaluate(scope).First();
                if (ret is SSFloat)
                    return Math.Abs((SSFloat)ret);
                else
                    return Math.Abs((SSInteger)ret);
                //return ret is SSFloat ? Math.Abs((SSFloat)ret) : Math.Abs((SSInteger)ret);
            }
        }

    }

    public static class RelationalExtension
    {


        public static SSObject Ops<T>(this SSExpression[] exps, SSScope scope, Func<T, T, SSObject> ops)
            where T : SSObject
        {
            (exps.Length == 2).OrThrows("expect two parameters");
            var ret = exps.Evaluate<T>(scope);
            var b0 = ret.ElementAt(0);
            var b1 = ret.ElementAt(1);
            return ops(b0, b1);
        }

        public static SSObject Op<T>(this SSExpression[] exps, SSScope scope, Func<T, SSObject> ops)
            where T : SSObject
        {
            (exps.Length == 1).OrThrows("expect one parameter");
            var ret = exps.Evaluate<T>(scope).First();
            return ops(ret);
        }



        public static SSInteger BitOp(this SSExpression[] exps, SSScope scope,
                        Func<SSInteger, SSInteger> ops)
        {
            (exps.Length == 1).OrThrows("expect one or more parameter");
            var ret = exps.Evaluate<SSInteger>(scope).First();
            return ops(ret);
        }

        public static SSInteger BitOps(this SSExpression[] exps, SSScope scope,
                       Func<SSInteger, SSInteger, SSInteger> ops)
        {
            (exps.Length == 2).OrThrows("expect one or more parameter");
            var ret = exps.Evaluate<SSInteger>(scope);
            var b0 = ret.ElementAt(0);
            var b1 = ret.ElementAt(1);
            return ops(b0, b1);
        }
    }

}
