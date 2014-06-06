using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slingshot
{
    namespace Objects
    {
        public interface ISSNumber
        {
            Int64 IntVal();
            double FloatVal();
        }

        public class SSInteger : SSObject, ISSNumber
        {
            public Int64 Val { get; private set; }

            public SSInteger(Int64 valInt)
            {
                this.Val = valInt;
            }

            public override object Clone()
            {
                return new SSInteger(Val);
            }

            public override String ToString()
            {
                return "SSInteger[" + Val.ToString() + "]";
            }

            public override bool Equals(object other)
            {
                return other is SSInteger ?
                                this.Val == ((SSInteger)other).Val
                                : false;
            }

            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }
            public static implicit operator Int64(SSInteger number)
            {
                return number.Val;
            }

            public static implicit operator double(SSInteger number)
            {
                return (double)number.Val;
            }

            public static implicit operator SSInteger(Int64 value)
            {
                return new SSInteger(value);
            }
            /// <summary>
            /// note that int can be float while float cannot be int
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static implicit operator SSInteger(double value)
            {
                return new SSInteger((Int64)value);
            }

            public long IntVal()
            {
                return Val;
            }

            public double FloatVal()
            {
                return Val;
            }
        }

        public class SSFloat : SSObject, ISSNumber
        {
            public double Val { get; private set; }
            public SSFloat(double db)
            {
                this.Val = db;
            }
            public override object Clone()
            {
                return new SSFloat(Val);
            }

            public override String ToString()
            {
                return "SSFloat[" +  Val.ToString() + "]";
            }
            public override bool Equals(object other)
            {
                return other is SSFloat ?
                                this.Val == ((SSFloat)other).Val
                                : false;
            }
            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public static implicit operator double(SSFloat number)
            {
                return number.Val;
            }

            public static implicit operator SSFloat(Int64 value)
            {
                return new SSFloat(value);
            }
            public static implicit operator SSFloat(double value)
            {
                return new SSFloat(value);
            }

            /// <summary>
            /// note that int can be float while float cannot be int
            /// </summary>
            public static implicit operator SSFloat(SSInteger value)
            {
                return new SSFloat(value.Val);
            }

            public long IntVal()
            {
                return (long)Val;
            }

            public double FloatVal()
            {
                return Val;
            }
        }
    }

}
