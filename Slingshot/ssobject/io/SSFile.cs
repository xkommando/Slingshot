using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Slingshot.Compiler;

namespace Slingshot.Objects
{

        public class SSFile : SSObject
        {
            public string Val { get; private set; }
            private byte[] cache;
            private DateTime lastWrite;
            public static readonly int Threshold = 1 << 20;

            public SSFile(string v)
            {
                this.Val = v;
            }

            public override bool Eq(SSObject obj)
            {
                return obj is SSFile && (obj as SSFile).Val.Equals(Val);
            }

            public override bool Replace(SSObject other)
            {
                if (other is SSFile)
                {
                    this.Val = (other as SSFile).Val;
                    return true;
                }
                return false;
            }

            public string ReadStr()
            {
                return File.ReadAllText(Val);
                //if (lastWrite != null)
                //var fi = new FileInfo(Val);
                //DateTime dt = fi.LastWriteTime;
                //string s = null;
                //using (StreamReader sr = new StreamReader(Val, Encoding.UTF8, true, 2048, true))
                //    s = sr.ReadToEnd();
                // if (s.Length)
            }

            public void WriteStr(string s)
            {
                using (var sw = new StreamWriter(Val))
                    sw.Write(s);
            }

            public override int GetHashCode()
            {
                return Val.GetHashCode();
            }

            public override string ToString()
            {
                return "SSFile[{0}]".Fmt(Val);
            }

            public override object Clone()
            {
                throw new NotImplementedException();
            }

            public static implicit operator string(SSFile f)
            {
                return f.Val;
            }

            public static implicit operator SSFile(string s)
            {
                return new SSFile(s);
            }
        }
}
