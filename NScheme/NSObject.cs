using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{

    public class NSObject
    {
        public static implicit operator NSObject(Int64 value)
        {
            return (NSInteger)value;
        }
        public static implicit operator NSObject(double value)
        {
            return (NSFloat)value;
        }
        public static implicit operator NSObject(Boolean value)
        {
            return (NSBool)value;
        }

    }

    public class NSInteger : NSObject
    {
        public readonly Int64 val;

        public NSInteger(Int64 valInt)
        {
            this.val = valInt;
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
    }

    public class NSFloat: NSObject
    {
        public readonly double val;
        public NSFloat(double db)
        {
            this.val = db;
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
    }

    public class NSBool : NSObject
    {
        public static readonly NSBool False = new NSBool();
        public static readonly NSBool True = new NSBool();
        public override String ToString()
        {
            return ((Boolean)this).ToString();
        }
        public static implicit operator Boolean(NSBool value)
        {
            return value == NSBool.True;
        }
        public static implicit operator NSBool(Boolean value)
        {
            return value ? True : False;
        }
    }

    public class NSList : NSObject, IEnumerable<NSObject>
    {
        private List<NSObject> ls;
        public NSList(IEnumerable<NSObject> values)
        {
            this.ls = values.ToList();
        }
        public NSObject this[int i]
        {
            get
            {
                return ls[i];
            }
            set
            {
                NSObject ret = ls[i];
                ls[i] = value;
            }
        }

        public NSObject PopBack()
        {
            NSObject ret = ls.Last();
            ls.RemoveAt(ls.Count - 1);
            return ret;
        }

        public NSObject PopFront()
        {
            NSObject ret = ls.First();
            ls.RemoveAt(0);
            return ret;
        }

        public override String ToString()
        {
            return "(list " + " ".Join(this.ls) + ")";
        }
        public int Length 
        {
            get { return ls.Count; }
        }

        public IEnumerator<NSObject> GetEnumerator()
        {
            return this.ls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ls.GetEnumerator();
        }


    }

}
