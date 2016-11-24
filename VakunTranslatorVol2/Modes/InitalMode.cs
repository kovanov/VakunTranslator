using System.Collections.Generic;
using System.Linq;
using VakunTranslatorVol2.Extensions;

namespace VakunTranslatorVol2.Modes
{
    public class InitalMode : Mode
    {
        public InitalMode()
        {
            NextModes = Enumerable.Range(0, 14).ToArray();
        }

        public override Mode MoveNext(char c)
        {
            if(c.IsNewLine())
            {
                OnNewLine();
                return this;
            }

            return base.MoveNext(c);
        }
    }
}