using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VakunTranslatorVol2.Model.Analyzers;

namespace VakunTranslatorVol2.Views
{
    public partial class AscendingAnalyzerForm : Form
    {
        public AscendingAnalyzerForm()
        {
            InitializeComponent();
        }

        public void SetOutput(List<AscendingSyntaxAnalyzer.AscendingInfo> info)
        {
            dataGridView1.DataSource = info;
        }
    }
}
