using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slingshot
{
    public interface IIterable<T>
    {
        Iterator<T> GetIterator();
    }


    /// <summary>
    /// Warning!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Iterator<T> : IEnumerator<T>
    {
        private List<T> val_;
        private int index_;

        //private 
        public Iterator(List<T> ls)
        {
            this.val_ = ls;
            index_ = 0;
        }

        public bool Forth
        {
            get {  return --index_ >= 0; }
        }

        public T GetAndForth
        {
            get
            {
                int idx = index_;
                index_++;
                //return idx < val_.Count ? val_[idx] : null;
                return val_[idx];
            }
        }
        public T ForthAndGet
        {
            get {return val_[++index_]; }
        }

        public bool Back
        {
            get { return --index_ >= 0;  }
        }

        public T GetAndBack 
        {
            get
            {
                int idx = index_;
                index_--;
                //return idx < val_.Count ? val_[idx] : null;
                return val_[idx];
            }
        }

        public T BackAndGet
        {
            get { return val_[--index_]; }
        }

        public bool Valid
        {
            get { return -1 < index_ && index_ < val_.Count; }
        }


        public bool MoveNext()
        {
            return Forth;
        }

        public void Reset()
        {
            index_ = 0;
        }

        public void Dispose()
        {
            index_ = -1;
            val_ = null;
        }

        T IEnumerator<T>.Current
        {
            get { return val_[index_]; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return val_[index_]; }
        }
    }
}
