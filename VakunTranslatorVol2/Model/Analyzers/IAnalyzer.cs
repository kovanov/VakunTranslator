using System;
using System.Collections.Generic;

namespace VakunTranslatorVol2.Model.Analyzers
{
    public interface IAnalyzer : IDisposable
    {
        List<string> Errors { get; }
        bool HasErrors { get; }
    }
}
