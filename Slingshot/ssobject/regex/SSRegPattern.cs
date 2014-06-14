using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Slingshot.Objects
{
    public class SSRegPattern : SSObject
    {

        public Regex Val { get; private set; }

        public SSRegPattern(Regex reg)
        {
            this.Val = reg;
        }

        public SSRegPattern(SSString str)
        {
            this.Val = new Regex(str.Val);
        }

        public override bool Eq(SSObject obj)
        {
            return obj is SSRegPattern && ((SSRegPattern) obj).Val.Equals(Val);
        }

        public override bool Replace(SSObject other)
        {
            this.Val = (other as SSRegPattern).Val;
            return true;
        }

        public override object Clone()
        {
            return new SSRegPattern(Val.ToString());
        }
    }
}
