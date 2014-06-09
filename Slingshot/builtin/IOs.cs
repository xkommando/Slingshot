using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slingshot.Compiler;
using Slingshot.Objects;
using Slingshot.Interpretor;

namespace Slingshot
{
    namespace BuiltIn
    {

        partial struct Functions
        {

            public struct IO
            {


                public static SSObject OutStream(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }

                public static SSObject InStream(SSExpression[] exps, SSScope scope)
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
                public static SSObject IOFile(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }

                public static SSObject LoadAndEval(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }
            }
        }

    }
}
