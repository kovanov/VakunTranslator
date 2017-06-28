using System;
using System.Collections.Generic;
using VakunTranslatorVol2.CodeHighlight;
using VakunTranslatorVol2.Model;
using VakunTranslatorVol2.Model.Analyzers;

namespace VakunTranslatorVol2.Views
{
    public interface IMainForm
    {
        event Action<string> SourceCodeAnalyzeRequired;
        event Action<string> SaveFileAsClick;
        event Action<string> SaveFileClick;
        event Action OpenSourceCodeFileClick;
        event Action GrammarAnalyzeRequired;
        event Action NewFileClick;
        event Action BuildRequired;

        void WriteConsole(string message);
        void DisplayConstants<T>(IEnumerable<T> source);
        void DisplayIdentificators<T>(IEnumerable<T> source);
        void DisplayLexemes<T>(IEnumerable<T> source);
        void WriteConsole(IEnumerable<string> lines);
        void SetColorizer(Colorizer<Lexeme> colorizer);
        void SetSourceCode(string text);
        void ClearConsole();
        void HideConsole();
        void SetPDAOutput(List<PDASyntaxAnalyzer.UsedRule> rules);
        void ShowConsole();
        void HighlightSourceCode(IEnumerable<Lexeme> lexemes);
        void ShowGrammarError(string message);
        void ShowGrammarTable(string[,] table);
        void SetAAOutput(List<AscendingSyntaxAnalyzer.AscendingInfo> info);
        void RunProgram(List<string> poliz);
        void EnableRunButton();
        void DisableRunButton();
        void SetPoliz(List<string> poliz);
    }
}