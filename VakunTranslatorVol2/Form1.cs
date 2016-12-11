using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VakunTranslatorVol2.Analyzers;

namespace VakunTranslatorVol2
{
    public partial class Form1 : Form, IView
    {
        public event Action<string> SourceCodeAnalyzeRequired;
        public event Action GrammarAnalyzeRequired;
        public event Action NewFileClick;
        public event Action OpenSourceCodeFileClick;
        public event Action<string> SaveFileClick;
        public event Action<string> SaveFileAsClick;

        public Form1()
        {
            InitializeComponent();
            menuStrip1.Renderer = new CustomToolStripRenderer();
            sourceBox.SelectionTabs = Enumerable.Range(1, 10).Select(x => x * 14).ToArray();
            analyzerWindow.LexemSelected += AnalyzerWindow_LexemSelected;
        }

        private void GrammarCheck_AnalyzeRequired(string grammar)
        {
            GrammarAnalyzeRequired?.Invoke();
        }

        public void ClearConsole()
        {
            console.Clear();
        }
        public void DisplayConstants<T>(IEnumerable<T> source)
        {
            analyzerWindow.DisplayConstants(source);
        }
        public void DisplayIdentificators<T>(IEnumerable<T> source)
        {
            analyzerWindow.DisplayIdentificators(source);
        }
        public void DisplayLexemes<T>(IEnumerable<T> source)
        {
            analyzerWindow.DisplayLexemes(source);
        }
        public void HighlightSourceCode(IEnumerable<Lexeme> source)
        {
            colorizer?.Highlight(sourceBox, source);
        }
        public void WriteConsole(IEnumerable<string> lines)
        {
            foreach(var line in lines)
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
            pdaWindow.SetRules(rules);
        }

        private void sourceBox_TextChanged(object sender, EventArgs e)
        {
            var source = (sender as RichTextBox).Text;
            if(!lastSource.Equals(source))
            {
                lastSource = source;
                SourceCodeAnalyzeRequired?.Invoke(source);
            }
        }
        private void lexicalAnlyzerOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            analyzerWindow.Show();
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
        private void AnalyzerWindow_LexemSelected(string body, int line)
        {
            var start = sourceBox.GetFirstCharIndexFromLine(line);
            var end = start + sourceBox.Lines[line].Length;

            sourceBox.Find(body, start, end, RichTextBoxFinds.WholeWord);
            Focus();
        }
        private void pDAAnalyzerOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pdaWindow.ShowDialog();
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GrammarAnalyzeRequired?.Invoke();
        }

        private const int CONSOLE_HEIGHT = 250;
        private string lastSource = string.Empty;
        private Colorizer<Lexeme> colorizer;
        private AnalyzerWindow analyzerWindow = new AnalyzerWindow();
        private PDAOutputForm pdaWindow = new PDAOutputForm();
    }
}