using System;
using System.Collections.Generic;
using System.Threading;

namespace VakunTranslatorVol2
{
    public interface IView
    {
        event Action<string> AnalyzeRequired;
        event Action NewFileClick;
        event Action OpenFileClick;
        event Action<string> SaveFileClick;
        event Action<string> SaveFileAsClick;

        void WriteConsole(string message);
        void DisplayConstants<T>(IEnumerable<T> source);
        void DisplayIdentificators<T>(IEnumerable<T> source);
        void DisplayLexemes<T>(IEnumerable<T> source);
        void WriteConsole(IEnumerable<string> lines);
        void SetColorizer(Colorizer<Lexeme> colorizer);
        void SetSourceCode(string text);
        void ClearConsole();
        void HideConsole();
        void ShowConsole();
        void HighlightSourceCode(IEnumerable<Lexeme> lexemes, CancellationToken token);
    }
}