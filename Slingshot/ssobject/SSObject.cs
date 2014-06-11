using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Objects
    {
        public abstract class SSObject : ICloneable
        {
            public static implicit operator SSObject(Int64 value)
            {
                return (SSInteger)value;
            }

            public static implicit operator SSObject(double value)
            {
                return (SSFloat)value;
            }

            public static implicit operator SSObject(Boolean value)
            {
                return (SSBool)value;
            }

            public static implicit operator SSObject(String value)
            {
                return (SSString)value;
            }

            public static implicit operator SSObject(char value)
            {
                return (SSChar)value;
            }

            public abstract bool Eq(SSObject obj);
            public abstract bool Replace(SSObject other);
            public abstract object Clone();
        }

    }
}