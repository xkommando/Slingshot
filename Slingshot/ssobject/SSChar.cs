﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace Objects
    {
        public class SSChar : SSNumber
        {
            public char Val { get; private set; }

            public SSChar(char c)
            {
                Val = c;
            }

            public override String ToString()
            {
                return "'" + Val + "'";
            }

            public override bool Eq(SSObject other)
            {
                var c = other as SSChar;
                return c != null && Val == c.Val;
            }

            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public static implicit operator char(SSChar value)
            {
                return value.Val;
            }

            public static implicit operator SSChar(char v)
            {
                return new SSChar(v);
            }


            public static implicit operator SSInteger(SSChar value)
            {
                return new SSInteger(value.Val);
            }

            public override object Clone()
            {
                return new SSChar(Val);
            }

            public override long IntVal()
            {
                return Val;
            }

            public override double FloatVal()
            {
                return Val;
            }

            public override bool Replace(SSObject other)
            {
                if (other is SSChar)
                {
                    this.Val = ((SSChar) other).Val;
                    return true;
                }
                return false;
            }
        }
    }

}
