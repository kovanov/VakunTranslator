using System;
using System.Collections.Generic;
using System.Linq;

namespace VakunTranslatorVol2
{
    public class GrammarChecker
    {
        private Dictionary<string, string[][]> map = new Dictionary<string, string[][]>();
        public string[,] Check(string grammar)
        {
            map = BuildMap(grammar);
            var terminals = GetTerminals();

            CheckForUndefinedTerminals();

            var lexemesMap = BuildLexemesMap(terminals);
            var precedenceTable = BuildPrecedenceTable(lexemesMap);
            var firstMap = GetFirstMap(lexemesMap);
            var lastMap = GetLastMap(lexemesMap);

            FillEquals(precedenceTable, lexemesMap);
            FillLess(precedenceTable, lexemesMap, firstMap);
            FillMore(precedenceTable, lexemesMap, firstMap, lastMap);
            FillSharp(precedenceTable);
            FillEmpties(precedenceTable);

            return precedenceTable;
        }

        private void FillEmpties(string[,] precedenceTable)
        {
            for(int i = 0; i < precedenceTable.GetLength(0); i++)
            {
                for(int j = 0; j < precedenceTable.GetLength(1); j++)
                {
                    if(string.IsNullOrEmpty(precedenceTable[i, j]))
                    {
                        precedenceTable[i, j] = "-";
                    }
                }
            }
        }

        private void FillSharp(string[,] precedenceTable)
        {
            var length = precedenceTable.GetLength(0) - 1;
            for(int i = 1; i < length; i++)
            {
                precedenceTable[length, i] = "<";
                precedenceTable[i, length] = ">";
            }
        }

        private void FillMore(string[,] precedenceTable, List<string> lexemesMap, Dictionary<string, string[]> firstMap, Dictionary<string, string[]> lastMap)
        {
            for(int i = 1; i < precedenceTable.GetLength(0); i++)
            {
                for(int j = 1; j < precedenceTable.GetLength(1); j++)
                {
                    if("=".Equals(precedenceTable[i, j]))
                    {
                        if(lastMap.ContainsKey(precedenceTable[i, 0]))
                        {
                            if(IsNonTerminal(precedenceTable[0, j]))
                            {
                                foreach(var lexeme in lastMap[precedenceTable[i, 0]])
                                {
                                    foreach(var lexemeS in firstMap[precedenceTable[0, j]])
                                    {
                                        var lexemePosition = lexemesMap.IndexOf(lexeme) + 1;
                                        var lexemeSPosition = lexemesMap.IndexOf(lexemeS) + 1;
                                        if(string.IsNullOrEmpty(precedenceTable[lexemePosition, lexemeSPosition]) || ">".Equals(precedenceTable[lexemePosition, lexemeSPosition]))
                                        {
                                            precedenceTable[lexemePosition, lexemeSPosition] = ">";
                                        }
                                        else
                                        {
                                            throw new ArgumentException($"Конфлікт >. Відношення ({precedenceTable[lexemePosition, 0]} i {precedenceTable[0, lexemeSPosition]}) уже існує {precedenceTable[lexemePosition, j]}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach(var lexeme in lastMap[precedenceTable[i, 0]])
                                {
                                    var lexemePosition = lexemesMap.IndexOf(lexeme) + 1;
                                    if(string.IsNullOrEmpty(precedenceTable[lexemePosition, j]) || ">".Equals(precedenceTable[lexemePosition, j]))
                                    {
                                        precedenceTable[lexemePosition, j] = ">";
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"Конфлікт >. Відношення ({precedenceTable[lexemePosition, 0]} i {precedenceTable[0, j]}) уже існує {precedenceTable[lexemePosition, j]}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillLess(string[,] precedenceTable, List<string> lexemesMap, Dictionary<string, string[]> firstMap)
        {
            for(int i = 1; i < precedenceTable.GetLength(0); i++)
            {
                for(int j = 1; j < precedenceTable.GetLength(1); j++)
                {
                    if("=".Equals(precedenceTable[i, j]))
                    {
                        if(firstMap.ContainsKey(precedenceTable[0, j]))
                        {
                            foreach(var lexeme in firstMap[precedenceTable[0, j]])
                            {
                                var lexemePosition = lexemesMap.IndexOf(lexeme) + 1;
                                if(string.IsNullOrEmpty(precedenceTable[i, lexemePosition]) || "<".Equals(precedenceTable[i, lexemePosition]))
                                {
                                    precedenceTable[i, lexemePosition] = "<";
                                }
                                else
                                {
                                    throw new ArgumentException($"Конфлікт <. Відношення ({precedenceTable[i, 0]} i { precedenceTable[0, lexemePosition] }) уже існує {precedenceTable[i, lexemePosition]}");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillEquals(string[,] precedenceTable, List<string> lexemesMap)
        {
            foreach(var line in map.Values)
            {
                foreach(var lexemes in line)
                {
                    for(int i = 0; i < lexemes.Length - 1; i++)
                    {
                        precedenceTable[lexemesMap.IndexOf(lexemes[i]) + 1, lexemesMap.IndexOf(lexemes[i + 1]) + 1] = "=";
                    }
                }
            }
        }

        private Dictionary<string, string[]> GetLastMap(List<string> lexemesMap)
        {
            return lexemesMap.Where(IsNonTerminal)
                .ToDictionary(x => x, GetLastPlus);
        }

        private string[] GetLastPlus(string parent)
        {
            Stack<string> queue = new Stack<string>();
            List<string> lasts = new List<string>();
            string lexeme;
            queue.Push(parent);
            while(queue.Any())
            {
                lexeme = queue.Pop();

                foreach(var line in map[lexeme])
                {
                    var lastInLine = line.Last();
                    if(!lasts.Contains(lastInLine))
                    {
                        lasts.Add(lastInLine);
                        if(IsNonTerminal(lastInLine))
                        {
                            queue.Push(lastInLine);
                        }
                    }
                }
            }
            return lasts.ToArray();
        }

        private Dictionary<string, string[]> GetFirstMap(List<string> lexemesMap)
        {
            return lexemesMap.Where(IsNonTerminal)
                .ToDictionary(x => x, GetFirstPlus);
        }

        private string[] GetFirstPlus(string parent)
        {
            Stack<string> queue = new Stack<string>();
            List<string> firsts = new List<string>();
            queue.Push(parent);
            string lexeme;
            while(queue.Any())
            {
                lexeme = queue.Pop();

                foreach(var line in map[lexeme])
                {
                    var firstInLine = line.First();
                    if(!firsts.Contains(firstInLine))
                    {
                        firsts.Add(firstInLine);
                        if(IsNonTerminal(firstInLine))
                        {
                            queue.Push(firstInLine);
                        }
                    }
                }
            }
            return firsts.ToArray();
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
            return map.Keys
                .Union(terminals)
                .Union(new[] { "#" })
                .ToList();
        }

        private string[] GetTerminals()
        {
            return map.Values
                .SelectMany(x => x)
                .SelectMany(x => x)
                .Union(map.Keys)
                .Where(x => !IsNonTerminal(x))
                .Distinct()
                .ToArray();
        }

        private void CheckForUndefinedTerminals()
        {
            foreach(var pair in map)
            {
                foreach(var line in pair.Value)
                {
                    foreach(var lexeme in line)
                    {
                        if(!map.ContainsKey(lexeme) && IsNonTerminal(lexeme))
                        {
                            throw new ArgumentException($"Can't find {lexeme} definition");
                        }
                    }
                }
            }
        }

        private Dictionary<string, string[][]> BuildMap(string grammar)
        {
            return grammar.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(new[] { "::=" }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => new
                    {
                        Head = x.First(),
                        Tail = x.Last().Split('|')
                    })
                    .ToDictionary(
                        keySelector: x => x.Head,
                        elementSelector: x => x.Tail
                                                .Select(y => y.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                                                .ToArray()
                    );
        }

        private bool IsNonTerminal(string str)
        {
            return str.StartsWith("<") && str.EndsWith(">");
        }
    }
}