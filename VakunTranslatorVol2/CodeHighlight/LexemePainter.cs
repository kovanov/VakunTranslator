using System.Collections.Generic;
using System.Drawing;

namespace VakunTranslatorVol2
{
    public class LexemePainter : IPainter<Lexeme>
    {
        public Color DefaultColor { get; } = Color.FromArgb(170, 170, 170);

        public Color GetColor(Lexeme lexeme)
        {
            return colorTable[lexeme.Flags];
        }

        private static Dictionary<LexemeFlags, Color> colorTable = new Dictionary<LexemeFlags, Color>
        {
            [LexemeFlags.Reserved] = Color.FromArgb(50, 120, 230),
            [LexemeFlags.TypeDefinition] = Color.FromArgb(230, 120, 20),
            [LexemeFlags.Const] = Color.FromArgb(230, 100, 100)
        };
    }
}