using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScheme.compiler;

namespace NScheme
{
    namespace compiler
    {
        public abstract class NSObject
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

            public static implicit operator NSObject(String value)
            {
                return (NSString)value;
            }

            public abstract bool Eq(NSObject other);
            public abstract NSObject Clone();
        }

    }
}