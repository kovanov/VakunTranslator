using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VakunTranslatorVol2.Extensions;
using static VakunTranslatorVol2.LexemeCodes;

namespace VakunTranslatorVol2.Analyzers
{
    public class PDASyntaxAnalyzer : ISyntaxAnalyzer
    {
        public List<string> Errors { get; private set; } = new List<string>();
        public bool HasErrors
        {
            get { return Errors.Any(); }
        }

        private static Stack<int> bubbleStack = new Stack<int>();

        public void Analyze(List<Lexeme> lexemes, CancellationToken token)
        {
            var currentAlpha = 1;
            foreach(var lexeme in lexemes)
            {
                token.ThrowIfCancellationRequested();
                var currentRule = PDATable
                                 .Where(x => x.Alpha == currentAlpha)
                                 .FirstOrDefault(x => (int)x.LabelCode == lexeme.Code);

                while(currentRule.IsNull())
                {
                    try
                    {
                        currentAlpha = inComparisionTable[currentAlpha](lexeme.Line);
                        currentRule = PDATable
                                        .Where(x => x.Alpha == currentAlpha)
                                        .FirstOrDefault(x => (int)x.LabelCode == lexeme.Code);
                    }
                    catch(InvalidOperationException e)
                    {
                        Errors.Add(e.Message);
                        return;
                    }
                }

                if(currentRule.Stack.IsNotNull())
                {
                    bubbleStack.Push((int)currentRule.Stack);
                }

                currentAlpha = currentRule.Beta;
            }

            if(currentAlpha != 5)
            {
                Errors.Add("Program must ends with }");
            }
        }

        public void Dispose()
        {
            bubbleStack.Clear();
            Errors.Clear();
        }

        private static PDARule[] PDATable = new[]
        {
            new PDARule { Alpha = 1,    LabelCode = PROGRAM,            Beta = 2                  },
            new PDARule { Alpha = 2,    LabelCode = ID,                 Beta = 3                  },
            new PDARule { Alpha = 3,    LabelCode = LEFT_BRACE,         Beta = 6,     Stack = 4   },
            new PDARule { Alpha = 4,    LabelCode = RIGHT_BRACE,        Beta = 5                  },
            new PDARule { Alpha = 5,    LabelCode = UNDEFINED,          Beta = 0                  },
            new PDARule { Alpha = 6,    LabelCode = INT,                Beta = 9,     Stack = 7   },
            new PDARule { Alpha = 6,    LabelCode = FLOAT,              Beta = 9,     Stack = 7   },
            new PDARule { Alpha = 6,    LabelCode = ID,                 Beta = 11,    Stack = 7   },
            new PDARule { Alpha = 6,    LabelCode = READ,               Beta = 15,                },
            new PDARule { Alpha = 6,    LabelCode = WRITE,              Beta = 18,                },
            new PDARule { Alpha = 6,    LabelCode = GOTO,               Beta = 19,                },
            new PDARule { Alpha = 6,    LabelCode = LABEL,              Beta = 19,                },
            new PDARule { Alpha = 6,    LabelCode = FOR,                Beta = 20,                },
            new PDARule { Alpha = 6,    LabelCode = IF,                 Beta = 27,    Stack = 31  },
            new PDARule { Alpha = 7,    LabelCode = SEMICOLON,          Beta = 8                  },
            new PDARule { Alpha = 8,    LabelCode = INT,                Beta = 9,     Stack = 7   },
            new PDARule { Alpha = 8,    LabelCode = FLOAT,              Beta = 9,     Stack = 7   },
            new PDARule { Alpha = 8,    LabelCode = ID,                 Beta = 11,    Stack = 7   },
            new PDARule { Alpha = 8,    LabelCode = READ,               Beta = 15,                },
            new PDARule { Alpha = 8,    LabelCode = WRITE,              Beta = 18,                },
            new PDARule { Alpha = 8,    LabelCode = GOTO,               Beta = 19,                },
            new PDARule { Alpha = 8,    LabelCode = LABEL,              Beta = 19,                },
            new PDARule { Alpha = 8,    LabelCode = FOR,                Beta = 20,                },
            new PDARule { Alpha = 8,    LabelCode = IF,                 Beta = 27,    Stack = 31  },
            new PDARule { Alpha = 9,    LabelCode = ID,                 Beta = 10,                },
            new PDARule { Alpha = 10,   LabelCode = COMMA,              Beta = 9,                 },
            new PDARule { Alpha = 11,   LabelCode = ASSIGN,             Beta = 12,                },
            new PDARule { Alpha = 12,   LabelCode = ID,                 Beta = 14,                },
            new PDARule { Alpha = 12,   LabelCode = CONSTANT,           Beta = 14,                },
            new PDARule { Alpha = 12,   LabelCode = LEFT_PARENTHESIS,   Beta = 12,    Stack = 13  },
            new PDARule { Alpha = 13,   LabelCode = RIGHT_PARENTHESIS,  Beta = 14,                },
            new PDARule { Alpha = 14,   LabelCode = MULTIPLY,           Beta = 12,                },
            new PDARule { Alpha = 14,   LabelCode = PLUS,               Beta = 12,                },
            new PDARule { Alpha = 14,   LabelCode = MINUS,              Beta = 12,                },
            new PDARule { Alpha = 14,   LabelCode = DIVISION,           Beta = 12,                },
            new PDARule { Alpha = 15,   LabelCode = LEFT_PARENTHESIS,   Beta = 16,                },
            new PDARule { Alpha = 16,   LabelCode = ID,                 Beta = 17,                },
            new PDARule { Alpha = 17,   LabelCode = RIGHT_PARENTHESIS,  Beta = 7,                 },
            new PDARule { Alpha = 18,   LabelCode = LEFT_PARENTHESIS,   Beta = 9,     Stack = 17  },
            new PDARule { Alpha = 19,   LabelCode = ID,                 Beta = 7,                 },
            new PDARule { Alpha = 20,   LabelCode = ID,                 Beta = 11,    Stack = 21  },
            new PDARule { Alpha = 21,   LabelCode = TO,                 Beta = 12,    Stack = 22  },
            new PDARule { Alpha = 22,   LabelCode = BY,                 Beta = 12,    Stack = 23  },
            new PDARule { Alpha = 23,   LabelCode = WHILE,              Beta = 24,                },
            new PDARule { Alpha = 24,   LabelCode = LEFT_PARENTHESIS,   Beta = 27,    Stack = 25  },
            new PDARule { Alpha = 25,   LabelCode = RIGHT_PARENTHESIS,  Beta = 6,     Stack = 26  },
            new PDARule { Alpha = 26,   LabelCode = END,                Beta = 7,                 },
            new PDARule { Alpha = 27,   LabelCode = NOT,                Beta = 27,                },
            new PDARule { Alpha = 27,   LabelCode = ID,                 Beta = 14,    Stack = 28  },
            new PDARule { Alpha = 27,   LabelCode = CONSTANT,           Beta = 14,    Stack = 28  },
            new PDARule { Alpha = 27,   LabelCode = LEFT_PARENTHESIS,   Beta = 27,    Stack = 30  },
            new PDARule { Alpha = 28,   LabelCode = LESS_EQUAL,         Beta = 12,    Stack = 29  },
            new PDARule { Alpha = 28,   LabelCode = MORE_EQUAL,         Beta = 12,    Stack = 29  },
            new PDARule { Alpha = 28,   LabelCode = MORE_THAN,          Beta = 12,    Stack = 29  },
            new PDARule { Alpha = 28,   LabelCode = LESS_THAN,          Beta = 12,    Stack = 29  },
            new PDARule { Alpha = 28,   LabelCode = EQUAL,              Beta = 12,    Stack = 29  },
            new PDARule { Alpha = 28,   LabelCode = NOT_EQUAL,          Beta = 12,    Stack = 29  },
            new PDARule { Alpha = 29,   LabelCode = AND,                Beta = 27,                },
            new PDARule { Alpha = 29,   LabelCode = OR,                 Beta = 27,                },
            new PDARule { Alpha = 30,   LabelCode = RIGHT_PARENTHESIS,  Beta = 29,                },
            new PDARule { Alpha = 31,   LabelCode = THEN,               Beta = 8,                 },
        };

        private static Dictionary<int, Func<int, int>> inComparisionTable = new Dictionary<int, Func<int, int>>
        {
            [1] = x => ThrowError("program", x),
            [2] = x => ThrowError("id", x),
            [3] = x => ThrowError("{", x),
            [4] = x => ThrowError("}", x),
            [5] = x => ThrowError("no code", x),
            [6] = x => ThrowError("operator", x),
            [7] = x => ThrowError(";", x),
            [8] = x => bubbleStack.Pop(),
            [9] = x => ThrowError("id", x),
            [10] = x => bubbleStack.Pop(),
            [11] = x => ThrowError("=", x),
            [12] = x => ThrowError("operand", x),
            [13] = x => ThrowError(")", x),
            [14] = x => bubbleStack.Pop(),
            [15] = x => ThrowError("(", x),
            [16] = x => ThrowError("id", x),
            [17] = x => ThrowError(")", x),
            [18] = x => ThrowError("(", x),
            [19] = x => ThrowError("id", x),
            [20] = x => ThrowError("id", x),
            [21] = x => ThrowError("to", x),
            [22] = x => ThrowError("by", x),
            [23] = x => ThrowError("while", x),
            [24] = x => ThrowError("(", x),
            [25] = x => ThrowError(")", x),
            [26] = x => ThrowError("end", x),
            [27] = x => ThrowError("logical expression", x),
            [28] = x => bubbleStack.Pop(),
            [29] = x => bubbleStack.Pop(),
            [30] = x => bubbleStack.Pop(),
            [31] = x => bubbleStack.Pop(),
        };

        private static int ThrowError(string error, int line)
        {
            throw new InvalidOperationException($"Expected {error} on {line} line");
        }

        public class PDARule
        {
            public int Alpha { get; set; }
            public LexemeCodes LabelCode { get; set; }
            public int Beta { get; set; }
            public int? Stack { get; set; }
        }
    }
}