using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScheme
{
    namespace compiler
    {
        public class NSString : NSObject
        {
            public string val;
            public NSString(string v)
            {
                this.val = v;
            }

            public override NSObject Clone()
            {
                return val;
            }
            public override string ToString()
            {
                return "NSString[" + val + "]";
            }

            public static implicit operator String(NSString value)
            {
                return value.val;
            }

            public static implicit operator NSString(String v)
            {
                return new NSString(v);
            }
            public override bool Eq(NSObject other)
            {
                return other is NSString ?
                                this.val.Equals(((NSString)other).val)
                                : false;
            }
    

        }
    }
}
