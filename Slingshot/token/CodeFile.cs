using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Slingshot
{

    #region lexcial
    namespace compiler
    {
        internal enum State
        {
            Start,
            InComment,
            InNote,
            InInteger,
            InFloat,
            InSciNumber, // scientific number 53E+9
            InString,
            InEscapingString,
            InIdentifier,
            Finished,
        }

        /// <summary>
        /// why put parser at Lexical.LexAnalyzer rather than Lexical.Analyzer?
        /// one LexAnalyzer obj -> one source file, all relevent variables for parsing are local, thus safe,
        /// this makes it easy to write paralle lexical analyzer, 
        /// i.e., analyzer that parse multi source file concurrently, 
        /// each source file is parsed in seperate LexAnalyzer obj and do not affect others
        /// </summary>
        class CodeFile : IEnumerable<CodeToken>, IBidEnumerable<CodeToken>
        {

            public List<CodeLine> TokenMatrix { get; private set; }

            public List<CodeToken> TokenList { get; private set; }

            public List<CodeError> ErrorList { get; private set; }

            public CodeFile()
            {
                TokenMatrix = new List<CodeLine>(384);
                TokenList = new List<CodeToken>(2048);
                ErrorList = new List<CodeError>(36);
            }

            public String SourceCode { get; set; }

            public int TokenCount { get; private set; }

            int currentIdx_ = 0; // current char index in SourceCode
            int currentLineIdx_ = 0; // current line start char index
            int tokenLeft_ = 0; // current token liter str begin index
            int lineCount_ = 1; // current line number
            State state_ = State.Start;

            private void resetState()
            {
                tokenLeft_ = currentIdx_;
                state_ = State.Start;
            }

            private void addError(String message)
            {
                var err = new CodeError(SourceCode.Substring(tokenLeft_, currentIdx_ - tokenLeft_),
                                            message);
                err.LineNumber = lineCount_;
                err.ColumeNumber = tokenLeft_ - currentLineIdx_;
                ErrorList.Add(err);
            }

            private void addToken(int tokenLen, TokenType type)
            {
                if (type == TokenType.Comment)
                {
                    return;
                }
                //Console.WriteLine("idx[" + currentIdx_ + "]  ch[" + "] tok len[" + tokenLen + "]" );
                var token = CodeToken.Build(type, SourceCode.Substring(currentIdx_ - tokenLen + 1, tokenLen));
                token.LineNumber = lineCount_;
                token.ColumeNumber = tokenLeft_ - currentLineIdx_;

                TokenList.Add(token);

                if (TokenMatrix.Count == 0 || TokenMatrix.ElementAt(TokenMatrix.Count - 1).LineNum != lineCount_)
                {
                    var lastLine = new CodeLine(lineCount_);
                    lastLine.Add(token);
                    TokenMatrix.Add(lastLine);
                    // Console.WriteLine("NEW " + lastLine.ToString());
                }
                else
                {
                    TokenMatrix.ElementAt(TokenMatrix.Count - 1).Add(token);
                    // Console.WriteLine(tokenFile_.ElementAt(tokenFile_.Count - 1).ToString());
                }
                this.TokenCount++;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            private void addToken(TokenType type)
            {
                if (type == TokenType.Comment)
                {
                    return;
                }
                addToken(currentIdx_ - tokenLeft_ + 1, type);
            }

            // add single char, remember to set state if successed
            private bool addSingle(char ch)
            {
                switch (ch)
                {
                    case '(': addToken(1, TokenType.LeftParentheses); return true; // 15 single char
                    case ')': addToken(1, TokenType.RightParenthese); return true;
                    case '[': addToken(1, TokenType.LeftBracket); return true;
                    case ']': addToken(1, TokenType.RightBracket); return true;
                    case '{': addToken(1, TokenType.LeftCurlyBracket); return true;
                    case '}': addToken(1, TokenType.RightCurlyBracket); return true;
                    // + ++
                    // - --
                    // * **
                    // / //
                    // > >= >>
                    // < <= <<
                    // | ||

                    case ',': addToken(1, TokenType.Comma); return true;
                    case ':': addToken(1, TokenType.Colon); return true;
                    case ';': addToken(1, TokenType.SemiColon); return true;
                }
                return false;
            }

            // start state
            private void parseStart(char ch)
            {
                if (ch.IsSkipChar())
                    return;
                if (addSingle(ch))
                    return;
                switch (ch)
                {
                    case '\n':
                        {
                            lineCount_++;
                            tokenLeft_ = currentIdx_ + 1;
                            currentLineIdx_ = currentIdx_ + 1;
                        }
                        break;

                    case '\"':
                        tokenLeft_ = currentIdx_ + 1;
                        state_ = State.InString;
                        break;

                    case '\'':
                        if (currentIdx_ < SourceCode.Count() - 2)
                        {
                            var s1 = SourceCode[currentIdx_ + 1];
                            var s2 = SourceCode[currentIdx_ + 2];
                            if (s2 == '\'')
                            {
                                currentIdx_++;
                                addToken(1, TokenType.Char);
                                currentIdx_++;
                            }
                            else
                                addError("Cannot add Char[" + ch + "]");
                        }
                        break;

                    case '+':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '+')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.Inc);
                            } 
                            else if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.AddAsign);
                            }
                            else
                                addToken(1, TokenType.Add);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '-':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '-')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.Dec);
                            }
                            else if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.SubAsign);
                            }
                            else if (next.IsDigit())
                            {
                                tokenLeft_ = currentIdx_;
                                state_ = State.InInteger;
                            }
                            else
                                addToken(1, TokenType.Sub);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '*':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '*')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.NPow);
                            }
                            else if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.MulAsign);
                            }
                            else
                                addToken(1, TokenType.Mul);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '/':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '/')
                            {
                                currentIdx_++;
                                this.state_ = State.InComment;
                            }
                            else if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.DivAsign);
                            }
                            else
                                addToken(1, TokenType.Div);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '%':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.ModAsign);
                            }
                            else
                                addToken(1, TokenType.Mod);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;
                    case '|':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.BitOrAsign);
                            }
                            else
                                addToken(1, TokenType.BitOr);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;
                    case '&':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.BitAndAsign);
                            }
                            else
                                addToken(1, TokenType.BitAnd);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;
                    case '^':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.BitXORAsign);
                            }
                            else
                                addToken(1, TokenType.BitXOR);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '<':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.LE);
                            } 
                            else if (next == '<')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.BitSL);
                            }
                            else
                                addToken(1, TokenType.LT);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '>':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.GE);
                            }
                            else if (next == '>')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.BitSR);
                            }
                            else
                                addToken(1, TokenType.GT);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '=':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            var next = SourceCode[currentIdx_ + 1];
                            if (next == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.EQ);
                            }
                            else
                                addToken(1, TokenType.Assign);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;

                    case '!':
                        if (currentIdx_ != SourceCode.Count() - 1)
                        {
                            if (SourceCode[currentIdx_ + 1] == '=')
                            {
                                currentIdx_++;
                                addToken(2, TokenType.NE);
                            }
                            else
                                addToken(1, TokenType.Not);
                        }
                        else
                            addError("Illegal Ending Char[" + ch + "]");
                        break;
                    case '@':
                        tokenLeft_ = currentIdx_ + 1;
                        state_ = State.InNote;
                        break;
                    default:
                        {
                            if (ch.IsDigit())
                            {
                                tokenLeft_ = currentIdx_;
                                state_ = State.InInteger;
                            }
                            else if (ch.IsValidLeadingIdCh())
                            {
                                tokenLeft_ = currentIdx_;
                                state_ = State.InIdentifier;
                            }
                            else
                            {
                                addError("Unknown Char[" + ch + "]");
                            }
                        }
                        break;
                }// switch ch
            }

            #region Parse()
            public void Parse()
            {
                (SourceCode.NotEmpty()).OrThrows("Null Source Code");

                // avoid re-parse
                if (state_ == State.Finished)
                    return;

                while (currentIdx_ < SourceCode.Length)
                {
                    var ch = SourceCode[currentIdx_];
                    switch (state_)
                    {
                        case State.Start:
                            parseStart(ch);
                            break;// start

                        case State.InComment:
                            if (ch == '\n')
                            {
                                resetState();
                                lineCount_++;
                                currentLineIdx_ = currentIdx_ + 1;
                            }
                            break;
                        case State.InNote:
                            if (ch == '\n')
                            {
                                currentIdx_--;
                                addToken(TokenType.Note);
                                currentIdx_++;
                                resetState();
                            }
                            break;
                        case State.InInteger:
                            if (currentIdx_ != SourceCode.Count() - 1)
                            {
                                var next = SourceCode[currentIdx_ + 1];
                                if (ch == '.' && next.IsDigit())
                                    state_ = State.InFloat;
                                else if (ch == 'E' || ch == 'e')
                                {
                                    state_ = State.InSciNumber;
                                }
                                else if (!ch.IsDigit())
                                {
                                    currentIdx_--;
                                    addToken(TokenType.Integer);
                                    resetState();
                                }
                            }
                            else if (ch.IsDigit())// last char
                            {
                                addToken(TokenType.Integer);
                                resetState();
                            }
                            else // last char is not digit
                            {
                                currentIdx_--;
                                addToken(TokenType.Integer);
                                currentIdx_++;
                                if (addSingle(ch))
                                    resetState();
                                else
                                    addError("Illegal Ending Char[" + ch + "] Following Integer");
                            }
                            break;

                        case State.InFloat:
                            if (currentIdx_ != SourceCode.Count() - 1)
                            {
                                var next = SourceCode[currentIdx_ + 1];
                                if (next == 'E' || next == 'e')
                                {
                                    state_ = State.InSciNumber;
                                    currentIdx_++;
                                }
                                if (!ch.IsDigit())
                                {
                                    currentIdx_--;
                                    addToken(TokenType.Float);
                                    resetState();
                                }
                            }
                            else if (ch.IsDigit()) // last char
                            {
                                addToken(TokenType.Float);
                                resetState();
                            }
                            else // last char is not digit
                            {
                                currentIdx_--;
                                addToken(TokenType.Float);
                                currentIdx_++;
                                if (addSingle(ch))
                                    resetState();
                                else
                                    addError("Illegal Ending Char[" + ch + "] Following Integer");
                            }
                            break;

                        case State.InSciNumber:
                            if (ch == '+' || ch == '-')
                            {
                                var prev = SourceCode[currentIdx_ - 1];
                                if (prev != 'E' && prev != 'e')
                                {
                                    currentIdx_--;
                                    addToken(TokenType.SciNumber);
                                    resetState();
                                    Console.WriteLine(" cur idx{0}", currentIdx_);
                                }
                            }
                            else if (!ch.IsDigit())
                            {
                                currentIdx_--;
                                addToken(TokenType.SciNumber);
                                resetState();
                            }
                            break;

                        case State.InIdentifier:
                            if (!ch.IsValidTailingIdCh())
                            {
                                currentIdx_--;
                                addToken(TokenType.Identifier);
                                resetState();
                            }

                            break;

                        case State.InString:
                            {
                                if (ch == '\"')
                                {
                                    currentIdx_--; // skip "
                                    addToken(TokenType.String);
                                    currentIdx_++;
                                    resetState();
                                    break;
                                }
                                else if (ch == '\\')
                                {
                                    state_ = State.InEscapingString;
                                    Console.WriteLine("escape");
                                }
                                else if (ch == '\n')
                                {
                                    addError("Multi-line string");
                                }
                            }
                            break;

                        case State.InEscapingString:// next char after
                            if (ch == '\n')
                            {
                                addError("Multi-line string");
                                resetState();
                            }
                            else
                                state_ = State.InString;

                            break;

                    } // while: foreach ch
                    currentIdx_++;
                }

                currentIdx_--;
                switch (state_)
                {
                    case State.InInteger:
                        addToken(TokenType.Integer);
                        break;
                    case State.InFloat:
                        addToken(TokenType.Float);
                        break;
                    case State.InSciNumber:
                        addToken(TokenType.SciNumber);
                        break;
                    case State.InIdentifier:
                        addToken(TokenType.Identifier);
                        break;
                    case State.InString:
                    case State.InEscapingString:
                        addError("Multi-Line String!");
                        break;
                }
                // ok we are done
                // mark as finished to prevent reparse
                // to re do parsing, see Reparse();
                state_ = State.Finished;
            }
            #endregion

            public void ReSet()
            {
                this.TokenMatrix.Clear();
                this.TokenList.Clear();
                this.ErrorList.Clear();

                this.TokenCount = 0;
                this.currentIdx_ = 0;
                this.currentLineIdx_ = 0;
                this.tokenLeft_ = 0;
                this.lineCount_ = 1;

                this.state_ = State.Start;
            }

            /// <summary>
            /// default out put to Console
            /// </summary>
            public void Print()
            {
                Print(Console.Out);
            }

            public void Print(TextWriter writer)
            {
                this.TokenMatrix.ForEach((CodeLine line)=> writer.Write(line.ToString()));
                if (0 < ErrorList.Count) 
                    return;
                PrintErrors(writer);
            }
            
            public void PrintErrors()
            {
                PrintErrors(Console.Out);
            }

            public void PrintErrors(TextWriter writer)
            {
                writer.WriteLine("{0} Errors:".Fmt(ErrorList.Count));
                this.ErrorList.ForEach(
                    (CodeError ce) => writer.WriteLine(" {0} ", ce.ToString()));
            }

            public string ErrrorStr()
            {
                var w = new StringWriter();
                PrintErrors(w);
                return w.ToString();
            }
            IEnumerator<CodeToken> IEnumerable<CodeToken>.GetEnumerator()
            {
                return TokenList.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return TokenList.GetEnumerator();
            }

            public IBidEnumerator<CodeToken> GetBidEnumerator()
            {
                return new BidEnumerator<CodeToken>(TokenList.GetEnumerator());
            }

        } // class

    } // namespace lexcial 

    #endregion

    
}
