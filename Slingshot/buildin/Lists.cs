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
            public struct Lists
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
                    ls.ForEach(a => sb.Append(a));
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
        }
    }
}
