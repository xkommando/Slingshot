﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
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

                public static SSObject OpenFile(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one string");
                    return new SSFile((exps[0].Evaluate(scope) as SSString).Val);
                }

                public static SSObject ReadStr(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one file");
                    return (exps[0].Evaluate(scope) as SSFile).ReadStr();
                }

                public static SSObject Load(SSExpression[] exps, SSScope scope)
                {
                    (exps.Length == 1).OrThrows("expect one string");
                    scope.BulkEval(exps[0].Evaluate(scope) as SSString);
                    return "loaded";
                }

                public static SSObject ReLoad(SSExpression[] exps, SSScope scope)
                {
                    return 0;
                }

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
