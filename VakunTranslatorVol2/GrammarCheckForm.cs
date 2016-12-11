using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VakunTranslatorVol2
{
    public partial class GrammarCheckForm : Form
    {
        private string[,] table;

        public GrammarCheckForm(string[,] table)
        {
            InitializeComponent();
            Shown += GrammarCheckForm_Shown;
            this.table = table;
        }

        private void GrammarCheckForm_Shown(object sender, EventArgs e)
        {
            var tableWidth = table.GetLength(0);

            dataGridView1.ColumnCount = tableWidth;

            for(int i = 0; i < tableWidth; i++)
            {
                dataGridView1.Rows.Add(GetTableRow(i).ToArray());
            }
        }

        private IEnumerable<string> GetTableRow(int row)
        {
            var tableWidth = table.GetLength(0);

            for(int i = 0; i < tableWidth; i++)
            {
                yield return table[row, i];
            }
        }
    }
}
