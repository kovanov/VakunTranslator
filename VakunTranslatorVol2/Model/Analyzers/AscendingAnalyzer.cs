using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VakunTranslatorVol2.Model.Analyzers
{
    public class AscendingSyntaxAnalyzer : SyntaxAnalyzer
    {
        private Action<List<AscendingInfo>> onReady;

        public List<AscendingInfo> Info { get; private set; } = new List<AscendingInfo>();
        public GrammarChecker GrammarChecker { private get; set; }

        public AscendingSyntaxAnalyzer(GrammarChecker grammarChecker, Action<List<AscendingInfo>> onReady)
        {
            GrammarChecker = grammarChecker;
            this.onReady = onReady;
        }

        private string getStackString(Stack<string> stack)
        {
            return string.Join(" ", stack.Reverse());
        }

        private string getInputString(Stack<string> input)
        {
            return string.Join(" ", input);
        }

        private void createReversedGrammar()
        {
            reversedGrammarMap = new Dictionary<string, string>();
            foreach(var key in grammarMap.Keys)
            {
                foreach(var line in grammarMap[key])
                {
                    reversedGrammarMap.Add(string.Join(string.Empty, line), key);
                }
            }
        }

        private void findBase()
        {
            var localStack = new Stack<string>();
            var firstStackElement = stack.Pop();

            while(getRelation(stack.Peek(), firstStackElement).Equals("="))
            {
                localStack.Push(firstStackElement);
                firstStackElement = stack.Pop();
            }
            localStack.Push(firstStackElement);

            string newLexeme = string.Empty;
            while(localStack.Any())
            {
                newLexeme += localStack.Pop();
            }

            if(reversedGrammarMap.ContainsKey(newLexeme.ToString()))
            {
                string newValue = reversedGrammarMap[newLexeme.ToString()];
                stack.Push(newValue);
            }
            else if(!newLexeme.ToString().Equals("<програма>"))
            {
                throw new ArgumentException($"Не можна перетворити ланцюжок {newLexeme.ToString()}");
            }
        }

        private string getRelation(string elem1, string elem2)
        {
            return grammarTable[getFirstElementRelation(elem1, grammarTable), getSecondElementRelation(elem2, grammarTable)];
        }

        private int getFirstElementRelation(string lexeme, string[,] table)
        {
            for(int i = 1; i < table.GetLength(0); i++)
            {
                if(table[i, 0].Equals(lexeme))
                {
                    return i;
                }
            }
            return -1;
        }

        private int getSecondElementRelation(string lexeme, string[,] table)
        {
            for(int i = 1; i < table.GetLength(0); i++)
            {
                if(table[0, i].Equals(lexeme))
                {
                    return i;
                }
            }
            return -1;
        }

        public override void Analyze(List<Lexeme> lexemes)
        {
            grammarTable = GrammarChecker.PrecedenceTable;
            grammarMap = GrammarChecker.Map;

            stack.Push("#");
            input = new Stack<string>();
            input.Push("#");
            foreach(var lexeme in lexemes)
            {
                input.Push(lexeme.Body);
            }
            createReversedGrammar();
            for(int i = 0; i < input.Count; i++)
            {
                string stackLexeme = stack.Peek();
                string sign = grammarTable[getFirstElementRelation(stackLexeme, grammarTable), getSecondElementRelation(input.Skip(i).First(), grammarTable)];
                if(sign.Equals("<") || sign.Equals("="))
                {
                    Info.Add(new AscendingInfo(Info.Count, getStackString(stack), sign, getInputString(input)));
                    stack.Push(input.Skip(i).First());
                    input.Pop();
                    i--;
                }
                else if(sign.Equals(">"))
                {
                    Info.Add(new AscendingInfo(Info.Count, getStackString(stack), sign, getInputString(input)));
                    findBase();
                    i--;
                }
                else if(!stackLexeme.Equals("#") && !input.Skip(i).First().Equals("#"))
                {
                    throw new ArgumentException($"Не існує відношення між {stackLexeme} та {input.Skip(i).First()}");
                }
            }
        }

        public override void Dispose()
        {
            Errors.Clear();
            stack.Clear();
            input.Clear();
            grammarMap.Clear();
            reversedGrammarMap.Clear();
        }

        private Stack<string> stack = new Stack<string>();
        private Stack<string> input = new Stack<string>();
        private Dictionary<string, string[][]> grammarMap = new Dictionary<string, string[][]>();
        private string[,] grammarTable;
        private Dictionary<string, string> reversedGrammarMap = new Dictionary<string, string>();

        public class AscendingInfo
        {
            public int Step { get; private set; }
            public string Stack { get; private set; }
            public string Sign { get; private set; }
            public string Input { get; private set; }
            public AscendingInfo(int step, string stack, string sign, string input)
            {
                Step = step;
                Stack = stack;
                Sign = sign;
                Input = input;
            }
        }
    }
}