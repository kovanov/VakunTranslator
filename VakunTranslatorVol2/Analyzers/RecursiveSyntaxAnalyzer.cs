using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VakunTranslatorVol2.Extensions;
using static VakunTranslatorVol2.LexemeCodes;

namespace VakunTranslatorVol2.Analyzers
{
    public class RecursiveSyntaxAnalyzer : ISyntaxAnalyzer
    {
        public List<string> Errors { get; private set; } = new List<string>();
        private Lexeme CurrentLexeme
        {
            get { return lexemes[currentIndex]; }
        }
        public bool HasErrors
        {
            get { return Errors.Any(); }
        }

        public RecursiveSyntaxAnalyzer()
        {
            Train<LexemeCodes>.Predicate = MoveNext;
            Train<LexemeCodes>.OnError = ErrorFrom;
            Train<LexemeCodes>.OnSuccess = Errors.Clear;
        }

        public void Analyze(List<Lexeme> lexemes)
        {
            this.lexemes = lexemes;

            try
            {
                Program();
            }
            catch(Exception e)
            {
                Errors.Add(e.Message);
            }
        }
        public void Dispose()
        {
            currentIndex = 0;
            Errors.Clear();
        }

        private void Program()
        {
            if(Train[PROGRAM][ID][LEFT_BRACE][CommandList][RIGHT_BRACE]["program"])
            {
                Errors.Clear();
                if(currentIndex < lexemes.Count)
                {
                    Errors.Add($"Full program, but there is another code after last brace (line {lexemes[currentIndex - 1].Line})");
                }
            }
        }
        private bool CommandList()
        {
            return Train[Command][SEMICOLON]["command list"] && Train[Command][SEMICOLON]["command list"].Repeat();
        }
        private bool Command()
        {
            return (Train[INT, FLOAT][ID]["command"] && (Train[COMMA][ID]["command"].Repeat())) ||
                    (Train[LABEL][ID]["label declaration"]) ||
                    (Train[GOTO][ID]["goto operator"]) ||
                    (Train[ID][ASSIGN][Expression]["assignment"]) ||
                    (Train[WRITE][LEFT_PARENTHESIS][IdList][RIGHT_PARENTHESIS]["output"]) ||
                    (Train[READ][LEFT_PARENTHESIS][ID][RIGHT_PARENTHESIS]["input"]) ||
                    (Train[IF][LogicalExpression][THEN][Command]["conditional"]) ||
                    (Train[FOR][ID][ASSIGN][Expression][TO][Expression][BY][Expression][WHILE][LEFT_PARENTHESIS][LogicalExpression][RIGHT_PARENTHESIS][CommandList][END]["loop"]);
        }
        private bool IdList()
        {
            return Train[ID]["id list"] && Train[COMMA][ID]["id list"].Repeat();
        }
        private bool LogicalExpression()
        {
            return Train[LogicalTerm]["logical expression"] && Train[OR][LogicalTerm]["logical expression"].Repeat();
        }
        private bool LogicalTerm()
        {
            return Train[LogicalMultiplier]["logical term"] && Train[AND][LogicalMultiplier]["logical term"].Repeat();
        }
        private bool LogicalMultiplier()
        {
            return Train[LEFT_PARENTHESIS][LogicalExpression][RIGHT_PARENTHESIS]["logical multiplier"]["logical multiplier"] || Train[Relation]["logical multiplier"] || Train[NOT][LogicalMultiplier]["logical multiplier"];
        }
        private bool Relation()
        {
            return Train[Expression][LESS_THAN, LESS_EQUAL, MORE_THAN, MORE_EQUAL, NOT_EQUAL, EQUAL][Expression]["relation"];
        }
        private bool Expression()
        {
            return Train[Term]["expression"] && Train[PLUS, MINUS][Term]["expression"].Repeat();
        }
        private bool Term()
        {
            return Train[Operand]["term"] && Train[MULTIPLY, DIVISION][Term]["term"].Repeat();
        }
        private bool Operand()
        {
            return Train[LEFT_PARENTHESIS][Expression][RIGHT_PARENTHESIS]["operand"] || Train[ID, CONSTANT]["operand"];
        }

        private bool MoveNext(LexemeCodes code)
        {
            if(currentIndex == lexemes.Count)
            {
                throw new InvalidOperationException($"I need moaar, give me {code}");
            }

            if(CurrentLexeme.Code == (int)code)
            {
                currentIndex++;
                return true;
            }

            return false;
        }
        private void ErrorFrom(string str)
        {
            Errors.Add($"Wrong {str}, line {CurrentLexeme.Line}");
        }
        private Train<LexemeCodes> Train
        {
            get { return new Train<LexemeCodes>(); }
        }

        private List<Lexeme> lexemes;
        private int currentIndex;
    }

    class Train<T>
    {
        public static Func<T, bool> Predicate { get; set; }
        public static Action<string> OnError { get; set; }
        public static Action OnSuccess { get; set; }


        public Train<T> this[T obj]
        {
            get
            {
                predicates.Add(() => Predicate(obj));
                return this;
            }
        }
        public Train<T> this[params T[] objs]
        {
            get
            {
                predicates.Add(() => objs.Any(Predicate));
                return this;
            }
        }
        public Train<T> this[params Func<bool>[] predicateGroup]
        {
            get
            {
                predicates.Add(() => predicateGroup.Any(x => x()));
                return this;
            }
        }
        public Train<T> this[Func<bool> predicate]
        {
            get
            {
                predicates.Add(predicate);
                return this;
            }
        }
        public Train<T> this[string errorMsg]
        {
            get
            {
                this.errorMsg = errorMsg;
                return this;
            }
        }

        public static implicit operator bool(Train<T> t)
        {
            if(t.predicates.All(x => x()))
            {
                OnSuccess();
                return true;
            }
            OnError(t.errorMsg);
            return false;
        }

        public bool Repeat()
        {
            while(true)
            {
                var enumerator = predicates.GetEnumerator();

                enumerator.MoveNext();

                if(enumerator.Current())
                {
                    while(enumerator.MoveNext())
                    {
                        if(!enumerator.Current())
                        {
                            OnError(errorMsg);
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public Train()
        {
            this.predicates = new List<Func<bool>>();
        }

        private string errorMsg;
        private List<Func<bool>> predicates;
    }
}