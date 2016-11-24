using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laba_1
{
    public class SyntaxAnalyzerPDA
    {
        private static Stack<int> jumpStack = new Stack<int>();
        public void Analyze(tab_lex_exit[] lexemes)
        {
            var currentAlpha = 1;
            foreach (var lexeme in lexemes.Skip(1))
            {
                var currentRules = PDATable.Where(x => x.Alpha == currentAlpha);
                var currentRule = currentRules.FirstOrDefault(x => x.LabelCode == lexeme.kod);

                while (currentRule == null)
                {
                    currentAlpha = inComparisionTable[currentAlpha].Invoke();
                    currentRules = PDATable.Where(x => x.Alpha == currentAlpha);
                    currentRule = currentRules.FirstOrDefault(x => x.LabelCode == lexeme.kod);
                }
                if (currentRule.Stack != null)
                {
                    jumpStack.Push((int)currentRule.Stack);
                }
                currentAlpha = currentRule.Beta();
            }
        }

        private PDARule[] PDATable = new[]
        {
            new PDARule { Alpha = 1,   LabelCode = 1,  Beta = () => 2 },
            new PDARule { Alpha = 2, LabelCode = 31, Beta = () => 3 },
            new PDARule { Alpha = 3, LabelCode = 2, Beta =() =>4 },
            new PDARule { Alpha = 4, LabelCode = 3, Beta =() => 5 },
            new PDARule { Alpha = 5, LabelCode = 31, Beta = () => 6 },
            new PDARule { Alpha = 6, LabelCode = 13, Beta = () => 5 },
            new PDARule { Alpha = 6, LabelCode = 12, Beta = () => 100 },
            new PDARule { Alpha = 100, LabelCode = 8, Beta = () => 10,               Stack = 7 },
            new PDARule { Alpha = 100, LabelCode = 3, Beta = () => 4 },
            new PDARule { Alpha = 7, LabelCode = 12, Beta = () => 8 },
            new PDARule { Alpha = 8,   LabelCode = 9,  Beta = () => 10,               Stack = 7  },//?
            new PDARule { Alpha = 10, LabelCode = 5, Beta = () => 11 },
            new PDARule { Alpha = 10, LabelCode = 4, Beta = () => 14 },
            new PDARule { Alpha = 10, LabelCode = 6, Beta = () => 37,              Stack = 170},
            new PDARule { Alpha = 10, LabelCode = 7, Beta = () => 20 },
            new PDARule { Alpha = 10, LabelCode = 31, Beta = () => 28 },
            new PDARule { Alpha = 11, LabelCode = 21, Beta = () => 12 },//?
            new PDARule { Alpha = 12, LabelCode = 31, Beta = () => 13 },
            new PDARule { Alpha = 13, LabelCode = 21, Beta = () => 12 },
            new PDARule { Alpha = 14, LabelCode = 20, Beta = () => 15 },
            new PDARule { Alpha = 15, LabelCode = 31, Beta = () => 16 },
            new PDARule { Alpha = 16, LabelCode = 20, Beta = () => 15 },
            new PDARule { Alpha = 170, LabelCode = 22, Beta = () => 37,            Stack = 17 },
            new PDARule { Alpha = 170, LabelCode = 23, Beta = () => 37,            Stack = 17 },
            new PDARule { Alpha = 170, LabelCode = 24, Beta = () => 37,            Stack = 17 },
            new PDARule { Alpha = 170, LabelCode = 25, Beta = () => 37,            Stack = 17 },
            new PDARule { Alpha = 170, LabelCode = 26, Beta = () => 37,            Stack = 17 },
            new PDARule { Alpha = 170, LabelCode = 27, Beta = () => 37,            Stack = 17 },
            new PDARule { Alpha = 17, LabelCode = 8, Beta = () => 10,              Stack = 18 },
            new PDARule { Alpha = 18, LabelCode = 12, Beta = () => 19 },
            new PDARule { Alpha = 19, LabelCode = 9, Beta = () => 10,              Stack = 18 },
            new PDARule { Alpha = 20, LabelCode = 10, Beta = () => 21 },
            new PDARule { Alpha = 21, LabelCode = 31, Beta = () => 22 },
            new PDARule { Alpha = 22, LabelCode = 19, Beta = () => 37,              Stack = 23},
            new PDARule { Alpha = 23, LabelCode = 12, Beta = () => 37,              Stack = 230},
            new PDARule { Alpha = 230, LabelCode = 22, Beta = () => 37,            Stack = 24 },
            new PDARule { Alpha = 230, LabelCode = 23, Beta = () => 37,            Stack = 24 },
            new PDARule { Alpha = 230, LabelCode = 24, Beta = () => 37,            Stack = 24 },
            new PDARule { Alpha = 230, LabelCode = 25, Beta = () => 37,            Stack = 24 },
            new PDARule { Alpha = 230, LabelCode = 26, Beta = () => 37,            Stack = 24 },
            new PDARule { Alpha = 230, LabelCode = 27, Beta = () => 37,            Stack = 24 },
            new PDARule { Alpha = 24, LabelCode = 12, Beta = () => 101 },
            new PDARule { Alpha = 101, LabelCode = 31, Beta = () => 25 },
            new PDARule { Alpha = 25, LabelCode = 19, Beta = () => 37,             Stack = 26 },
            new PDARule { Alpha = 26, LabelCode = 11, Beta = () => 10,             Stack = 27 },
            new PDARule { Alpha = 27,  Beta = () => jumpStack.Pop() },//?
            new PDARule { Alpha = 28, LabelCode = 19, Beta = () => 37,             Stack = 29 },
            new PDARule { Alpha = 29, LabelCode = 22, Beta = () => 37,             Stack = 30 },
            new PDARule { Alpha = 29, LabelCode = 23, Beta = () => 37,             Stack = 30 },
            new PDARule { Alpha = 29, LabelCode = 24, Beta = () => 37,             Stack = 30 },
            new PDARule { Alpha = 29, LabelCode = 25, Beta = () => 37,             Stack = 30 },
            new PDARule { Alpha = 29, LabelCode = 26, Beta = () => 37,             Stack = 30 },
            new PDARule { Alpha = 29, LabelCode = 27, Beta = () => 37,             Stack = 30 },
            new PDARule { Alpha = 30, LabelCode = 28, Beta = () => 37,             Stack = 31 },
            new PDARule { Alpha = 31, LabelCode = 29, Beta = () => 37,             Stack = 32 },
            new PDARule { Alpha = 32, Beta = () => jumpStack.Pop()  },
            new PDARule { Alpha = 37, LabelCode = 15, Beta = () => 8 },
            new PDARule { Alpha = 37, LabelCode = 10, Beta = () => 37,             Stack = 39 },
            new PDARule { Alpha = 37, LabelCode = 31, Beta = () => 40 },
            new PDARule { Alpha = 37, LabelCode = 30, Beta = () => 40 },
            new PDARule { Alpha = 38, LabelCode = 31, Beta = () => 40 },
            new PDARule { Alpha = 38, LabelCode = 30, Beta = () => 40 },
            new PDARule { Alpha = 38, LabelCode = 10, Beta = () => 37,             Stack = 39 },
            new PDARule { Alpha = 39, LabelCode = 11, Beta = () => 40 },
            new PDARule { Alpha = 40, LabelCode = 14, Beta = () => 38 },
            new PDARule { Alpha = 40, LabelCode = 15, Beta = () => 38 },
            new PDARule { Alpha = 40, LabelCode = 16, Beta = () => 38 },
            new PDARule { Alpha = 40, LabelCode = 17, Beta = () => 38 },
            new PDARule { Alpha = 40, LabelCode = 18, Beta = () => 38 }
        };

        private Dictionary<int, Func<int>> inComparisionTable = new Dictionary<int, Func<int>>
        {
            [1] = () => { throw new ArgumentException("error "); },
            [2] = () => { throw new ArgumentException("error "); },
            [3] = () => { throw new ArgumentException("error "); },
            [4] = () => { throw new ArgumentException("error "); },
            [5] = () => { throw new ArgumentException("error "); },
            [6] = () => { throw new ArgumentException("error "); },
            [100] = () => { throw new ArgumentException("error "); },
            [7] = () => { throw new ArgumentException("error "); },
            [8] = () => 10,
            [10] = () => { throw new ArgumentException("error "); },
            [11] = () => { throw new ArgumentException("error "); },
            [12] = () => { throw new ArgumentException("error "); },
            [13] = () => jumpStack.Pop(),
            [14] = () => { throw new ArgumentException("error "); },
            [15] = () => { throw new ArgumentException("error "); },
            [16] = () => jumpStack.Pop(),
            [170] = () => { throw new ArgumentException("error "); },
            [17] = () => { throw new ArgumentException("error "); },
            [18] = () => { throw new ArgumentException("error "); },
            [19] = () => 18,
            [20] = () => { throw new ArgumentException("error "); },
            [21] = () => { throw new ArgumentException("error "); },
            [22] = () => { throw new ArgumentException("error "); },
            [23] = () => { throw new ArgumentException("error "); },
            [230] = () => { throw new ArgumentException("error "); },
            [24] = () => { throw new ArgumentException("error "); },
            [101] = () => { throw new ArgumentException("error "); },
            [25] = () => { throw new ArgumentException("error "); },
            [26] = () => { throw new ArgumentException("error "); },
            [27] = () => jumpStack.Pop(),
            [28] = () => { throw new ArgumentException("error "); },
            [29] = () => jumpStack.Pop(),
            [30] = () => { throw new ArgumentException("error "); },
            [31] = () => { throw new ArgumentException("error "); },
            [32] = () => jumpStack.Pop(),
            [37] = () => { throw new ArgumentException("error "); },
            [38] = () => { throw new ArgumentException("error "); },
            [39] = () => { throw new ArgumentException("error "); },
            [40] = () => jumpStack.Pop()
        };
    }
}