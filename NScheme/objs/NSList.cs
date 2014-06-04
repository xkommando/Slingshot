using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScheme.compiler;

namespace NScheme
{
    namespace compiler
    {
        public class NSList : NSObject, IEnumerable<NSObject>
        {
            public List<NSObject> ls { get; private set; }

            public NSList(IEnumerable<NSObject> values)
            {
                this.ls = values.ToList();
            }
            public NSObject this[int i]
            {
                get
                {
                    return ls[i];
                }
                set
                {
                    NSObject ret = ls[i];
                    ls[i] = value;
                }
            }
            public override NSObject Clone()
            {
                return new NSList(ls);
            }
            public NSObject PopBack()
            {
                NSObject ret = ls.Last();
                ls.RemoveAt(ls.Count - 1);
                return ret;
            }

            public NSObject PopFront()
            {
                NSObject ret = ls.First();
                ls.RemoveAt(0);
                return ret;
            }

            public override String ToString()
            {
                return "(list " + " ".Join(this.ls) + ")";
            }

            public int Length
            {
                get { return ls.Count; }
            }

            public IEnumerator<NSObject> GetEnumerator()
            {
                return this.ls.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.ls.GetEnumerator();
            }
        }
    }
}
