using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Slingshot.Objects;
using Slingshot.Compiler;
using Slingshot.Interpretor;

namespace Slingshot
{
    namespace BuiltIn
    {
        public partial struct Functions
        {
            public struct Seqs
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

                /// <summary>
                /// null? list null? str
                /// </summary>
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

                public static SSObject ToStr(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one parameter");
                    var v = exps.Evaluate(scope).First();
                    if (v is SSList)
                    {
                        var ls = v as SSList;
                        var sb = new StringBuilder(ls.Length*5);
                        ls.ForEach(a => sb.Append((SSChar) a));
                        return sb.ToString();
                    }
                    else if (v is SSString)
                        return v;
                    else
                        return v.ToString();
                }

                public static SSObject ToInt(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one parameter");
                    var ls = exps.Evaluate(scope).First();
                    if (ls is SSString)
                    {
                        return Int64.Parse((ls as SSString).Val.Trim());
                    }
                    else if (ls is SSNumber)
                        return (ls as SSNumber).IntVal();
                    else
                        throw new ArgumentException("expect string or ssnumber, get {0}".Fmt(ls.GetType()));
                }

                public static SSObject ToFloat(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one parameter");
                    var ls = exps.Evaluate(scope).First();
                    if (ls is SSString)
                    {
                        return double.Parse((ls as SSString).Val.Trim());
                    }
                    else if (ls is SSNumber)
                        return (ls as SSNumber).FloatVal();
                    else
                        throw new ArgumentException("expect string or ssnumber, get {0}".Fmt(ls.GetType()));
                }
                /// <summary>
                /// cons 
                /// atom + list
                /// atom + string
                /// </summary>
                public static SSObject Cons(SSExpression[] exps, SSScope scope)
                {
                    SSObject atom = null;
                    (exps.Length == 2
                        && (atom = (exps[0].Evaluate(scope))) != null
                    ).OrThrows("Expect atom/char and list/string)");
                    var b2 = exps[1].Evaluate(scope);

                    if (b2 is SSList)
                    {
                        var ls = new List<SSObject>(b2 as SSList);
                        ls.Insert(0, atom);
                        return new SSList(ls);
                    }
                    else if (b2 is SSString)
                    {
                        var b = new StringBuilder(b2 as SSString);
                        b.Insert(0, (SSChar) atom);
                        return b.ToString();
                    }
                    throw new SystemException("expect list or string");
                }

                /// <summary>
                /// append 
                /// list + atom|| list + list 
                /// string + char || string + string
                /// </summary>
                /// 
                public static SSObject Append(SSExpression[] exps, SSScope scope)
                {
                    SSObject o1 = exps[0].Evaluate(scope);
                    SSObject o2 = exps[1].Evaluate(scope);
                    if (o1 is SSList)
                    {
                        var nls = new List<SSObject>((o1 as SSList).Val.Count * 2);
                        nls.AddRange(o1 as SSList);
                        if (o2 is SSList)
                            nls.AddRange(o2 as SSList);
                        else
                            nls.Add(o2);
                        return new SSList(nls);
                    }
                    else if (o1 is SSString)
                    {
                        var str = o1 as SSString;
                        var nls = new StringBuilder(str.Val.Length * 2);
                        nls.Append(str);
                        if (o2 is SSString)
                            nls.Append(o2 as SSString);
                        else
                            nls.Append((SSChar)o2);

                        return nls.ToString();
                    }
                    throw new SystemException("expect list or string");
                }

                public static SSObject Popback(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1
                    ).OrThrows("Expect one list or string");
                    var cont = exps[0].Evaluate(scope);
                    if (cont is SSString)
                    {
                        var str = cont as SSString;
                        int len = str.Val.Length;
                        char ret = str.Val[len - 1];
                        str.Replace(str.Val.Remove(len - 1));
                        return ret;
                    }
                    else if (cont is SSList)
                        return ((SSList)cont).PopBack();
                    else
                        throw new SystemException("not a list or string");
                }

                public static SSObject PopFront(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1
                    ).OrThrows("Expect one list or string");
                    var cont = exps[0].Evaluate(scope);
                    if (cont is SSString)
                    {
                        var b = new StringBuilder((SSString)cont);
                        char ret = b[0];
                        b.Remove(0, 1);
                        ((SSString) cont).Replace(b.ToString());
                        return ret;
                    }
                    else if (cont is SSList)
                        return ((SSList) cont).PopFront();
                    else
                        throw new SystemException("not a list or string");
                }

                /// <summary>
                /// 
                /// (set-at! p-str 4 'x') 
                /// </summary>
                public static SSObject SetAt(SSExpression[] exps, SSScope scope)
                {
                    SSObject newval = null;
                    (exps.Length == 3
                        && (newval = (exps[2]).Evaluate(scope)) != null
                    ).OrThrows("Expect a container, an integer and a new NSObject");

                    var nv = exps[1].Evaluate(scope);
                    var cont = exps[0].Evaluate(scope);
                    if (cont is SSList)
                        ((SSList)cont).Val[(SSInteger)nv] = newval;
                    else if (cont is SSString)
                    {
                        var b = new StringBuilder((SSString)cont);
                        b[(SSInteger)nv] = (SSChar)newval;
                        cont.Replace(b.ToString());
                    }
                    else if (cont is SSDict)
                        ((SSDict)cont).Val[nv] = newval;

                    return newval;
                }

                public static SSObject ElemAt(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 2).OrThrows("Expect a list and an integer ");

                    var ret = exps[0].Evaluate(scope);
                    var idx = exps[1].Evaluate(scope);
                    if (ret is SSList)
                        return ((SSList)ret)[(SSInteger)idx];
                    else if (ret is SSString)
                        return ((SSString)ret).Val[(SSInteger)idx];
                    else if (ret is SSDict)
                        return ((SSDict)ret).Val[idx];
                    else
                        throw new SystemException("wrong type {0}".Fmt(ret.GetType()));
                }

            }
        }
    }
}
