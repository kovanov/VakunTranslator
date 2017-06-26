using System;
using System.Collections.Generic;
using System.Linq;
using static VakunTranslatorVol2.Model.LexemeCodes;

namespace VakunTranslatorVol2.Model.Analyzers
{
    public class PDASyntaxAnalyzer : SyntaxAnalyzer
    {
        private static Stack<int> bubbleStack = new Stack<int>();
        private List<UsedRule> usedRules = new List<UsedRule>();
        private Action<List<UsedRule>> onReady;

        public PDASyntaxAnalyzer(Action<List<UsedRule>> onReady)
        {
            this.onReady = onReady;
        }
        public override void Analyze(List<Lexeme> lexemes)
        {
            var currentAlpha = 1;
            foreach(var lexeme in lexemes)
            {
                try
                {
                    var currentRules = PDATable.Where(x => x.Alpha == currentAlpha);
                    var currentRule = currentRules.FirstOrDefault(x => (int)x.LexemeCode == lexeme.Code);

                    while(currentRule == null)
                    {
                        currentAlpha = ParseFault(currentRules);
                        currentRules = PDATable.Where(x => x.Alpha == currentAlpha);
                        currentRule = currentRules.FirstOrDefault(x => (int)x.LexemeCode == lexeme.Code);
                    }

                    currentAlpha = ProcessRule(currentRule);

                    usedRules.Add(new UsedRule { Rule = currentRule, StackState = GetStackState() });
                }
                catch(Exception e)
                {
                    Errors.Add($"{e.Message} on line {lexeme.Line}");
                    onReady(usedRules);
                    return;
                }
            }

            if(currentAlpha != 26)
            {
                Errors.Add("Program must ends with }");
            }

            onReady(usedRules);
        }

        private string GetStackState()
        {
            return string.Join(" ", bubbleStack);
        }
        private int ProcessRule(PDARule rule)
        {
            if(rule.OnComparisionSuccess == "exit")
            {
                return bubbleStack.Pop();
            }

            if(rule.Stack != null)
            {
                bubbleStack.Push((int)rule.Stack);
            }

            return (int)rule.Beta;
        }
        private int ParseFault(IEnumerable<PDARule> currentRules)
        {
            var rule = currentRules.First();
            var faultString = rule.OnComparisionFault;

            if(faultString == "exit")
            {
                return bubbleStack.Pop();
            }

            if(faultString == "error")
            {
                var expectedCodes = currentRules.Select(x => x.LexemeCode);
                var errorMessage = string.Join(", ", expectedCodes);

                throw new ArgumentException($"Error, expected {errorMessage}");
            }


            var numbers = faultString
                            .Split()
                            .Select(int.Parse)
                            .ToArray();

            var newAlpha = numbers.First();
            var stack = numbers.Last();

            bubbleStack.Push(stack);

            return newAlpha;
        }

        public override void Dispose()
        {
            bubbleStack.Clear();
            Errors.Clear();
            usedRules.Clear();
        }

        private static PDARule[] PDATable = new[]
        {
            new PDARule { Alpha = 1,    LexemeCode = PROGRAM,            Beta = 2,                                                                          },
            new PDARule { Alpha = 2,    LexemeCode = ID,                 Beta = 3,                                                                          },
            new PDARule { Alpha = 3,    LexemeCode = LEFT_BRACE,         Beta = 6,  Stack = 4,                                                              },
            new PDARule { Alpha = 4,    LexemeCode = SEMICOLON,          Beta = 5,                                                                          },
            new PDARule { Alpha = 5,    LexemeCode = RIGHT_BRACE,        Beta = 26,             OnComparisionFault = "6 4",                                 },
            new PDARule { Alpha = 6,    LexemeCode = INT,                Beta = 7,                                                                          },
            new PDARule { Alpha = 6,    LexemeCode = FLOAT,              Beta = 7,                                                                          },
            new PDARule { Alpha = 6,    LexemeCode = LABEL,              Beta = 10,                                                                         },
            new PDARule { Alpha = 6,    LexemeCode = WRITE,              Beta = 12,                                                                         },
            new PDARule { Alpha = 6,    LexemeCode = READ,               Beta = 15,                                                                         },
            new PDARule { Alpha = 6,    LexemeCode = ID,                 Beta = 18,                                                                         },
            new PDARule { Alpha = 6,    LexemeCode = FOR,                Beta = 20,                                                                         },
            new PDARule { Alpha = 6,    LexemeCode = IF,                 Beta = 37, Stack = 31,                                                             },
            new PDARule { Alpha = 6,    LexemeCode = GOTO,               Beta = 10,                                                                         },
            new PDARule { Alpha = 7,    LexemeCode = ID,                 Beta = 8,                                                                          },
            new PDARule { Alpha = 8,    LexemeCode = COMMA,              Beta = 41,             OnComparisionFault = "exit"                                 },
            new PDARule { Alpha = 8,    LexemeCode = ASSIGN,             Beta = 34,             OnComparisionFault = "exit"                                 },
            new PDARule { Alpha = 9,    LexemeCode = NOTHING,                                   OnComparisionFault = "exit"                                 },
            new PDARule { Alpha = 10,   LexemeCode = LABEL_ID,                                                                    OnComparisionSuccess = "exit"   },
            new PDARule { Alpha = 12,   LexemeCode = LEFT_PARENTHESIS,   Beta = 13,                                                                         },
            new PDARule { Alpha = 13,   LexemeCode = ID,                 Beta = 14,                                                                         },
            new PDARule { Alpha = 14,   LexemeCode = COMMA,              Beta = 13,                                                                         },
            new PDARule { Alpha = 14,   LexemeCode = RIGHT_PARENTHESIS,                                                     OnComparisionSuccess = "exit"   },
            new PDARule { Alpha = 15,   LexemeCode = LEFT_PARENTHESIS,   Beta = 16,                                                                         },
            new PDARule { Alpha = 16,   LexemeCode = ID,                 Beta = 17,                                                                         },
            new PDARule { Alpha = 17,   LexemeCode = RIGHT_PARENTHESIS,                                                     OnComparisionSuccess = "exit"   },
            new PDARule { Alpha = 18,   LexemeCode = ASSIGN,             Beta = 34, Stack = 19                                                              },
            new PDARule { Alpha = 19,   LexemeCode = NOTHING,                                  OnComparisionFault = "exit"                                  },
            new PDARule { Alpha = 20,   LexemeCode = ID,                 Beta = 21                                                                          },
            new PDARule { Alpha = 21,   LexemeCode = ASSIGN,             Beta = 34, Stack = 22                                                              },
            new PDARule { Alpha = 22,   LexemeCode = TO,                 Beta = 34, Stack = 23                                                              },
            new PDARule { Alpha = 23,   LexemeCode = BY,                 Beta = 34, Stack = 24                                                              },
            new PDARule { Alpha = 24,   LexemeCode = WHILE,              Beta = 30,                                                                         },
            new PDARule { Alpha = 25,   LexemeCode = LEFT_PARENTHESIS,   Beta = 37, Stack = 27                                                              },
            new PDARule { Alpha = 26,   LexemeCode = NOTHING,                                                                                               },
            new PDARule { Alpha = 27,   LexemeCode = RIGHT_PARENTHESIS,  Beta = 6,  Stack = 28                                                              },
            new PDARule { Alpha = 28,   LexemeCode = SEMICOLON,          Beta = 29,                                                                         },
            new PDARule { Alpha = 29,   LexemeCode = END,                                       OnComparisionFault = "6 28", OnComparisionSuccess = "exit"  },
            new PDARule { Alpha = 30,   LexemeCode = LEFT_BRACKET,       Beta = 37, Stack = 32                                                              },
            new PDARule { Alpha = 31,   LexemeCode = THEN,               Beta = 6,  Stack = 19                                                              },
            new PDARule { Alpha = 32,   LexemeCode = RIGHT_BRACKET,      Beta = 6,  Stack = 28                                                              },
            new PDARule { Alpha = 34,   LexemeCode = ID,                 Beta = 36,                                                                         },
            new PDARule { Alpha = 34,   LexemeCode = CONSTANT,           Beta = 36,                                                                         },
            new PDARule { Alpha = 34,   LexemeCode = LEFT_PARENTHESIS,   Beta = 34, Stack = 35                                                              },
            new PDARule { Alpha = 35,   LexemeCode = RIGHT_PARENTHESIS,  Beta = 36,                                                                         },
            new PDARule { Alpha = 36,   LexemeCode = PLUS,               Beta = 34,             OnComparisionFault = "exit"                                 },
            new PDARule { Alpha = 36,   LexemeCode = MINUS,              Beta = 34,                                                                         },
            new PDARule { Alpha = 36,   LexemeCode = MULTIPLY,           Beta = 34,                                                                         },
            new PDARule { Alpha = 36,   LexemeCode = DIVISION,           Beta = 34,                                                                         },
            new PDARule { Alpha = 37,   LexemeCode = NOT,                Beta = 37,             OnComparisionFault = "34 38"                                },
            new PDARule { Alpha = 37,   LexemeCode = LEFT_PARENTHESIS,   Beta = 37, Stack = 39, OnComparisionFault = "34 38"                                },
            new PDARule { Alpha = 38,   LexemeCode = LESS_EQUAL,         Beta = 34, Stack = 40                                                              },
            new PDARule { Alpha = 38,   LexemeCode = LESS_THAN,          Beta = 34, Stack = 40                                                              },
            new PDARule { Alpha = 38,   LexemeCode = MORE_EQUAL,         Beta = 34, Stack = 40                                                              },
            new PDARule { Alpha = 38,   LexemeCode = MORE_THAN,          Beta = 34, Stack = 40                                                              },
            new PDARule { Alpha = 38,   LexemeCode = EQUAL,              Beta = 34, Stack = 40                                                              },
            new PDARule { Alpha = 38,   LexemeCode = NOT_EQUAL,          Beta = 34, Stack = 40                                                              },
            new PDARule { Alpha = 39,   LexemeCode = RIGHT_PARENTHESIS,  Beta = 40,                                                                         },
            new PDARule { Alpha = 40,   LexemeCode = AND,                Beta = 37,             OnComparisionFault = "exit"                                 },
            new PDARule { Alpha = 40,   LexemeCode = OR,                 Beta = 37,             OnComparisionFault = "exit"                                 },
            new PDARule { Alpha = 41,   LexemeCode = ID,                 Beta = 42,                                                                         },
            new PDARule { Alpha = 42,   LexemeCode = COMMA,              Beta = 41,             OnComparisionFault = "exit"                                 },
        };

        public class PDARule
        {
            public int Alpha { get; set; }
            public LexemeCodes LexemeCode { get; set; }
            public int? Beta { get; set; }
            public int? Stack { get; set; }
            public string OnComparisionSuccess { get; set; }
            public string OnComparisionFault { get; set; } = "error";
        }

        public class UsedRule
        {
            public PDARule Rule { get; set; }
            public string StackState { get; set; }
        }
    }
}