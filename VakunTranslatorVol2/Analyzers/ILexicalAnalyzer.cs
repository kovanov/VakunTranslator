using System.Collections.Generic;
using System.Threading;

namespace VakunTranslatorVol2.Analyzers
{
    public interface ILexicalAnalyzer:IAnalyzer
    {
        List<Lexeme> Constants { get; }
        List<Lexeme> Identificators { get; }
        List<Lexeme> AllLexemes { get; }
        List<Lexeme> Analyze(string sourceCode);
    }
}