using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Objects;
using Slingshot.Compiler;
using Slingshot.Interpretor;

namespace Slingshot
{
    namespace BuiltIn
    {
        public partial struct Functions
        {
            public struct Lists
            {
                /// <summary>
                /// (length a-list) || (length a-str)
                /// </summary>
                public static SSObject Length(SSExpression[] exps, SSScope scope)
                {
                    //nsexps.Evaluate<NSList>(scope).First().Length
                    (exps.Length == 1).OrThrows("expect one parameter");
                    var v = exps.Evaluate(scope).First();
                    if (v is SSList)
                        return ((SSList)v).Length;
                    else
                        return ((SSString)v).Val.Length;
                }

                public static SSObject IsNull(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one list or string");
                    var v = exps[0].Evaluate(scope);
                    if (v is SSList)
                        return ((SSList)v).Length < 1;
                    else
                        return ((SSString)v).Val.Length < 1;
                }


                public static SSObject StrToLs(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one parameter");
                    var v = ((SSString)exps.Evaluate(scope).First());
                    return new SSList(v.Val.Select(a => new SSChar(a)));
                }

                public static SSObject LsToStr(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one parameter");
                    var ls = ((SSList)exps.Evaluate(scope).First());
                    var sb = new StringBuilder(ls.Length * 5);
                    ls.ForEach(a => sb.Append(a));
                    return sb.ToString();
                }

                public static SSObject Cons(SSExpression[] exps, SSScope scope)
                {
                    SSList list = null;
                    SSObject atom = null;
                    (exps.Length == 2
                        && (atom = (exps[0].Evaluate(scope) as SSObject)) != null
                        && (list = (exps[1].Evaluate(scope) as SSList)) != null
                    ).OrThrows("Expect atom and list, get[{0}] and [{1}]"
                                .Fmt(exps[0].Evaluate(scope).GetType(),
                                exps[1].Evaluate(scope).GetType()));

                    var ls = new List<SSObject>(list);
                    ls.Insert(0, atom);
                    return new SSList(ls);

                }

                /// <summary>
                /// append atom + list || list + atom || list + list
                /// </summary>
                public static SSObject Append(SSExpression[] exps, SSScope scope)
                {
                    SSObject o1 = exps[0].Evaluate(scope);
                    SSObject o2 = exps[1].Evaluate(scope);
                    SSList nsl;//o1 is NSList ? (NSList)o1 : (NSList)o2;
                    //Console.WriteLine("======"+ (o1 is SSList) + "  " + "   "  + (o2 is SSList));
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

                public static SSObject Popback(SSExpression[] exps, SSScope scope)
                {
                    SSList list = null;
                    (exps.Length == 1
                        && (list = (exps[0].Evaluate(scope) as SSList)) != null
                    ).OrThrows("Expect one list, get [{0}]".Fmt(exps[0].Evaluate(scope).GetType()));

                    return list.PopBack();
                }

                public static SSObject PopFront(SSExpression[] exps, SSScope scope)
                {
                    SSList list = null;
                    (exps.Length == 1
                        && (list = (exps[0].Evaluate(scope) as SSList)) != null
                    ).OrThrows("Expect one list, get [{0}]".Fmt(exps[0].Evaluate(scope).GetType()));

                    return list.PopFront();
                }

                public static SSObject SetAt(SSExpression[] exps, SSScope scope)
                {
                    SSList list = null;
                    SSInteger idx = null;
                    SSObject newval = null;
                    (exps.Length == 3
                        && (list = (exps[0].Evaluate(scope) as SSList)) != null
                        && (idx = (exps[1]).Evaluate(scope) as SSInteger) != null
                        && (newval = (exps[2]).Evaluate(scope)) != null
                    ).OrThrows("Expect a list, an integer and a new NSObject, get [{0}] and [{1}]]"
                                .Fmt(exps[0].Evaluate(scope).GetType(),
                                                    exps[1].Evaluate(scope).GetType()
                                                ));
                    list[(int)idx] = newval;
                    return newval;
                }

                public static SSObject ElemAt(SSExpression[] exps, SSScope scope)
                {
                    SSList list = null;
                    SSInteger idx = null;
                    (exps.Length == 2
                        && (list = (exps[0].Evaluate(scope) as SSList)) != null
                        && (idx = (exps[1]).Evaluate(scope) as SSInteger) != null
                    ).OrThrows("Expect a list and an integer , get [{0}] and [{1}]"
                                .Fmt(exps[0].Evaluate(scope).GetType(), exps[1].Evaluate(scope).GetType()));
                    return list.ElementAt((int)idx);
                }

            }
        }
    }
}
