using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slingshot
{
    namespace Objects
    {
        public class SSBool : SSObject, ISSNumber
        {
            public static readonly SSBool NSFalse = new SSBool();
            public static readonly SSBool NSTrue = new SSBool();

            public override object Clone()
            {
                return this ? NSTrue : NSFalse;
            }

            public override String ToString()
            {
                return this ? "SSTrue" : "SSFalse";
            }

            public override int GetHashCode()
            {
                return this ? 1 : 0;
            }

            public override bool Equals(object obj)
            {
                return this == obj;
            }

            public long IntVal()
            {
                return this ? 1 : 0;
            }

            public double FloatVal()
            {
                return this ? 1.0 : 0.0;
            }
            public static implicit operator Boolean(SSBool value)
            {
                return value == SSBool.NSTrue;
            }

            public static implicit operator SSInteger(SSBool value)
            {
                return value ? 1 : 0;
            }

            public static implicit operator SSBool(Boolean value)
            {
                return value ? NSTrue : NSFalse;
            }
  
        }


    }
}
