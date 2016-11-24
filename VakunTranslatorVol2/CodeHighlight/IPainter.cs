using System.Drawing;

namespace VakunTranslatorVol2
{
    public interface IPainter<T>
    {
        Color DefaultColor { get; }

        Color GetColor(T obj);
    }
}