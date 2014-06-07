using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Slingshot.Compiler;
using Slingshot.Objects;

namespace Slingshot
{
    /// <summary>
    /// extension functions for the interpretor
    /// </summary>
    public static class InterExtension
    {

        public static SSScope LoadLib(this SSScope scope, string lib)
        {
            scope.Output.WriteLine(">>> Loading Library [{0}] ...".Fmt(lib));
            using (var sr = new StreamReader(lib))
            {
                string code;
                if (null != (code = sr.ReadToEnd()))
                {
                    try
                    {
                        // prevent lazy eval
                        foreach (var exp in code.ParseExpSeq())
                            exp.Evaluate(scope);
                    }//
                    catch (Exception ex)
                    {
                        scope.Output.WriteLine("Failed to Load library[{0}]".Fmt(lib));
                        scope.Output.WriteLine(ex);
                        ex.StackTrace.Split('\r').Take(3).ForEach(a => scope.Output.WriteLine(a));
                    }
                    scope.variableTable.ForEach(a => scope.Output.WriteLine(">>> Added {0} : {1} "
                        .Fmt(a.Key, a.Value.GetType())));
                }
            }
            return scope;
        }

        public static void InterpretingInConsole(this SSScope scope,
                                                    Func<string, SSScope, SSObject> evaluate)
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(">>> ");
                    String code;
                    if (!String.IsNullOrWhiteSpace(code = Console.ReadLine()))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(">>> " + evaluate(code, scope));
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(">>> " + ex);
                    ex.StackTrace.Split("at".ToCharArray()).Take(3).ForEach(a => scope.Output.WriteLine(a));
                }
            }
        }

        public static IEnumerable<T> Evaluate<T>(this IEnumerable<SSExpression> expressions, SSScope scope)
        where T : SSObject
        {
            return expressions.Evaluate(scope).Cast<T>();
        }

        public static IEnumerable<SSObject> Evaluate(this IEnumerable<SSExpression> expressions, SSScope scope)
        {
            return expressions.Select(exp => exp.Evaluate(scope));
        }


        public static SSBool ChainRelation(this SSExpression[] expressions,
                                            SSScope scope, Func<ISSNumber, ISSNumber, Boolean> relation)
        {
            (expressions.Length > 1).OrThrows("Must have more than 1 parameter in relation operation.");
            var current = (ISSNumber)expressions[0].Evaluate(scope);
            foreach (var obj in expressions.Skip(1))
            {
                var next = (ISSNumber)obj.Evaluate(scope);
                if (relation(current, next))
                {
                    current = next;
                }
                else
                {
                    return SSBool.NSFalse;
                }
            }
            return SSBool.NSTrue;
        }

        public static SSList RetrieveSList(this SSExpression[] expressions, SSScope scope, String operationName)
        {
            SSList list = null;
            (expressions.Length == 1 && (list = (expressions[0].Evaluate(scope) as SSList)) != null)
                .OrThrows("[" + operationName + "] must apply to a list");

            return list;
        }

        public static String[] Tokenize(String text)
        {
            String[] tokens = text.Replace("(", " ( ").Replace(")", " ) ").Split(" \t\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            return tokens;
        }

        public static List<SSExpression> ParseExpSeq(this String code)
        {
            var file = new CodeFile();
            file.SourceCode = code;
            file.Parse();
            (file.ErrorList.Count < 1).OrThrows(file.ErrrorStr());

            var program = new SSExpression(null, null);
            var current = program;

            foreach (var lex in file.TokenList)
            {
                //if (lex.Type == TokenType.Char)
                    //Console.WriteLine(lex.Type + "      [" + lex.Value+"]");

                if (lex.Type == TokenType.LeftParentheses)
                {
                    var newNode = new SSExpression(tok: lex, parent: current);
                    current.Children.Add(newNode);
                    //Console.WriteLine("current.Children.Add(newNode); (    child count " + current.Children.Count);
                    current = newNode;
                }
                else if (lex.Type == TokenType.RightParenthese)
                {
                    current = current.Parent;
                }
                else
                {
                  //  Console.WriteLine("add " + lex.Value);
                    current.Children.Add(new SSExpression(tok: lex, parent: current));
                }
            }
            return program.Children;
        }

        public static SSExpression ParseAsIScheme(this String code)
        {
            return ParseExpSeq(code)[0];
        }
    }
}
