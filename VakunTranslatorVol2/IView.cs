using System;
using System.Collections.Generic;
using System.Threading;
using VakunTranslatorVol2.Analyzers;

namespace VakunTranslatorVol2
{
    public interface IView
    {
        event Action<string> SourceCodeAnalyzeRequired;
        event Action<string> SaveFileAsClick;
        event Action<string> SaveFileClick;
        event Action OpenSourceCodeFileClick;
        event Action GrammarAnalyzeRequired;
        event Action NewFileClick;

        void WriteConsole(string message);
        void DisplayConstants<T>(IEnumerable<T> source);
        void DisplayIdentificators<T>(IEnumerable<T> source);
        void DisplayLexemes<T>(IEnumerable<T> source);
        void WriteConsole(IEnumerable<string> lines);
        void SetColorizer(Colorizer<Lexeme> colorizer);
        void SetSourceCode(string text);
        void ClearConsole();
        void HideConsole();
        void SetPDAOutput(List<PDASyntaxAnalyzer.UsedRule> obj);
        void ShowConsole();
        void HighlightSourceCode(IEnumerable<Lexeme> lexemes);
        void ShowGrammarError(string message);
        void ShowGrammarTable(string[,] table);
    }
}