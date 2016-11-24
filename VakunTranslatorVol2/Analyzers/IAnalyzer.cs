using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakunTranslatorVol2.Analyzers
{
    public interface IAnalyzer : IDisposable
    {
        List<string> Errors { get; }
        bool HasErrors { get; }
    }
}
