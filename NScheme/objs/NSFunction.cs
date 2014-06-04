using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    namespace compiler
    {
        public class NSFunction : NSObject
        {
            public NSExpression Body { get; private set; }
            public CodeToken[] Parameters { get; private set; }
            public NSScope Scope { get; private set; }
            public Boolean IsPartial
            {
                get
                {
                    return this.ComputeFilledParameters().Length.InBetween(1, this.Parameters.Length);
                }
            }
            /// <summary>
            /// functions are immutable
            /// </summary>
            /// <returns></returns>
            public override NSObject Clone()
            {
                return this;
            }

            public NSFunction(NSExpression body, CodeToken[] parameters, NSScope scope)
            {
                this.Body = body;
                this.Parameters = parameters;
                this.Scope = scope;
            }

            public NSObject Evaluate()
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
                        NSObject value = null;
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

            public NSFunction Update(NSObject[] arguments)
            {
                var existingArguments = 
                    this.Parameters.Select(p => this.Scope.FindInTop(p.Value))
                                    .Where(obj => obj != null);

                var newArguments = existingArguments.Concat(arguments).ToArray();
                NSScope newScope = this.Scope.Parent.SpawnScopeWith(this.Parameters, newArguments);
                return new NSFunction(this.Body, this.Parameters, newScope);
            }
        }
    }
}