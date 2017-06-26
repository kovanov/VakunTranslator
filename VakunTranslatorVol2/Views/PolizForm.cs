using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VakunTranslatorVol2.Views
{
    public partial class PolizForm : Form
    {
        public PolizForm()
        {
            InitializeComponent();
        }

        public void SetPoliz(string poliz)
        {
            textBox1.Text = poliz;
        }
    }
}
