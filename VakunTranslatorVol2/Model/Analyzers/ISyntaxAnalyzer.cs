using System.Collections.Generic;
using System.Threading;

namespace VakunTranslatorVol2.Model.Analyzers
{
    public interface ISyntaxAnalyzer:IAnalyzer
    {
        void Analyze(List<Lexeme> lexemes);
    }
}