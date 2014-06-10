using System.Linq;
using Slingshot.Objects;
using Slingshot.Compiler;

namespace Slingshot
{
    namespace BuiltIn
    {
        public partial struct Functions
        {
            public struct Dicts
            {

                public static SSObject KeyList(SSExpression[] exps, SSScope scope)
                {
                    var dict = exps[0].Evaluate(scope) as SSDict;
                    return new SSList(dict.Val.Keys.ToList());
                }

                public static SSObject KeySet(SSExpression[] exps, SSScope scope)
                {
                    var dict = exps[0].Evaluate(scope) as SSDict;
                    return null;
                }

                public static SSObject ValueList(SSExpression[] exps, SSScope scope)
                {
                    var dict = exps[0].Evaluate(scope) as SSDict;
                    return new SSList(dict.Val.Values.ToList());
                }
                
                public static SSObject ValueSet(SSExpression[] exps, SSScope scope)
                {
                    return null;
                }
            }
        }
    }
}
