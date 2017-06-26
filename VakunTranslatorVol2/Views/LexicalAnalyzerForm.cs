using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VakunTranslatorVol2.Views
{
    public partial class LexicalAnalyzerForm : Form
    {
        public event Action<string, int> LexemSelected;
        public LexicalAnalyzerForm()
        {
            InitializeComponent();
        }

        public void DisplayConstants<T>(IEnumerable<T> source)
        {
            constGrid.DataSource = source.ToArray();
        }
        public void DisplayIdentificators<T>(IEnumerable<T> source)
        {
            idGrid.DataSource = source.ToArray();
        }
        public void DisplayLexemes<T>(IEnumerable<T> source)
        {
            lexemeGrid.DataSource = source.ToArray();
        }

        private void AnalyzerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void lexemeGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var cells = lexemeGrid.CurrentCell.OwningRow.Cells;

            var lexemeBody = cells[0].Value.ToString();
            var lexemeRow = int.Parse(cells[2].Value.ToString());

            LexemSelected?.Invoke(lexemeBody, lexemeRow);
        }
    }
}
