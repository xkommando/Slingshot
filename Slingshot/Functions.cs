using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.compiler;

namespace Slingshot
{
    namespace compiler
    {
        static class Functions
        {
            public static class Numbers
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

            public static class Booleans
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

            public static class Lists
            {
                /// <summary>
                /// (length a-list) || (length a-str)
                /// </summary>
                public static SSObject Length(SSExpression[] args, SSScope scope)
                {
                    //nsargs.Evaluate<NSList>(scope).First().Length
                    (args.Length == 1).OrThrows("expect one parameter");
                    var v = args.Evaluate(scope).First();
                    if (v is SSList)
                        return ((SSList)v).Length;
                    else
                        return ((SSString)v).Val.Length;
                }

                public static SSObject IsNull(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 1).OrThrows("expect one list or string");
                    var v = args[0].Evaluate(scope);
                    if (v is SSList)
                        return ((SSList)v).Length < 1;
                    else
                        return ((SSString)v).Val.Length < 1;
                }

                
                public static SSObject StrToLs(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 1).OrThrows("expect one parameter");
                    var v = ((SSString)args.Evaluate(scope).First());
                    return new SSList(v.Val.Select(a => new SSChar(a)));
                }

                public static SSObject LsToStr(SSExpression[] args, SSScope scope)
                {
                    (args.Length == 1).OrThrows("expect one parameter");
                    var ls = ((SSList)args.Evaluate(scope).First());
                    var sb = new StringBuilder(ls.Length * 5);
                    ls.ForEach(a=>sb.Append(a));
                    return sb.ToString();
                }

                public static SSObject Cons(SSExpression[] args, SSScope scope)
                {
                    SSList list = null;
                    SSObject atom = null;
                    (args.Length == 2
                        && (atom = (args[0].Evaluate(scope) as SSObject)) != null
                        && (list = (args[1].Evaluate(scope) as SSList)) != null
                    ).OrThrows("Expect atom and list, get[{0}] and [{1}]"
                                .Fmt(args[0].Evaluate(scope).GetType(), 
                                args[1].Evaluate(scope).GetType()));

                    var ls = new List<SSObject>(list);
                    ls.Insert(0, atom);
                    return new SSList(ls);

                }

                /// <summary>
                /// append atom + list || list + atom || list + list
                /// </summary>
                /// <param name="args"></param>
                /// <param name="scope"></param>
                /// <returns></returns>
                public static SSObject Append(SSExpression[] args, SSScope scope)
                {

                    SSObject o1 = args[0].Evaluate(scope);
                    SSObject o2 = args[1].Evaluate(scope);
                    SSList nsl;//o1 is NSList ? (NSList)o1 : (NSList)o2;
                    if (o1 is SSList)
                        nsl = (SSList)o1;
                    else
                    {
                        nsl = (SSList)o2;
                        o2 = o1;
                    }

                    if (o2 is SSList)
                        return new SSList(nsl.Concat((SSList)o2));
                    else
                    {
                        var nl = new SSList(nsl);
                        nl.Val.Add(o2);
                        return nl;
                    }
                }

                public static SSObject Popback(SSExpression[] args, SSScope scope)
                {
                    SSList list = null;
                    (args.Length == 1
                        && (list = (args[0].Evaluate(scope) as SSList)) != null
                    ).OrThrows("Expect one list, get [{0}]".Fmt(args[0].Evaluate(scope).GetType()));

                    return list.PopBack();
                }

                public static SSObject PopFront(SSExpression[] args, SSScope scope)
                {
                    SSList list = null;
                    (args.Length == 1
                        && (list = (args[0].Evaluate(scope) as SSList)) != null
                    ).OrThrows("Expect one list, get [{0}]".Fmt(args[0].Evaluate(scope).GetType()));

                    return list.PopFront();
                }

                public static SSObject SetAt(SSExpression[] args, SSScope scope)
                {
                    SSList list = null;
                    SSInteger idx = null;
                    SSObject newval = null;
                    (args.Length == 3
                        && (list = (args[0].Evaluate(scope) as SSList)) != null
                        && (idx = (args[1]).Evaluate(scope) as SSInteger) != null
                        && (newval = (args[2]).Evaluate(scope)) != null
                    ).OrThrows("Expect a list, an integer and a new NSObject, get [{0}] and [{1}]]"
                                .Fmt(args[0].Evaluate(scope).GetType(),
                                                    args[1].Evaluate(scope).GetType()
                                                ));
                    list[(int)idx] = newval;
                    return newval;
                }

                public static SSObject GetAt(SSExpression[] args, SSScope scope)
                {
                    SSList list = null;
                    SSInteger idx = null;
                    (args.Length == 2
                        && (list = (args[0].Evaluate(scope) as SSList)) != null
                        && (idx = (args[1]).Evaluate(scope) as SSInteger) != null
                    ).OrThrows("Expect a list and an integer , get [{0}] and [{1}]"
                                .Fmt(args[0].Evaluate(scope).GetType(), args[1].Evaluate(scope).GetType()));
                    return list.ElementAt((int)idx);
                }

            }

            public static class IO
            {

                public static SSObject OutStream(SSExpression[] args, SSScope scope)
                {
                    return 0;
                }

                public static SSObject InStream(SSExpression[] args, SSScope scope)
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
                public static SSObject IOFile(SSExpression[] args, SSScope scope)
                {
                    return 0;
                }

                public static SSObject LoadAndEval(SSExpression[] args, SSScope scope)
                {
                    return 0;
                }
            }

        }
    }

}
