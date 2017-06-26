using System.Drawing;
using System.Windows.Forms;

namespace VakunTranslatorVol2.CustomFace
{
    public class CustomColorTable : ProfessionalColorTable
    {
        private Color darkGrey = Color.FromArgb(20, 20, 20);

        public override Color MenuBorder
        {
            get { return darkGrey; }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return darkGrey; }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return darkGrey; }
        }

        public override Color MenuItemBorder
        {
            get { return darkGrey; }
        }
        public override Color MenuItemSelected
        {
            get { return darkGrey; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return darkGrey; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return darkGrey; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return darkGrey; }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return darkGrey; }
        }
    }
}