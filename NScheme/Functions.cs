﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScheme.compiler;

namespace NScheme
{
    namespace compiler
    {
        static class Functions
        {
            public static class Numbers
            {
                public static NSObject Add(IEnumerable<NSExpression> args, NSScope scope)
                {
                    var iter = args.Select(obj => obj.Evaluate(scope)).GetEnumerator();
                    double db = args.Select(a => a.Evaluate(scope)).Where(a => a is NSFloat).Cast<NSFloat>().Sum(a => a);
                    Int64 i = args.Select(a => a.Evaluate(scope)).Where(a => a is NSInteger).Cast<NSInteger>().Sum(a => a);
                    return -0.000001 < db && db < 0.000001 ? i : db + i;
                }

                public static NSObject Sub(NSExpression[] args, NSScope scope)
                {
                    var first = args[0].Evaluate(scope);
                    var b = first is NSFloat ? (NSFloat)first : (NSInteger)first;
                    if (args.Length == 1)
                        return - b;
                    else
                    {
                        var sec = Add(args.Skip(1), scope);
                        var c = sec is NSFloat ? (NSFloat)sec : (NSInteger)sec;
                        return b - c;
                    }
                }


                public static NSObject Power(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var nums = args.Select(o => o.Evaluate(scope)).ToArray();
                    var i1 = nums[0] is NSFloat ? (NSFloat)nums[0] : (NSInteger)nums[0];
                    var i2 = nums[1] is NSFloat ? (NSFloat)nums[1] : (NSInteger)nums[1];
                    return Math.Pow(i1, i2);
                }

                public static NSObject Mul(NSExpression[] args, NSScope scope)
                {
                    var iter = args.Select(obj => obj.Evaluate(scope)).GetEnumerator();
                    double db = 1.0;
                    var dbE = args.Select(a => a.Evaluate(scope))
                                    .Where(a => a is NSFloat);
                    if (dbE.Count() > 0)
                        db = dbE.Cast<NSFloat>().
                                    Aggregate((a, b) => a * b);

                    Int64 i = 1;
                    var iE = args.Select(a => a.Evaluate(scope))
                                    .Where(a => a is NSInteger);
                    if (iE.Count() > 0)
                        i = iE.Cast<NSInteger>().
                                    Aggregate((a, b) => a * b);

                    return -0.000000001 < db && db < 0.000000001 ? i : db * i;
                }

                public static NSObject Mod(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var two = args.Evaluate<NSInteger>(scope).ToArray();
                    
                    return two[0] % two[1];
                }

                public static NSObject Abs(NSExpression[] args, NSScope scope)
                {
                    //(args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var ret = args.Evaluate(scope).First();
                    return ret is NSFloat ? Math.Abs((NSFloat)ret) : Math.Abs((NSInteger)ret);
                }

                public static NSObject Eq(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                    var nums = args.Select(o => o.Evaluate(scope)).ToArray();
                    return nums[0].Eq(nums[1]);
                }

                public static NSObject Set(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameters");
                    var b0 = args[0];
                    var b1 = args[1].Evaluate(scope);
                    scope.Undefine(b0.Token.Value);
                    scope.Define(b0.Token.Value, b1);
                    return NSBool.NSTrue;
                }

            }

            public static class Booleans
            {
                public static NSBool And(NSExpression[] args, NSScope scope)
                {
                    (args.Length > 1).OrThrows("expect two or more parameter");
                    return args.All(arg => (NSBool)args.Evaluate(scope));
                }

                public static NSObject Or(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameters");
                    return args.Any(arg => (NSBool)arg.Evaluate(scope));
                }

                public static NSObject Not(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 1).OrThrows("expect one parameter");
                    return !args.Any(arg => (NSBool)arg.Evaluate(scope));
                }

                public static NSObject Xor(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect two parameter");
                    var ret = args.Evaluate<NSBool>(scope);
                    var b0 = ret.ElementAt(0);
                    var b1 = ret.ElementAt(1);
                    return !b0 && b1 || b0 && !b1;
                }

                public static NSObject Xnor(NSExpression[] args, NSScope scope)
                {
                    (args.Length == 2).OrThrows("expect one or more parameter");
                    var ret = args.Evaluate<NSBool>(scope);
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
                public static NSObject Length(NSExpression[] args, NSScope scope)
                {
                    //nsargs.Evaluate<NSList>(scope).First().Length
                    (args.Length == 1).OrThrows("expect one parameter");
                    var v = args.Evaluate(scope).First();
                    if (v is NSList)
                        return ((NSList)v).Length;
                    else
                        return ((NSString)v).val.Length;
                }

                public static NSObject Cons(NSExpression[] args, NSScope scope)
                {
                    NSList list = null;
                    NSObject atom = null;
                    (args.Length == 2
                        && (atom = (args[0].Evaluate(scope) as NSObject)) != null
                        && (list = (args[1].Evaluate(scope) as NSList)) != null
                    ).OrThrows("Expect atom and list, get[{0}] and [{1}]".Fmt(args[0].Evaluate(scope).GetType(), args[1].Evaluate(scope).GetType()));
                    var ls = new List<NSObject>(list);
                    ls.Insert(0, atom);
                    return new NSList(ls);

                }

                /// <summary>
                /// append atom + list || list + atom || list + list
                /// </summary>
                /// <param name="args"></param>
                /// <param name="scope"></param>
                /// <returns></returns>
                public static NSObject Append(NSExpression[] args, NSScope scope)
                {

                    NSObject o1 = args[0].Evaluate(scope);
                    NSObject o2 = args[1].Evaluate(scope);
                    NSList nsl;//o1 is NSList ? (NSList)o1 : (NSList)o2;
                    if (o1 is NSList)
                        nsl = (NSList)o1;
                    else
                    {
                        nsl = (NSList)o2;
                        o2 = o1;
                    }

                    if (o2 is NSList)
                        return new NSList(nsl.Concat((NSList)o2));
                    else
                    {
                        var nl = new NSList(nsl);
                        nl.ls.Add(o2);
                        return nl;
                    }
                }

                public static NSObject Popback(NSExpression[] args, NSScope scope)
                {
                    NSList list = null;
                    (args.Length == 1
                        && (list = (args[0].Evaluate(scope) as NSList)) != null
                    ).OrThrows("Expect one list, get [{0}]".Fmt(args[0].Evaluate(scope).GetType()));

                    return list.PopBack();
                }

                public static NSObject PopFront(NSExpression[] args, NSScope scope)
                {
                    NSList list = null;
                    (args.Length == 1
                        && (list = (args[0].Evaluate(scope) as NSList)) != null
                    ).OrThrows("Expect one list, get [{0}]".Fmt(args[0].Evaluate(scope).GetType()));

                    return list.PopFront();
                }

                public static NSObject SetAt(NSExpression[] args, NSScope scope)
                {
                    NSList list = null;
                    NSInteger idx = null;
                    NSObject newval = null;
                    (args.Length == 3
                        && (list = (args[0].Evaluate(scope) as NSList)) != null
                        && (idx = (args[1]).Evaluate(scope) as NSInteger) != null
                        && (newval = (args[2]).Evaluate(scope)) != null
                    ).OrThrows("Expect a list, an integer and a new NSObject, get [{0}] and [{1}]]"
                                .Fmt(args[0].Evaluate(scope).GetType(),
                                                    args[1].Evaluate(scope).GetType()
                                                ));
                    list[(int)idx] = newval;
                    return newval;
                }

                public static NSObject GetAt(NSExpression[] args, NSScope scope)
                {
                    NSList list = null;
                    NSInteger idx = null;
                    (args.Length == 2
                        && (list = (args[0].Evaluate(scope) as NSList)) != null
                        && (idx = (args[1]).Evaluate(scope) as NSInteger) != null
                    ).OrThrows("Expect a list and an integer , get [{0}] and [{1}]"
                                .Fmt(args[0].Evaluate(scope).GetType(), args[1].Evaluate(scope).GetType()));
                    return list.ElementAt((int)idx);
                }

            }

            public static class IO
            {

            }

        }
    }

}
