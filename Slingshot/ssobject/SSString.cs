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

            public override bool Eq(SSObject other)
            {
                return other is SSString && this.Val.Equals(((SSString)other).Val);
            }
            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }
            public override bool Replace(SSObject other)
            {
                if (other is SSString)
                {
                    this.Val = ((SSString)other).Val;
                    return true;
                }
                return false;
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
