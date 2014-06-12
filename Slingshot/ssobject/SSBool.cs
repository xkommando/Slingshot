using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slingshot
{
    namespace Objects
    {
        public class SSBool : SSNumber
        {
            public static readonly SSBool False = new SSBool();
            public static readonly SSBool True = new SSBool();

            public override object Clone()
            {
                return this ? True : False;
            }

            public override String ToString()
            {
                return this ? "SSTrue" : "SSFalse";
            }

            public override int GetHashCode()
            {
                return this ? 1 : 0;
            }

            public override bool Eq(SSObject obj)
            {
                return this == obj;
            }

            public override long IntVal()
            {
                return this ? 1 : 0;
            }

            public override double FloatVal()
            {
                return this ? 1.0 : 0.0;
            }
            public static implicit operator Boolean(SSBool value)
            {
                return value == SSBool.True;
            }

            public static implicit operator SSInteger(SSBool value)
            {
                return value ? 1 : 0;
            }

            public static implicit operator SSBool(Boolean value)
            {
                return value ? True : False;
            }


            public override bool Replace(SSObject other)
            {
                return false;
            }
        }


    }
}
