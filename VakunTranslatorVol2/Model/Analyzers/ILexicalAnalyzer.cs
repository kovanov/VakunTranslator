using System.Collections.Generic;

namespace VakunTranslatorVol2.Model.Analyzers
{
    public interface ILexicalAnalyzer:IAnalyzer
    {
        List<Lexeme> Constants { get; }
        List<Lexeme> Identificators { get; }
        List<Lexeme> AllLexemes { get; }
        List<Lexeme> Analyze(string sourceCode);
    }
}