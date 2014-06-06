using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Compiler;
using Slingshot.Objects;

namespace Slingshot
{
    public static partial class Extensions
    {
        #region String
        public static bool NotEmpty
            (this String s)
        {
            if (s != null)
            {
                int right = s.Length;
                int left = 0;
                while (left < right && s.ElementAt(left) <= ' ')
                {
                    left++;
                }
                while (left < right && s.ElementAt(right - 1) <= ' ')
                {
                    right--;
                }
                return left != right;
            }
            return false;
        }

        public static String Join(this String separator, IEnumerable<Object> values)
        {
            return String.Join(separator, values);
        }

        public static string Fmt(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        public static Int64 ToInt(this String s)
        {
            return Int64.Parse(s);
        }
        public static Int64 ToFloat(this String s)
        {
            return Int64.Parse(s);
        }

        #endregion String



        #region numerics

        public static bool IsDigit<T>(this T ch)
            where T : IComparable
        {
            return ch.CompareTo('0') > -1
                   && ch.CompareTo('9') < 1;
        }

        /// <summary>
        /// v1 < v < v2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Between<T>(this T v, T v1, T v2)
           where T : IComparable
        {
            return v.CompareTo(v1) == 1 && v.CompareTo(v2) == -1;
        }

        /// <summary>
        /// v1 <= v <= v2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool InBetween<T>(this T v, T v1, T v2)
            where T : IComparable
        {
            return v.CompareTo(v1) > -1 && v.CompareTo(v2) < 1;
        }

        public static void OrThrows(this Boolean condition, String message = null)
        {
            if (!condition) { throw new Exception(message ?? "NULL"); }
        }

        #endregion numerics



        #region

        /// <summary>
        /// val = v1 || val == v2 || val == v3 ....
        /// to
        /// val.In(v1, v2, v3)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }

        public static bool Contains<T>(this IEnumerable<T> source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Any(a => source.Contains(a));
        }
        /// <summary>
        /// list.AddRange(1, 2, 3);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public static void AddRange<T, S>(this ICollection<T> list, params S[] values)
            where S : T
        {
            foreach (S value in values)
                list.Add(value);
        }

        /// <summary>
        /// enumerable.ForEach(act);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <param name="mapFunction"></param>
        public static void ForEach<T>(this IEnumerable<T> @enum, Action<T> mapFunction)
        {
            foreach (var item in @enum) mapFunction(item);
        }

        #endregion

        #region general


        public static T To<T>(this object input) where T : IConvertible
        {
            var type = typeof(T).Name;

            TypeCode typecode;
            if (!Enum.TryParse(type, out typecode)) throw new ArgumentException("Could not convert!");

            return (T)Convert.ChangeType(input, typecode);
        }

        #endregion


        // for "abc".Join(...)
    }

    public interface IBidEnumerator<T> : IEnumerator<T>
    {
        bool MovePrevious();
    }

    public interface IBidEnumerable<T>
    {
        IBidEnumerator<T> GetBidEnumerator();
    }


    public class BidEnumerator<T> : IBidEnumerator<T>
    {
        private IEnumerator<T> enumerator_;
        private List<T> buffer_;
        private int index_;

        public BidEnumerator(IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            enumerator_ = enumerator;
            buffer_ = new List<T>();
            index_ = -1;
        }

        public bool MovePrevious()
        {
            if (index_ <= 0)
            {
                return false;
            }

            --index_;
            return true;
        }

        public bool MoveNext()
        {
            if (index_ < buffer_.Count - 1)
            {
                ++index_;
                return true;
            }

            if (enumerator_.MoveNext())
            {
                buffer_.Add(enumerator_.Current);
                ++index_;
                return true;
            }

            return false;
        }

        public T Current
        {
            get
            {
                if (index_ < 0 || index_ >= buffer_.Count)
                    throw new InvalidOperationException();

                return buffer_[index_];
            }
        }

        public void Reset()
        {
            enumerator_.Reset();
            buffer_.Clear();
            index_ = -1;
        }

        public void Dispose()
        {
            enumerator_.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }
    }

    //public class LinearIterater<T> : IEnumerator<T>
    //{
    //    private readonly ICollection<T> collection_;
    //    private HashSet<T> set_; 
    //    private readonly int index_;
    //    public T Current
    //    {
            
    //        get { return collection_[index_];  }
            
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    object System.Collections.IEnumerator.Current
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public bool MoveNext()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Reset()
    //    {
    //    }
    //}
}
