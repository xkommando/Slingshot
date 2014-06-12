using System;
using System.Collections.Generic;
using System.Linq;

namespace Slingshot.Objects
{
    public class SSSignal : SSObject
    {
        public static readonly SSSignal Break = new SSSignal();
        public static readonly SSSignal Continue = new SSSignal();

        public override bool Eq(SSObject obj)
        {
            return obj is SSSignal && (obj as SSSignal) == this;
        }

        public override bool Replace(SSObject other)
        {
            return false;
        }

        public override object Clone()
        {
            return this;
        }
    }
}
