using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Slingshot.Compiler;
using Slingshot.Objects;

namespace Slingshot
{
    namespace Interpretor
    {

        /// <summary>
        /// extension functions for the interpretor
        /// </summary>
        public static class InterpretorAux
        {
            public static SSScope LoadLib(this SSScope scope, string lib)
            {
                scope.Output.WriteLine(">>> Loading Library [{0}] ...".Fmt(lib));
                using (var sr = new StreamReader(lib))
                {
                    string code = null;
                    if ((code = sr.ReadToEnd()).NotEmpty())
                    {
                        scope.BulkEval(code);
                    }
                }
                return scope;
            }

            public static void BulkEval(this SSScope scope, string src)
            {
                if (src.NotEmpty())
                {
                    var file = new CodeFile();
                    var syntax = new SyntaxAnalyzer();
                    file.SourceCode = src;
                    syntax.Take(file);
                    if (syntax.ErrorList.Count > 0)
                    {
                        scope.Output.WriteLine("Errors:");
                        syntax.ErrorList.ForEach(a => scope.Output.WriteLine(a));
                    }
                    try
                    {
                        // prevent lazy eval
                        foreach (var exp in syntax.Expressions)
                            exp.Evaluate(scope);
                    }//
                    catch (Exception ex)
                    {
                        scope.Output.WriteLine("Failed to evaluate");
                        scope.Output.WriteLine(ex);
                        ex.StackTrace.Split('\r').Take(3).ForEach(a => scope.Output.WriteLine(a));
                    }
                    //    scope.VariableTable.ForEach(a => scope.Output.WriteLine(">>> Added {0} : {1} "
                    //       .Fmt(a.Key, a.Value.GetType())));
                }
            }

            public static SSObject Cmd(this SSScope scope, List<SSExpression> exps)
            {
                return true;
            }
            public static void InterpretingInConsole(this SSScope scope)
            {
                var w = new Stopwatch();
                var file = new CodeFile();
                var syntax = new SyntaxAnalyzer();
                while (true)
                {
                    file.ReSet();
                    syntax.ReSet();
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(">>> ");
                        String code;
                        if ((code = Console.ReadLine()).NotEmpty())
                        {
                            file.SourceCode = code;
                            file.Parse();
                            syntax.Take(file);
                            Console.ForegroundColor = ConsoleColor.Green;
                            
                            w.Reset();
                            w.Start();
                            Console.WriteLine(">>> " + syntax.Expressions.Last().Evaluate(scope));
                            w.Stop();
                            Console.WriteLine(w.ElapsedMilliseconds + "ms");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                       // ex.StackTrace.Split("at".ToCharArray()).Take(3).ForEach(a => scope.Output.WriteLine(a));
                    }
                }
            }

            public static SSBool ChainRelation(this SSExpression[] expressions,
                                                SSScope scope, Func<SSNumber, SSNumber, Boolean> relation)
            {
                (expressions.Length > 1).OrThrows("Must have more than 1 parameter in relation operation.");
                var current = (SSNumber)expressions[0].Evaluate(scope);
                foreach (var obj in expressions.Skip(1))
                {
                    var next = (SSNumber)obj.Evaluate(scope);
                    if (relation(current, next))
                    {
                        current = next;
                    }
                    else
                    {
                        return SSBool.False;
                    }
                }
                return SSBool.True;
            }

            public static SSList RetrieveSList(this SSExpression[] expressions, SSScope scope, String operationName)
            {
                SSList list = null;
                (expressions.Length == 1 && (list = (expressions[0].Evaluate(scope) as SSList)) != null)
                    .OrThrows("[" + operationName + "] must apply to a list");

                return list;
            }

            //public static String[] Tokenize(String text)
            //{
            //    String[] tokens = text.Replace("(", " ( ").Replace(")", " ) ").Split(" \t\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            //    return tokens;
            //}
            public static void Print(this SSExpression exp, TextWriter writer)
            {
                var ct = 3;
                while (ct-- != 0 && exp.Parent != null)
                {
                    exp = exp.Parent;
                }
                DoPrint(exp, writer, 5);
            }

            public static void DoPrint(this SSExpression exp, TextWriter writer, int threshold)
            {
                if (threshold < 0)
                    return;

                if (exp.Token != null)
                {
                    if (exp.Token.Type == TokenType.LeftBracket
                        || exp.Token.Type == TokenType.LeftCurlyBracket
                        || exp.Token.Type == TokenType.LeftParentheses)
                        writer.WriteLine(exp.Token.Value);
                    else
                        writer.Write(exp.Token.Value + "  ");
                }
                exp.Children.ForEach(a=>a.DoPrint(writer, --threshold));
            }

        }
    }

}
