using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Objects
    {
        public class SSDict : SSObject
        {
            public Dictionary<SSObject, SSObject> Val { get; private set; }
            public SSDict(Dictionary<SSObject, SSObject> d)
            {
                this.Val = d;
            }

            public override bool Eq(SSObject obj)
            {
                var dict = obj as SSDict;
                return dict != null && Val.Equals(dict.Val);
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
