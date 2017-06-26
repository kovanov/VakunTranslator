using System.Windows.Forms;

namespace VakunTranslatorVol2.CustomFace
{
    public class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        public CustomToolStripRenderer() : base(new CustomColorTable()) { }
    }
}
