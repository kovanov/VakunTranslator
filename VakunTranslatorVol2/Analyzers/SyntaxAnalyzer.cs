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

        public void Analyze(List<Lexeme> lexemes, CancellationToken token)
        {
            this.lexemes = lexemes;
            this.token = token;

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
                if(currentIndex < lexemes.Count)
                {
                    Errors.Add($"Full program, but there is another code after last brace (line {lexemes[currentIndex - 1].Line})");
                }
            }
        }
        private bool CommandList()
        {
            return Train[Command][SEMICOLON]["command list"] && Train[Command][SEMICOLON]["command list"].Repeat(() => currentIndex);
        }
        private bool Command()
        {
            return Train[Operator, Declaration, LabelDeclaration]["command"];
        }
        private bool Declaration()
        {
            return Train[INT, FLOAT][IdList]["declaration"];
        }
        private bool IdList()
        {
            return Train[ID]["id list"] && Train[COMMA][ID]["id list"].Repeat(() => currentIndex);
        }
        private bool LabelDeclaration()
        {
            return Train[LABEL][ID]["label declaration"];
        }
        private bool Operator()
        {
            return Train[Assignment, Loop, Conditional, Input, Output, GotoOperator]["operator"];
        }
        private bool GotoOperator()
        {
            return Train[GOTO][ID]["goto operator"];
        }
        private bool Assignment()
        {
            return Train[ID][ASSIGN][Expression]["assignment"];
        }
        private bool Output()
        {
            return Train[WRITE][LEFT_PARENTHESIS][IdList][RIGHT_PARENTHESIS]["output"];
        }
        private bool Input()
        {
            return Train[READ][LEFT_PARENTHESIS][ID][RIGHT_PARENTHESIS]["input"];
        }
        private bool Conditional()
        {
            return Train[IF][Relation][THEN][Command]["conditional"];
        }
        private bool Loop()
        {
            return Train[FOR][Assignment][TO][Expression][BY][Expression][WHILE][LEFT_PARENTHESIS][Relation][RIGHT_PARENTHESIS][CommandList][END]["loop"];
        }
        private bool LogicalExpression()
        {
            return Train[LogicalTerm]["logical expression"] && Train[OR][LogicalTerm]["logical expression"].Repeat(() => currentIndex);
        }
        private bool LogicalTerm()
        {
            return Train[LogicalMultiplier]["logical term"] && Train[AND][LogicalMultiplier]["logical term"].Repeat(() => currentIndex);
        }
        private bool LogicalMultiplier()
        {
            return Train[Relation, () => Train[NOT][LogicalMultiplier]["logical multiplier"], () => Train[LEFT_PARENTHESIS][LogicalExpression][RIGHT_PARENTHESIS]["logical multiplier"]]["logical multiplier"];
        }
        private bool Relation()
        {
            return Train[Expression][RelationSign][Expression]["relation"];
        }
        private bool RelationSign()
        {
            return Train[LESS_THAN, LESS_EQUAL, MORE_THAN, MORE_EQUAL, NOT_EQUAL, EQUAL]["relation sign"];
        }
        private bool Expression()
        {
            return Train[Term]["expression"] && Train[PLUS, MINUS][Term]["expression"].Repeat(() => currentIndex);
        }
        private bool Term()
        {
            return Train[Operand]["term"] && Train[MULTIPLY, DIVISION][Term]["term"].Repeat(() => currentIndex);
        }
        private bool Operand()
        {
            return Train[() => Train[ID, CONSTANT]["operand"], () => Train[LEFT_PARENTHESIS][Expression][RIGHT_PARENTHESIS]["operand"]]["operand"];
        }

        private bool MoveNext(LexemeCodes code)
        {
            token.ThrowIfCancellationRequested();

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
            get { return Train<LexemeCodes>.New(currentIndex, i => currentIndex = i); }
        }

        private List<Lexeme> lexemes;
        private int currentIndex;
        private CancellationToken token;
    }

    class Train<T>
    {
        public static Train<T> New(int fallbackIndex, Action<int> fallback)
        {
            return new Train<T>(fallbackIndex, fallback);
        }
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
                predicates.Add(() =>
                {
                    foreach(var predicate in predicateGroup)
                    {
                        if(predicate())
                        {
                            return true;
                        }
                        fallback(fallbackIndex);
                    }
                    return false;
                });

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
            t.fallback(t.fallbackIndex);
            OnError(t.errorMsg);
            return false;
        }

        public bool Repeat(Func<int> fallbackIndexFactory)
        {
            while(predicates.All(x => x()))
            {
                fallbackIndex = fallbackIndexFactory();
            }
            fallback(fallbackIndex);
            OnError(errorMsg);
            return true;
        }

        private Train(int fallbackIndex, Action<int> onFail)
        {
            this.fallbackIndex = fallbackIndex;
            this.fallback = onFail;
            this.predicates = new List<Func<bool>>();
        }

        private Action<int> fallback;
        private string errorMsg;
        private int fallbackIndex;
        private List<Func<bool>> predicates;
    }
}