using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slingshot
{
    namespace Objects
    {
        public abstract class SSNumber : SSObject
        {
            public abstract Int64 IntVal();
            public abstract double FloatVal();
        }

        public class SSInteger : SSNumber
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
                return Val.ToString();
            }

            public override bool Eq(SSObject other)
            {
                return other is SSInteger && this.Val == ((SSInteger)other).Val;
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

            public override long IntVal()
            {
                return Val;
            }

            public override double FloatVal()
            {
                return Val;
            }
        }

        public class SSFloat : SSNumber
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
                return Val.ToString();
            }
            public override bool Eq(SSObject other)
            {
                return other is SSFloat && Math.Abs(this.Val - ((SSFloat)other).Val) < 0.0000001;
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

            public override long IntVal()
            {
                return (long)Val;
            }

            public override double FloatVal()
            {
                return Val;
            }
        }
    }

}
