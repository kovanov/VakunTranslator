using System.Collections.Generic;
using System.Drawing;
using VakunTranslatorVol2.Model;

namespace VakunTranslatorVol2.CodeHighlight
{
    public class LexemePainter : IPainter<Lexeme>
    {
        public Color DefaultColor { get; } = Color.FromArgb(170, 170, 170);

        private readonly Color _blue = Color.FromArgb(50, 120, 230);
        private readonly Color _orange = Color.FromArgb(230, 120, 20);
        private readonly Color _red = Color.FromArgb(230, 100, 100);

        public Color GetColor(Lexeme lexeme)
        {
            if (lexeme.Is(LexemeFlags.Reserved))
            {
                return _blue;
            }
            if (lexeme.Is(LexemeFlags.Const))
            {
                return _red;
            }
            if (lexeme.Is(LexemeFlags.TypeDefinition))
            {
                return _orange;
            }

            return DefaultColor;
        }
    }
}