using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;
using Slingshot.Objects;

namespace Slingshot
{
    partial class Functions
    {
        public struct IO
        {

            public static SSObject OutStream(SSExpression[] args, SSScope scope)
            {
                return 0;
            }

            public static SSObject InStream(SSExpression[] args, SSScope scope)
            {
                return 0;
            }
            /// <summary>
            /// // read
            /// (file "a-path" 'r')
            /// 
            /// // read and write
            /// (file "a-path" 'r' 'w')
            /// 
            ///  create append delete...
            /// </summary>
            public static SSObject IOFile(SSExpression[] args, SSScope scope)
            {
                return 0;
            }

            public static SSObject LoadAndEval(SSExpression[] args, SSScope scope)
            {
                return 0;
            }
        }
    }
}
