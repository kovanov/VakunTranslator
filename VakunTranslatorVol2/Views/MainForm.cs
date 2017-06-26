using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using VakunTranslatorVol2.CodeHighlight;
using VakunTranslatorVol2.CustomFace;
using VakunTranslatorVol2.Model;
using VakunTranslatorVol2.Model.Analyzers;

namespace VakunTranslatorVol2.Views
{
    public partial class MainForm : Form, IMainForm
    {
        public MainForm()
        {
            InitializeComponent();
            menuStrip1.Renderer = new CustomToolStripRenderer();
            sourceBox.SelectionTabs = Enumerable.Range(1, 10).Select(x => x * 14).ToArray();
        }

        #region IMainForm implementation
        public event Action<string> SourceCodeAnalyzeRequired;
        public event Action GrammarAnalyzeRequired;
        public event Action NewFileClick;
        public event Action OpenSourceCodeFileClick;
        public event Action<string> SaveFileClick;
        public event Action<string> SaveFileAsClick;
        public event Action BuildRequired;

        public void ClearConsole()
        {
            console.Clear();
        }
        public void DisplayConstants<T>(IEnumerable<T> source)
        {
            LAForm.DisplayConstants(source);
        }
        public void DisplayIdentificators<T>(IEnumerable<T> source)
        {
            LAForm.DisplayIdentificators(source);
        }
        public void DisplayLexemes<T>(IEnumerable<T> source)
        {
            LAForm.DisplayLexemes(source);
        }
        public void HighlightSourceCode(IEnumerable<Lexeme> source)
        {
            colorizer?.Highlight(sourceBox, source);
        }
        public void WriteConsole(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                WriteConsole(line);
            }
        }
        public void WriteConsole(string message)
        {
            console.AppendText(message);
            console.AppendText(Environment.NewLine);
        }
        public void SetSourceCode(string text)
        {
            sourceBox.Text = text;
        }
        public void HideConsole()
        {
            tableLayoutPanel1.RowStyles[1].Height = 0;
        }
        public void ShowConsole()
        {
            tableLayoutPanel1.RowStyles[1].Height = CONSOLE_HEIGHT;
        }
        public void SetColorizer(Colorizer<Lexeme> colorizer)
        {
            this.colorizer = colorizer;
        }
        public void SetPDAOutput(List<PDASyntaxAnalyzer.UsedRule> rules)
        {
            PdaForm.SetRules(rules);
        }
        public void SetAAOutput(List<AscendingSyntaxAnalyzer.AscendingInfo> info)
        {
            AAForm.SetOutput(info);
        }
        public void ShowGrammarError(string message)
        {
            MessageBox.Show(message);
        }
        public void ShowGrammarTable(string[,] table)
        {
            var grammarForm = new GrammarCheckForm(table);
            grammarForm.ShowDialog();
        }
        public void RunProgram(List<string> poliz)
        {
            polizForm.SetPoliz(string.Join(" ", poliz));
            new ExecutingForm(poliz).ShowDialog();
        }
        public void EnableRunButton()
        {
            if (!runEnabled)
            {
                rUNToolStripMenuItem.ForeColor = System.Drawing.Color.DarkGreen;
                rUNToolStripMenuItem.Click += rUNToolStripMenuItem_Click;
                rUNToolStripMenuItem.Click -= DisabledRun_Click;
                runEnabled = true;
            }
        }
        public void DisableRunButton()
        {
            if (runEnabled)
            {
                rUNToolStripMenuItem.ForeColor = System.Drawing.Color.DarkRed;
                rUNToolStripMenuItem.Click -= rUNToolStripMenuItem_Click;
                rUNToolStripMenuItem.Click += DisabledRun_Click;
                runEnabled = false;
            }
        }
        #endregion

        #region Control EventHandlers
        private void sourceBox_TextChanged(object sender, EventArgs e)
        {
            var source = (sender as RichTextBox).Text;
            if (!lastSource.Equals(source))
            {
                lastSource = source;
                SourceCodeAnalyzeRequired?.Invoke(source);
            }
        }
        private void lexicalAnlyzerOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LAForm.Show();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFileClick?.Invoke();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSourceCodeFileClick?.Invoke();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileClick?.Invoke(sourceBox.Text);
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAsClick?.Invoke(sourceBox.Text);
        }
        private void pDAAnalyzerOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PdaForm.ShowDialog();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GrammarAnalyzeRequired?.Invoke();
        }
        private void ascendingAnalyzerOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AAForm.ShowDialog();
        }
        private void rUNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildRequired?.Invoke();
        }
        private void polizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polizForm.ShowDialog();
        }
        #endregion

        private void DisabledRun_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Can't run program, see console", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private const int CONSOLE_HEIGHT = 100;
        private string lastSource = string.Empty;
        private Colorizer<Lexeme> colorizer;
        private LexicalAnalyzerForm LAForm = new LexicalAnalyzerForm();
        private PDAOutputForm PdaForm = new PDAOutputForm();
        private AscendingAnalyzerForm AAForm = new AscendingAnalyzerForm();
        private PolizForm polizForm = new PolizForm();
        private bool runEnabled = false;
    }
}