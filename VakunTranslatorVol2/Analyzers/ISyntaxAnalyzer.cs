using System.Collections.Generic;
using System.Threading;

namespace VakunTranslatorVol2.Analyzers
{
    public interface ISyntaxAnalyzer:IAnalyzer
    {
        void Analyze(List<Lexeme> lexemes);
    }
}