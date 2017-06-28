using System;
using System.Collections.Generic;
using System.Linq;
using VakunTranslatorVol2.Extensions;
using VakunTranslatorVol2.Model.Analyzers;
using static VakunTranslatorVol2.Model.LexemeCodes;

namespace VakunTranslatorVol2.Model
{
    public class PolizGenerator
    {
        public List<string> Poliz { get; private set; }

        private Stack<List<string>> _stack;
        private ILexicalAnalyzer _lexicalAnalyzer;
        private List<string> _labels;
        private List<Lexeme> _lexems;
        private List<string> _loopTempVars;

        public PolizGenerator(ILexicalAnalyzer lexicalAnalyzer)
        {
            _lexicalAnalyzer = lexicalAnalyzer;
            Poliz = new List<string>();
        }

        public List<string> MakePoliz()
        {
            _labels = new List<string>();
            _stack = new Stack<List<string>>();
            _loopTempVars = new List<string>();
            Poliz = new List<string>();
            _lexems = _lexicalAnalyzer.AllLexemes.ToList(); //я не тупой, мне просто нужен новый список

            _lexems.RemoveRange(0, 3);
            _lexems.RemoveAt(_lexems.Count - 1);

            var typedefMode = false;
            for (int i = 0; i < _lexems.Count; i++)
            {
                var lexeme = _lexems[i];

                typedefMode = typedefMode || lexeme.Is(INT) || lexeme.Is(FLOAT);

                if (typedefMode)
                {
                    _lexems.RemoveAt(i--);
                }

                typedefMode = typedefMode && !lexeme.Is(SEMICOLON);
            }

            while (_lexems.Count > 1)
            {
                ParseExpression();
            }

            return Poliz;
        }

        private void ParseExpression()
        {
            switch ((LexemeCodes)_lexems[0].Code)
            {
                case ID: ParseAssign(); break;
                case READ:
                case WRITE: ParseIO(); break;
                case IF: ParseIf(); break;
                case LABEL: ParseLabel(); break;
                case GOTO: ParseGoto(); break;
                case FOR: ParseFor(); break;
            }
        }

        private void ParseFor()
        {
            var loopVars = new
            {
                R0 = $"r{{{_loopTempVars.Count}}}",
                R1 = $"r{{{_loopTempVars.Count + 1}}}"
            };

            var loopLabels = new
            {
                M0 = $"m{{{_labels.Count}}}",
                M1 = $"m{{{_labels.Count + 1}}}"
            };

            #region for
            _stack.Push(new List<string> { _lexems[0].Body });
            _lexems.RemoveAt(0);
            var loopVar = _lexems[0];
            Poliz.Add(_lexems[0].Body);
            _lexems.RemoveAt(0);
            _stack.Push(new List<string> { _lexems[0].Body });
            _lexems.RemoveAt(0);

            DeicstraUntil(TO);
            
            #endregion

            #region to
            _labels.Add(loopLabels.M0);
            Poliz.Add($"{loopLabels.M0}:");
            _loopTempVars.Add(loopVars.R0);
            Poliz.Add(loopVars.R0);

            DeicstraUntil(BY);

            Poliz.Add("=");

            #endregion

            #region by
            _loopTempVars.Add(loopVars.R1);
            Poliz.Add(loopVars.R1);

            DeicstraUntil(WHILE);

            Poliz.Add("=");

            #endregion

            _lexems.RemoveAt(0);

            DeicstraUntil(RIGHT_BRACKET);
            Poliz.AddRange(new[] { loopVar.Body, loopVars.R0, "-", loopVars.R1, "*" , "0", "<=", "&" });

            _labels.Add(loopLabels.M1);
            Poliz.Add(loopLabels.M1);
            Poliz.Add("УПЛ");

            while (!_lexems[0].Is(END))
            {
                ParseExpression();
            }

            _lexems.RemoveAt(0);
            _lexems.RemoveAt(0);
            Poliz.AddRange(new[] { loopVar.Body, loopVar.Body, loopVars.R1, "+", "=" });
            Poliz.Add(loopLabels.M0);
            Poliz.Add("БП");
            Poliz.Add($"{loopLabels.M1}:");
            _stack.Pop();

            void DeicstraUntil(LexemeCodes code)
            {
                while (!_lexems[0].Is(code))
                {
                    Deicstra();
                }

                while (_stack.Any() && GetOperatorPrioretet(_stack.Peek()[0]) >= GetOperatorPrioretet(_lexems[0].Body))
                {
                    Poliz.Add(_stack.Pop()[0]);
                }

                _lexems.RemoveAt(0);
            }
        }

        public void Clear()
        {
            Poliz.Clear();
        }

        private void ParseGoto()
        {
            _lexems.RemoveAt(0);
            Poliz.Add($"m{{{_lexems[0].Body}}}");
            Poliz.Add("БП");
            _lexems.RemoveAt(0);
            _lexems.RemoveAt(0);
        }

        private void ParseLabel()
        {
            _lexems.RemoveAt(0);
            _labels.Add($"m{{{_lexems[0].Body}}}:");
            Poliz.Add(_labels.Last());
            _lexems.RemoveAt(0);
            _lexems.RemoveAt(0);
        }

        private void ParseIf()
        {
            var ifLabel = $"m{{{_labels.Count}}}";
            _stack.Push(new List<string>() { _lexems[0].Body });
            _lexems.RemoveAt(0);

            while (!_lexems[0].Is(THEN))
            {
                Deicstra();
            }

            while (_stack.Peek()[0] != "if")
            {
                Poliz.Add(_stack.Pop()[0]);
            }

            _lexems.RemoveAt(0);
            _labels.Add(ifLabel);
            _stack.Push(new List<string>() { _stack.Pop()[0], _labels.Last() });
            Poliz.Add(ifLabel);
            Poliz.Add("УПЛ");

            ParseExpression();

            while (!_stack.Peek()[0].Equals("if"))
            {
                Poliz.Add(_stack.Pop()[0]);
            }

            Poliz.Add($"{ifLabel}:");

            _stack.Pop();
        }

        private void ParseIO()
        {
            var @operator = _lexems[0];
            _lexems.RemoveAt(0);
            while (!_lexems[0].Is(RIGHT_PARENTHESIS))
            {
                if (_lexems[0].Is(ID))
                {
                    Poliz.Add(_lexems[0].Body);
                    Poliz.Add(@operator.Body);
                }
                _lexems.RemoveAt(0);
            }
            _lexems.RemoveAt(0); // )
            _lexems.RemoveAt(0); // ;
        }

        private void ParseAssign()
        {
            Poliz.Add(_lexems[0].Body);
            _lexems.RemoveAt(0);
            _stack.Push(new List<string> { _lexems[0].Body });
            _lexems.RemoveAt(0);

            while (!_lexems[0].Is(SEMICOLON))
            {
                Deicstra();
            }

            while (_stack.Any() && GetOperatorPrioretet(_stack.Peek()[0]) >= GetOperatorPrioretet(_lexems[0].Body))
            {
                Poliz.Add(_stack.Pop()[0]);
            }

            _lexems.RemoveAt(0);
        }

        private int GetOperatorPrioretet(string op)
        {
            return PriorityTable.FirstOrDefault(x => x.Contains(op))?.Priority ?? -1;
        }


        private void Deicstra()
        {
            if (_lexems[0].Is(CONSTANT) || _lexems[0].Is(ID))
            {
                Poliz.Add(_lexems[0].Body);
                _lexems.RemoveAt(0);
            }
            else
            {
                if (_lexems[0].Is(LEFT_PARENTHESIS) || _lexems[0].Is(LEFT_BRACKET))
                {
                    _stack.Push(new List<string>() { _lexems[0].Body });
                    _lexems.RemoveAt(0);
                }
                else if (_lexems[0].Is(RIGHT_PARENTHESIS) || _lexems[0].Is(RIGHT_BRACKET))
                {
                    while (!new[] { "(", "[" }.Contains(_stack.Peek()[0]))
                    {
                        Poliz.Add(_stack.Pop()[0]);
                    }

                    _stack.Pop();

                    _lexems.RemoveAt(0);
                }
                else
                {
                    while (GetOperatorPrioretet(_stack.Peek()[0]) >= GetOperatorPrioretet(_lexems[0].Body))
                    {
                        if (!(_lexems[0].Is(FOR) || _lexems[0].Is(IF)))
                        {
                            Poliz.Add(_stack.Pop()[0]);
                        }
                    }
                    if (GetOperatorPrioretet(_stack.Peek()[0]) < GetOperatorPrioretet(_lexems[0].Body))
                    {
                        _stack.Push(new List<string>() { _lexems[0].Body });
                    }
                    _lexems.RemoveAt(0);
                }
            }
        }
        public static List<PriorityRule> PriorityTable = new List<PriorityRule>
        {
            new PriorityRule(0, "[","(", "if", "for","read","write" ),
            new PriorityRule(1, ")", "then","]",";", "end","to", "by", "while"),
            new PriorityRule(2, "="),
            new PriorityRule(3, "|"),
            new PriorityRule(4, "&"),
            new PriorityRule(5, "!"),
            new PriorityRule(6, "<", ">" ,"<=",">=","!=","=="),
            new PriorityRule(7, "+", "-" ),
            new PriorityRule(8,  "*", "/")
        };
    }

    public class PriorityRule
    {
        public int Priority { get; set; }
        private List<string> _lexems;

        public PriorityRule(int priority, params string[] lexemes)
        {
            Priority = priority;
            _lexems = lexemes?.ToList() ?? new List<string>();
        }

        public bool Contains(string lexeme)
        {
            return _lexems.Contains(lexeme);
        }
    }
}