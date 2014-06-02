using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScheme
{
    public class NSFunction : NSObject
    {
        public NSExpression Body { get; private set; }
        public String[] Parameters { get; private set; }
        public NSScope Scope { get; private set; }
        public Boolean IsPartial
        {
            get
            {
                return this.ComputeFilledParameters().Length.InBetween(1, this.Parameters.Length);
            }
        }

        public NSFunction(NSExpression body, String[] parameters, NSScope scope)
        {
            this.Body = body;
            this.Parameters = parameters;
            this.Scope = scope;
        }

        public NSObject Evaluate()
        {
            String[] filledParameters = this.ComputeFilledParameters();
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
                    if ((value = this.Scope.FindInTop(p)) != null)
                    {
                        return p + ":" + value;
                    }
                    return p;
                })), this.Body);
        }

        private String[] ComputeFilledParameters()
        {
            return this.Parameters.Where(p => Scope.FindInTop(p) != null).ToArray();
        }

        public NSFunction Update(NSObject[] arguments)
        {
            var existingArguments = this.Parameters.Select(p => this.Scope.FindInTop(p)).Where(obj => obj != null);
            var newArguments = existingArguments.Concat(arguments).ToArray();
            NSScope newScope = this.Scope.Parent.SpawnScopeWith(this.Parameters, newArguments);
            return new NSFunction(this.Body, this.Parameters, newScope);
        }
    }
}
