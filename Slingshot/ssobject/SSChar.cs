using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Objects
    {
        public class SSChar: SSObject, ISSNumber
        {
            public char Val { get; private set; }

            public SSChar(char c)
            {
                Val = c;
            }
            public override String ToString()
            {
                return "'" + Val + "'";
            }

            public override bool Equals(object other)
            {
                return other is SSChar ? Val == ((SSChar)other).Val : false;
            }

            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public static implicit operator char(SSChar value)
            {
                return value.Val;
            }

            public static implicit operator SSChar(char v)
            {
                return new SSChar(v);
            }


            public static implicit operator SSInteger(SSChar value)
            {
                return new SSInteger(value.Val);
            }

            public override object Clone()
            {
                return new SSChar(Val);
            }

            public long IntVal()
            {
                return Val;
            }

            public double FloatVal()
            {
                return Val;
            }
        }
    }

}
