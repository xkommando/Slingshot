using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.compiler;

namespace Slingshot
{
    namespace compiler
    {
        public class SSDict : SSObject
        {

            public Dictionary<SSObject, SSObject> Val { get; private set; }
            public SSDict(Dictionary<SSObject, SSObject> d)
            {
                this.Val = d;
            }

            public override bool Equals(object obj)
            {
                return obj is SSDict ? Val.Equals(((SSDict)obj).Val) : false;
            }
            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public override object Clone()
            {
                return new SSDict(Val.ToDictionary(e=>e.Key, e=>e.Value));
            }
        }
    }

}
