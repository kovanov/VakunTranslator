using System;
using System.Collections.Generic;
using System.Linq;

namespace VakunTranslatorVol2.Model
{
    public class GrammarChecker
    {
        public string[,] PrecedenceTable { get; private set; }
        public Dictionary<string, string[][]> Map { get; private set; }
        public string[,] Check(string grammar)
        {
            Map = BuildMap(grammar);
            var terminals = GetTerminals();

            CheckForUndefinedTerminals();

            var lexemesMap = BuildLexemesMap(terminals);
            PrecedenceTable = BuildPrecedenceTable(lexemesMap);
            var firstMap = GetFirstMap(lexemesMap);
            var lastMap = GetLastMap(lexemesMap);

            FillEquals(lexemesMap);
            FillLess(lexemesMap, firstMap);
            FillMore(lexemesMap, firstMap, lastMap);
            FillSharp();
            FillEmpties();

            return PrecedenceTable;
        }

        private void FillEmpties()
        {
            for(int i = 0; i < PrecedenceTable.GetLength(0); i++)
            {
                for(int j = 0; j < PrecedenceTable.GetLength(1); j++)
                {
                    if(string.IsNullOrEmpty(PrecedenceTable[i, j]))
                    {
                        PrecedenceTable[i, j] = "-";
                    }
                }
            }
        }
        private void FillSharp()
        {
            var length = PrecedenceTable.GetLength(0) - 1;
            for(int i = 1; i < length; i++)
            {
                PrecedenceTable[length, i] = "<";
                PrecedenceTable[i, length] = ">";
            }
        }
        private void FillMore(List<string> lexemesMap, Dictionary<string, string[]> firstMap, Dictionary<string, string[]> lastMap)
        {
            for(int i = 1; i < PrecedenceTable.GetLength(0); i++)
            {
                for(int j = 1; j < PrecedenceTable.GetLength(1); j++)
                {
                    if(PrecedenceTable[i, j] == "=" && lastMap.ContainsKey(PrecedenceTable[i, 0]))
                    {
                        if(IsNonTerminal(PrecedenceTable[0, j]))
                        {
                            foreach(var lexeme in lastMap[PrecedenceTable[i, 0]])
                            {
                                foreach(var lexemeS in firstMap[PrecedenceTable[0, j]])
                                {
                                    var lexemePosition = lexemesMap.IndexOf(lexeme) + 1;
                                    var lexemeSPosition = lexemesMap.IndexOf(lexemeS) + 1;

                                    if(string.IsNullOrEmpty(PrecedenceTable[lexemePosition, lexemeSPosition]) || PrecedenceTable[lexemePosition, lexemeSPosition] == ">")
                                    {
                                        PrecedenceTable[lexemePosition, lexemeSPosition] = ">";
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"Конфлікт >. Відношення ({PrecedenceTable[lexemePosition, 0]} i {PrecedenceTable[0, lexemeSPosition]}) уже існує {PrecedenceTable[lexemePosition, j]}");
                                    }
                                }
                            }
                        }
                        foreach(var lexeme in lastMap[PrecedenceTable[i, 0]])
                        {
                            var lexemePosition = lexemesMap.IndexOf(lexeme) + 1;
                            if(string.IsNullOrEmpty(PrecedenceTable[lexemePosition, j]) || ">".Equals(PrecedenceTable[lexemePosition, j]))
                            {
                                PrecedenceTable[lexemePosition, j] = ">";
                            }
                            else
                            {
                                throw new ArgumentException($"Конфлікт >. Відношення ({PrecedenceTable[lexemePosition, 0]} i {PrecedenceTable[0, j]}) уже існує {PrecedenceTable[lexemePosition, j]}");
                            }
                        }
                    }
                }
            }
        }
        private void FillLess(List<string> lexemesMap, Dictionary<string, string[]> firstMap)
        {
            for(int i = 1; i < PrecedenceTable.GetLength(0); i++)
            {
                for(int j = 1; j < PrecedenceTable.GetLength(1); j++)
                {
                    if(PrecedenceTable[i, j] == "=" && firstMap.ContainsKey(PrecedenceTable[0, j]))
                    {
                        foreach(var lexeme in firstMap[PrecedenceTable[0, j]])
                        {
                            var lexemePosition = lexemesMap.IndexOf(lexeme) + 1;

                            if(string.IsNullOrEmpty(PrecedenceTable[i, lexemePosition]) || PrecedenceTable[i, lexemePosition] == "<")
                            {
                                PrecedenceTable[i, lexemePosition] = "<";
                            }
                            else
                            {
                                throw new ArgumentException($"Конфлікт <. Відношення ({PrecedenceTable[i, 0]} i { PrecedenceTable[0, lexemePosition] }) уже існує {PrecedenceTable[i, lexemePosition]}");
                            }
                        }
                    }
                }
            }
        }
        private void FillEquals(List<string> lexemesMap)
        {
            foreach(var line in Map.Values)
            {
                foreach(var lexemes in line)
                {
                    for(int i = 0; i < lexemes.Length - 1; i++)
                    {
                        PrecedenceTable[lexemesMap.IndexOf(lexemes[i]) + 1, lexemesMap.IndexOf(lexemes[i + 1]) + 1] = "=";
                    }
                }
            }
        }

        private string[] GetPlus(string parent, Func<string[], string> selector)
        {
            Stack<string> queue = new Stack<string>();
            List<string> result = new List<string>();

            queue.Push(parent);

            while(queue.Any())
            {
                string lexeme = queue.Pop();

                foreach(var line in Map[lexeme])
                {
                    var item = selector(line);
                    if(!result.Contains(item))
                    {
                        result.Add(item);
                        if(IsNonTerminal(item))
                        {
                            queue.Push(item);
                        }
                    }
                }
            }

            return result.ToArray();
        }
        private string[] GetLastPlus(string parent)
        {
            return GetPlus(parent, x => x.Last());
        }
        private string[] GetFirstPlus(string parent)
        {
            return GetPlus(parent, x => x.First());
        }

        private Dictionary<string, string[]> GetMap(List<string> lexemesMap, Func<string, string[]> selector)
        {
            return lexemesMap.Where(IsNonTerminal).ToDictionary(x => x, selector);
        }
        private Dictionary<string, string[]> GetFirstMap(List<string> lexemesMap)
        {
            return GetMap(lexemesMap, GetFirstPlus);
        }
        private Dictionary<string, string[]> GetLastMap(List<string> lexemesMap)
        {
            return GetMap(lexemesMap, GetLastPlus);
        }

        private string[,] BuildPrecedenceTable(List<string> lexemesMap)
        {
            var tableSize = lexemesMap.Count + 1;
            var table = new string[tableSize, tableSize];

            for(var i = 0; i < lexemesMap.Count; i++)
            {
                table[i + 1, 0] = lexemesMap[i];
                table[0, i + 1] = lexemesMap[i];
            }

            return table;
        }
        private List<string> BuildLexemesMap(string[] terminals)
        {
            return Map.Keys
                .Union(terminals)
                .Union(new[] { "#" })
                .ToList();
        }
        private Dictionary<string, string[][]> BuildMap(string grammar)
        {
            return grammar
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(new[] { "::=" }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => new
                    {
                        Head = x.First(),
                        Tail = x.Last().Split('#')
                    })
                    .Select(x => new
                    {
                        Head = x.Head,
                        Tail = x.Tail.Select(y => y.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)).ToArray()
                    })
                    .ToDictionary(x => x.Head, x => x.Tail);
        }

        private string[] GetTerminals()
        {
            return Map.Values
                .SelectMany(x => x)
                .SelectMany(x => x)
                .Union(Map.Keys)
                .Where(x => !IsNonTerminal(x))
                .Distinct()
                .ToArray();
        }

        private void CheckForUndefinedTerminals()
        {
            var unknownWord = Map
                        .SelectMany(x => x.Value)
                        .SelectMany(x => x)
                        .FirstOrDefault(x => !Map.ContainsKey(x) && IsNonTerminal(x));

            if(!string.IsNullOrEmpty(unknownWord))
            {
                throw new ArgumentException($"Can't find {unknownWord} definition");
            }
        }


        private bool IsNonTerminal(string str)
        {
            return str.StartsWith("<") && str.EndsWith(">");
        }
    }
}