using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScheme.compiler;

namespace NScheme
{
    namespace compiler
    {
        public class NSChar: NSObject
        {
            char val;
            public NSChar(char c)
            {
                val = c;
            }

            public override bool Eq(NSObject other)
            {
                return other is NSChar ? val == ((NSChar)other).val : false;
            }

            public override NSObject Clone()
            {
                return new NSChar(val);
            }
            public override string ToString()
            {
                return "NSChar[" + val + "]";
            }

        }
    }

}
