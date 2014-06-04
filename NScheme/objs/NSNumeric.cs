using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScheme.compiler;

namespace NScheme
{
    namespace compiler
    {
        public class NSBool : NSObject
        {
            public static readonly NSBool NSFalse = new NSBool();
            public static readonly NSBool NSTrue = new NSBool();

            public override NSObject Clone()
            {
                return this ? NSTrue : NSFalse;
            }

            public override String ToString()
            {
                return this ? "NSTrue" : "NSFalse";
            }

            public static implicit operator Boolean(NSBool value)
            {
                return value == NSBool.NSTrue;
            }

            public static implicit operator NSBool(Boolean value)
            {
                return value ? NSTrue : NSFalse;
            }
        }

        public class NSInteger : NSObject
        {
            public readonly Int64 val;

            public NSInteger(Int64 valInt)
            {
                this.val = valInt;
            }

            public override NSObject Clone()
            {
                return new NSInteger(val);
            }
            public override String ToString()
            {
                return val.ToString();
            }

            public static implicit operator Int64(NSInteger number)
            {
                return number.val;
            }

            public static implicit operator double(NSInteger number)
            {
                return (double)number.val;
            }

            public static implicit operator NSInteger(Int64 value)
            {
                return new NSInteger(value);
            }
            public static implicit operator NSInteger(double value)
            {
                return new NSInteger((Int64)value);
            }
            //public static implicit operator NSInteger(NSFloat value)
            //{
            //    return new NSInteger((Int64)value.val);
            //}
        }

        public class NSFloat : NSObject
        {
            public readonly double val;
            public NSFloat(double db)
            {
                this.val = db;
            }
            public override NSObject Clone()
            {
                return new NSFloat(val);
            }

            public override String ToString()
            {
                return val.ToString();
            }
            public static implicit operator double(NSFloat number)
            {
                return number.val;
            }

            public static implicit operator NSFloat(Int64 value)
            {
                return new NSFloat(value);
            }
            public static implicit operator NSFloat(double value)
            {
                return new NSFloat(value);
            }
            public static implicit operator NSFloat(NSInteger value)
            {
                return new NSFloat(value.val);
            }

        }
    }
}
