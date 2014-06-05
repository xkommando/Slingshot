using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NScheme.compiler;

namespace NScheme
{
    /// <summary>
    /// extension functions for the interpretor
    /// </summary>
    public static class InterExtension
    {

        public static NSScope LoadLib(this NSScope scope, string lib)
        {
            scope.StdOut.WriteLine(">>> Loading Library [{0}] ...".Fmt(lib));
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

                        scope.variableTable.ForEach(a => scope.StdOut.WriteLine(">>> Added {0} : {1} "
                            .Fmt( a.Key, a.Value.GetType())));
                    }//
                    catch (Exception ex)
                    {
                        scope.StdOut.WriteLine("Failed to Load library[{0}]".Fmt(lib));
                        scope.StdOut.WriteLine(ex);
                        ex.StackTrace.Split('\n').Take(3).ForEach(a => scope.StdOut.WriteLine(a));
                    }
                }
            }
            return scope;
        }

        public static void InterpretingInConsole(this NSScope scope,
                                                    Func<String, NSScope, NSObject> evaluate)
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
                    ex.StackTrace.Split("at".ToCharArray()).Take(3).ForEach(a => scope.StdOut.WriteLine(a));
                }
            }
        }

        public static IEnumerable<T> Evaluate<T>(this IEnumerable<NSExpression> expressions, NSScope scope)
        where T : NSObject
        {
            return expressions.Evaluate(scope).Cast<T>();
        }

        public static IEnumerable<NSObject> Evaluate(this IEnumerable<NSExpression> expressions, NSScope scope)
        {
            return expressions.Select(exp => exp.Evaluate(scope));
        }


        public static NSBool ChainRelation(this NSExpression[] expressions, 
                                            NSScope scope, Func<NSInteger, NSInteger, Boolean> relation)
        {
            (expressions.Length > 1).OrThrows("Must have more than 1 parameter in relation operation.");
            NSInteger current = (NSInteger)expressions[0].Evaluate(scope);
            foreach (var obj in expressions.Skip(1))
            {
                var next = (NSInteger)obj.Evaluate(scope);
                if (relation(current, next))
                {
                    current = next;
                }
                else
                {
                    return NSBool.NSFalse;
                }
            }
            return NSBool.NSTrue;
        }

        public static NSList RetrieveSList(this NSExpression[] expressions, NSScope scope, String operationName)
        {
            NSList list = null;
            (expressions.Length == 1 && (list = (expressions[0].Evaluate(scope) as NSList)) != null)
                .OrThrows("[" + operationName + "] must apply to a list");

            return list;
        }

        public static String[] Tokenize(String text)
        {
            String[] tokens = text.Replace("(", " ( ").Replace(")", " ) ").Split(" \t\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            return tokens;
        }

        public static List<NSExpression> ParseExpSeq(this String code)
        {
            var file = new CodeFile();
            file.SourceCode = code;
            file.Parse();
            (file.ErrorList.Count < 1).OrThrows(file.ErrrorStr());

            var program = new NSExpression(null, null);
            var current = program;

            foreach (var lex in file.TokenList)
            {
                //if (lex.Type == TokenType.Char)
                    //Console.WriteLine(lex.Type + "      [" + lex.Value+"]");

                if (lex.Type == TokenType.LeftParentheses)
                {
                    var newNode = new NSExpression(tok: lex, parent: current);
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
                    current.Children.Add(new NSExpression(tok: lex, parent: current));
                }
            }
            return program.Children;
        }

        public static NSExpression ParseAsIScheme(this String code)
        {
            return ParseExpSeq(code)[0];
        }
    }
}
