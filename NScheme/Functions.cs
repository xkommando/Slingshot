using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    static class Functions
    {
        public static class Numbers
        {
            public static NSObject Add(NSExpression[] args, NSScope scope)
            {
                //var ret = args.Evaluate<NSInteger>(scope);
                //double dbv = ret.Aggregate((a, b) => (double.IsNaN(b.valFp) ? a.valFp : a.valFp + b.valFp));
                //Int64 intv = ret.Aggregate((a, b) => (double.IsNaN(b.valFp) ? a.val + b.val : a.val));
                return args.Select(obj => obj.Evaluate(scope)).Cast<NSInteger>().Sum(n=>n);
            }

            public static NSObject Sub(NSExpression[] args, NSScope scope)
            {
                var nums = args.Select(obj => obj.Evaluate(scope)).Cast<NSInteger>().ToArray();
                var first = nums[0];
                if (nums.Length == 1)
                    return -first;
                else
                    return first - nums.Skip(1).Sum(v => v);
            }
            public static NSObject Power(NSExpression[] args, NSScope scope)
            {
                (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                var nums = args.Select(o => o.Evaluate(scope)).Cast<NSInteger>().ToArray();
                return (Int64)Math.Pow(nums[0], nums[1]);
            }
            public static NSObject Mul(NSExpression[] args, NSScope scope)
            {
                return args.Evaluate<NSInteger>(scope).Aggregate((a, b) => a * b);
            }
            public static NSObject Mod(NSExpression[] args, NSScope scope)
            {
                var two = args.Evaluate<NSInteger>(scope).ToArray();
                return two[0] % two[1];
            }
            public static NSObject Abs(NSExpression[] args, NSScope scope)
            {
                var ret = args.Evaluate<NSInteger>(scope);
                return (NSInteger)Math.Abs((decimal)ret.First());
            }

            public static NSObject Eq(NSExpression[] args, NSScope scope)
            {
                (args.Length == 2).OrThrows("expect 2 parameters, get " + args.Length);
                var nums = args.Select(o => o.Evaluate(scope)).Cast<NSInteger>().ToArray();
                return nums[0] == nums[1];
            }
            
            public static NSObject Set(NSExpression[] args, NSScope scope)
            {
                (args.Length == 2).OrThrows("expect two parameters");
                var b0 = args[0];
                var b1 = args[1].Evaluate(scope);
                scope.Undefine(b0.Value);
                scope.Define(b0.Value, b1);
                return NSBool.True;
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
                (args.Length == 2).OrThrows("expect one or more parameter");
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

            public static NSObject Cons(NSExpression[] args, NSScope scope)
            {
                NSList list = null;
                NSObject atom = null;
                (args.Length == 2
                    && (atom = (args[0].Evaluate(scope) as NSObject)) != null
                    && (list = (args[1].Evaluate(scope) as NSList)) != null
                ).OrThrows("Expect atom and list, get[{0}] and [{1}]".Fmt(args[0].Evaluate(scope).GetType(), args                                                               [1].Evaluate(scope).GetType()));
                var ls = new List<NSObject>(list);
                ls.Insert(0, atom);
                return new NSList(ls);

            }

            public static NSObject Append(NSExpression[] args, NSScope scope)
            {
                NSList list0 = null, list1 = null;
                (args.Length == 2
                    && (list0 = (args[0].Evaluate(scope) as NSList)) != null
                    && (list1 = (args[1].Evaluate(scope) as NSList)) != null
                ).OrThrows("Expect two lists, get[{0}] and [{1}]".Fmt(args[0].Evaluate(scope).GetType(),                                                                args[1].Evaluate(scope).GetType()));
                return new NSList(list0.Concat(list1));
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
                    && (newval = (args[2]).Evaluate(scope) as NSObject) != null
                ).OrThrows("Expect a list, an integer and a new NSObject, get [{0}], [{1}] and [{2}]"
                            .Fmt(args[0].Evaluate(scope).GetType(), 
                                                args[1].Evaluate(scope).GetType(),
                                                args[2].Evaluate(scope).GetType()
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
