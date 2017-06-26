using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakunTranslatorVol2.Model.Analyzers
{
    public abstract class SyntaxAnalyzer : ISyntaxAnalyzer
    {
        public List<string> Errors { get; } = new List<string>();
        public bool HasErrors
        {
            get { return Errors.Any(); }
        }

        public abstract void Analyze(List<Lexeme> lexemes);

        public abstract void Dispose();
    }
}
