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
        private Stack<List<string>> _stack;
        private ILexicalAnalyzer _lexicalAnalyzer;
        private List<string> _labels;
        private List<Lexeme> _lexems;
        private List<string> _poliz;
        private List<string> _loopTempVars;

        public PolizGenerator(ILexicalAnalyzer lexicalAnalyzer)
        {
            _lexicalAnalyzer = lexicalAnalyzer;
        }

        public List<string> MakePoliz()
        {
            _labels = new List<string>();
            _stack = new Stack<List<string>>();
            _loopTempVars = new List<string>();
            _poliz = new List<string>();
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

            return _poliz;
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
            _poliz.Add(_lexems[0].Body);
            _lexems.RemoveAt(0);
            _stack.Push(new List<string> { _lexems[0].Body });
            _lexems.RemoveAt(0);

            DeicstraUntil(TO);
            
            #endregion

            #region to
            _labels.Add(loopLabels.M0);
            _poliz.Add($"{loopLabels.M0}:");
            _loopTempVars.Add(loopVars.R0);
            _poliz.Add(loopVars.R0);

            DeicstraUntil(BY);

            _poliz.Add("=");

            #endregion

            #region by
            _loopTempVars.Add(loopVars.R1);
            _poliz.Add(loopVars.R1);

            DeicstraUntil(WHILE);

            _poliz.Add("=");

            #endregion

            _lexems.RemoveAt(0);

            DeicstraUntil(RIGHT_BRACKET);
            _poliz.AddRange(new[] { loopVar.Body, loopVars.R0, "-", "0", "<=", "&" });

            _labels.Add(loopLabels.M1);
            _poliz.Add(loopLabels.M1);
            _poliz.Add("УПЛ");

            while (!_lexems[0].Is(END))
            {
                ParseExpression();
            }

            _lexems.RemoveAt(0);
            _lexems.RemoveAt(0);
            _poliz.AddRange(new[] { loopVar.Body, loopVar.Body, loopVars.R1, "+", "=" });
            _poliz.Add(loopLabels.M0);
            _poliz.Add("БП");
            _poliz.Add($"{loopLabels.M1}:");
            _stack.Pop();

            void DeicstraUntil(LexemeCodes code)
            {
                while (!_lexems[0].Is(code))
                {
                    Deicstra();
                }

                while (_stack.Any() && GetOperatorPrioretet(_stack.Peek()[0]) >= GetOperatorPrioretet(_lexems[0].Body))
                {
                    _poliz.Add(_stack.Pop()[0]);
                }

                _lexems.RemoveAt(0);
            }
        }

        private void ParseGoto()
        {
            _lexems.RemoveAt(0);
            _poliz.Add($"m{{{_lexems[0].Body}}}");
            _poliz.Add("БП");
            _lexems.RemoveAt(0);
            _lexems.RemoveAt(0);
        }

        private void ParseLabel()
        {
            _lexems.RemoveAt(0);
            _labels.Add($"m{{{_lexems[0].Body}}}:");
            _poliz.Add(_labels.Last());
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
                _poliz.Add(_stack.Pop()[0]);
            }

            _lexems.RemoveAt(0);
            _labels.Add(ifLabel);
            _stack.Push(new List<string>() { _stack.Pop()[0], _labels.Last() });
            _poliz.Add(ifLabel);
            _poliz.Add("УПЛ");

            ParseExpression();

            while (!_stack.Peek()[0].Equals("if"))
            {
                _poliz.Add(_stack.Pop()[0]);
            }

            _poliz.Add($"{ifLabel}:");

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
                    _poliz.Add(_lexems[0].Body);
                    _poliz.Add(@operator.Body);
                }
                _lexems.RemoveAt(0);
            }
            _lexems.RemoveAt(0); // )
            _lexems.RemoveAt(0); // ;
        }

        private void ParseAssign()
        {
            _poliz.Add(_lexems[0].Body);
            _lexems.RemoveAt(0);
            _stack.Push(new List<string> { _lexems[0].Body });
            _lexems.RemoveAt(0);

            while (!_lexems[0].Is(SEMICOLON))
            {
                Deicstra();
            }

            while (_stack.Any() && GetOperatorPrioretet(_stack.Peek()[0]) >= GetOperatorPrioretet(_lexems[0].Body))
            {
                _poliz.Add(_stack.Pop()[0]);
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
                _poliz.Add(_lexems[0].Body);
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
                        _poliz.Add(_stack.Pop()[0]);
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
                            _poliz.Add(_stack.Pop()[0]);
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