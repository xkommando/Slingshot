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
    public static class InterEx
    {

        public static NSScope LoadLib(this NSScope scope, string lib)
        {
            using (var sr = new StreamReader(lib))
            {
                string code;
                if (null != (code = sr.ReadToEnd()))
                {
                    try
                    {
                        code.ParseSequence().Evaluate(scope).ForEach(a => Console.WriteLine(a));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to Load library[{0}]".Fmt(lib));
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
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
                    Console.Write("NS >> ");
                    String code;
                    if (!String.IsNullOrWhiteSpace(code = Console.ReadLine()))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("NS >> " + evaluate(code, scope));
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("NS >> " + ex.Message);
                    Console.WriteLine(">>Source " + ex.Source);
                    Console.WriteLine(">>Trace " + ex.StackTrace);
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


        public static NSBool ChainRelation(this NSExpression[] expressions, NSScope scope, Func<NSInteger, NSInteger, Boolean> relation)
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

        public static List<NSExpression> ParseSequence(this String code)
        {
            var program = new NSExpression(null, null);
            var current = program;
            var file = new CodeFile();
            file.SourceCode = code;
            file.Parse();
            (file.ErrorList.Count < 1).OrThrows(file.ErrrorStr());

            List<CodeToken> toks = file.TokenList;

            foreach (var lex in file.TokenList)
            {
                if (lex.Type == TokenType.LeftParentheses)
                {
                    var newNode = new NSExpression(tok: lex, parent: current);
                    current.Children.Add(newNode);
                    current = newNode;
                }
                else if (lex.Type == TokenType.RightParenthese)
                {
                    current = current.Parent;
                }
                else
                {
                    current.Children.Add(new NSExpression(tok: lex, parent: current));
                }
            }
            return program.Children;
        }

        public static NSExpression ParseAsIScheme(this String code)
        {
            return ParseSequence(code)[0];
        }
    }
}
