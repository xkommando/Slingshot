using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slingshot
{
    namespace Objects
    {
        public class SSString : SSObject
        {
            public string Val  { get; private set; }
            public SSString(string v)
            {
                this.Val = v;
            }

            public override object Clone()
            {
                return new SSString(Val.ToString());
            }

            public override string ToString()
            {
                return "\"" + Val + "\"";
            }

            public override bool Equals(object other)
            {
                return other is SSString ?
                                this.Val.Equals(((SSString)other).Val)
                                : false;
            }
            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }
            public static implicit operator String(SSString value)
            {
                return value.Val;
            }

            public static implicit operator SSString(String v)
            {
                return new SSString(v);
            }

        }
    }
}
