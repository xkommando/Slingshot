using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slingshot
{
    namespace Objects
    {
        public class SSSet : SSObject
        {

            public HashSet<SSObject> Val { get; private set; }

            public SSSet(HashSet<SSObject> s)
            {
                Val = s;
            }

            public override bool Eq(SSObject obj)
            {
                return obj is SSSet && Val.Equals(((SSSet) obj).Val);
            }

            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public override object Clone()
            {
                return new SSSet(new HashSet<SSObject>(Val));
            }

            public override bool Replace(SSObject other)
            {
                if (other is SSSet)
                {
                    this.Val = ((SSSet)other).Val;
                    return true;
                }
                return false;
            }
        }
    }
}
