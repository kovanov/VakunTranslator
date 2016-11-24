using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VakunTranslatorVol2.Extensions;
using VakunTranslatorVol2.Modes;

namespace VakunTranslatorVol2.Analyzers
{
    public class LexicalAnalyzer : ILexicalAnalyzer
    {
        public List<Lexeme> Constants { get; private set; } = new List<Lexeme>();
        public List<Lexeme> Identificators { get; private set; } = new List<Lexeme>();
        public List<Lexeme> AllLexemes { get; private set; } = new List<Lexeme>();
        public List<string> Errors { get; private set; } = new List<string>();
        public bool HasErrors { get { return Errors.Any(); } }

        private int lineNumber;

        public List<Lexeme> Analyze(string sourceCode, CancellationToken token)
        {
            Mode.LexemeComplited += SaveLexeme;
            Mode.NewLine += MoveToNextLine;

            try
            {
                sourceCode.Aggregate(Mode.Initial, (acc, symbol) =>
                {
                    token.ThrowIfCancellationRequested();
                    return acc.MoveNext(symbol);
                }).PickLast();

                DefineIds();
            }
            catch(ArgumentException e)
            {
                Errors.Add($"{e.Message} at {lineNumber} line");
            }
            catch(InvalidOperationException io)
            {
                Errors.Add(io.Message);
            }
            finally
            {
                Mode.LexemeComplited -= SaveLexeme;
                Mode.NewLine -= MoveToNextLine;
            }

            return AllLexemes;
        }

        private void DefineIds()
        {
            var enumerator = AllLexemes.GetEnumerator();
            var moveNext = enumerator.MoveNext();

            while(moveNext)
            {
                while(moveNext)
                {
                    if(enumerator.Current.Is(LexemeFlags.TypeDefinition))
                    {
                        break;
                    }
                    moveNext = enumerator.MoveNext();
                }
                if(moveNext)
                {
                    var type = enumerator.Current;

                    while(moveNext = enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if(current.Is(LexemeFlags.Id))
                        {
                            if(string.IsNullOrEmpty(current.Type))
                            {
                                foreach(var lexeme in AllLexemes.Where(x=>type.Line <= x.Line).Where(x => x.Body == current.Body))
                                {
                                    lexeme.Type = type.Body;
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException($"Error at line {current.Line}, {current.Body} already defined");
                            }
                        }
                        if(!current.Is(LexemeFlags.Id| LexemeFlags.InlineDelimiter))
                        {
                            break;
                        }
                    }
                }
            }

            var undefined = AllLexemes.Where(x => x.Is(LexemeFlags.Id)).FirstOrDefault(x => string.IsNullOrEmpty(x.Type));
            if(undefined != null)
            {
                throw new InvalidOperationException($"Undefined id {undefined.Body} at line {undefined.Line}");
            }
        }

        private void MoveToNextLine()
        {
            lineNumber++;
        }

        private void SaveLexeme(string accumulatedLexeme)
        {
            if(AllLexemes.Any())
            {
                var last = AllLexemes.Last();
                var previous = new[] { LexemeCodes.RIGHT_PARENTHESIS, LexemeCodes.ID, LexemeCodes.CONSTANT };

                if(accumulatedLexeme.Length > 1 && accumulatedLexeme.First().IsSign() && previous.Any(x => (int)x == last.Code))
                {
                    SaveLexeme(accumulatedLexeme.Substring(0, 1));
                    SaveLexeme(accumulatedLexeme.Substring(1));
                    return;
                }
            }

            var lexeme = Lexeme.Parse(accumulatedLexeme);
            lexeme.Line = lineNumber;

            if(lexeme.Is(LexemeFlags.Const))
            {
                AddTo(Constants, lexeme);
            }

            if(lexeme.Is(LexemeFlags.Id))
            {
                AddTo(Identificators, lexeme);
            }

            AllLexemes.Add(lexeme);
        }

        private void AddTo(List<Lexeme> list, Lexeme lexeme)
        {
            if(!list.Contains(lexeme))
            {
                lexeme.ConstCode = list.Count;
                list.Add(lexeme);
            }
        }

        public void Dispose()
        {
            Errors.Clear();
            Constants.Clear();
            Identificators.Clear();
            AllLexemes.Clear();
            lineNumber = 0;
        }
    }
}