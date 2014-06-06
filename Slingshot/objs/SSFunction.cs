using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slingshot
{
    namespace compiler
    {
        public class SSFunction : SSObject
        {
            public SSExpression Body { get; private set; }
            public CodeToken[] Parameters { get; private set; }
            public SSScope Scope { get; private set; }
            public bool IsPartial()
            {
                return this.ComputeFilledParameters().Length.InBetween(1, this.Parameters.Length);
            }
            /// <summary>
            /// functions are immutable
            /// </summary>
            /// <returns></returns>
            public override object Clone()
            {
                return this;
            }

            public SSFunction(SSExpression body, CodeToken[] parameters, SSScope scope)
            {
                this.Body = body;
                this.Parameters = parameters;
                this.Scope = scope;
            }

            public SSObject Evaluate()
            {
                CodeToken[] filledParameters = this.ComputeFilledParameters();
                if (filledParameters.Length < Parameters.Length)
                {
                    return this;
                }
                else
                {
                    return this.Body.Evaluate(this.Scope);
                }
            }

            public override String ToString()
            {
                return String.Format("(func ({0}) {1})",
                    " ".Join(this.Parameters.Select(p =>
                    {
                        SSObject value = null;
                        if ((value = this.Scope.FindInTop(p.Value)) != null)
                        {
                            return p + ":" + value;
                        }
                        return p.Value;
                    })), this.Body);
            }

            private CodeToken[] ComputeFilledParameters()
            {
                return this.Parameters.Where(p => Scope.FindInTop(p.Value) != null).ToArray();
            }

            public SSFunction Update(SSObject[] arguments)
            {
                var existingArguments = 
                    this.Parameters.Select(p => this.Scope.FindInTop(p.Value))
                                    .Where(obj => obj != null);

                var newArguments = existingArguments.Concat(arguments).ToArray();
                SSScope newScope = this.Scope.Parent.SpawnScopeWith(this.Parameters, newArguments);
                return new SSFunction(this.Body, this.Parameters, newScope);
            }

        }
    }
}