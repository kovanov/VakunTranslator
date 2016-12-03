using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VakunTranslatorVol2.Analyzers;

namespace VakunTranslatorVol2
{
    public partial class PDAOutputForm : Form
    {
        public PDAOutputForm()
        {
            InitializeComponent();
        }

        public void SetRules(List<PDASyntaxAnalyzer.UsedRule> rules)
        {
            dataGridView1.DataSource = rules.Select(x => new
            {
                Alpha = x.Rule.Alpha,
                LexemeCode = x.Rule.LexemeCode,
                Beta = x.Rule.Beta,
                Stack = x.Rule.Stack,
                OnComparisionFault = x.Rule.OnComparisionFault,
                OnComparisionSuccess = x.Rule.OnComparisionSuccess,
                StackState = x.StackState
            }).ToList();
        }
    }
}
