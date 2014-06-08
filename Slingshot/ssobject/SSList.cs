using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Objects
    {
        public class SSList : SSObject, IEnumerable<SSObject>
        {
            public List<SSObject> Val { get; private set; }

            public SSList(IEnumerable<SSObject> values)
            {
                this.Val = values.ToList();
            }
            public SSList(List<SSObject> values)
            {
                this.Val = values;
            }
            public SSObject this[int i]
            {
                get
                {
                    return Val[i];
                }
                set
                {
                    SSObject ret = Val[i];
                    Val[i] = value;
                }
            }
            public override object Clone()
            {
                return new SSList(Val);
            }
            public SSObject PopBack()
            {
                SSObject ret = Val.Last();
                Val.RemoveAt(Val.Count - 1);
                return ret;
            }

            public SSObject PopFront()
            {
                SSObject ret = Val.First();
                Val.RemoveAt(0);
                return ret;
            }

            public override String ToString()
            {
                return "(list " + " ".Join(this.Val) + ")";
            }

            public override bool Eq(SSObject obj)
            {
                return obj is SSList && Val.SequenceEqual(((SSList)obj).Val);
            }

            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public int Length
            {
                get { return Val.Count; }
            }

            public IEnumerator<SSObject> GetEnumerator()
            {
                return this.Val.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.Val.GetEnumerator();
            }

        }
    }
}
