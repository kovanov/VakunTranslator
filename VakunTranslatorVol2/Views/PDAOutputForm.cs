using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using VakunTranslatorVol2.Model.Analyzers;

namespace VakunTranslatorVol2.Views
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
