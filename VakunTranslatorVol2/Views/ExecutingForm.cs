using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VakunTranslatorVol2.Model;

namespace VakunTranslatorVol2.Views
{
    public partial class ExecutingForm : Form
    {
        private List<string> _poliz;
        private Stack<string> _stack = new Stack<string>();
        private Dictionary<string, string> _scope = new Dictionary<string, string>();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public ExecutingForm(List<string> poliz)
        {
            InitializeComponent();
            _poliz = poliz;
            Shown += ExecutingForm_Shown;
            FormClosing += delegate { _cts.Cancel(); };
        }

        private void ExecutingForm_Shown(object sender, EventArgs e)
        {
            var lexemes = _poliz.Select(x => Lexeme.Parse(x)).ToList();

            foreach (var lexeme in lexemes)
            {
                if (lexeme.Body.StartsWith("m{") && !lexeme.Body.EndsWith(":"))
                {
                    lexeme.Code = (int)LexemeCodes.LABEL;
                    continue;
                }

                if (lexeme.Body == "УПЛ" || lexeme.Body == "БП" || lexeme.Body.EndsWith(":"))
                {
                    lexeme.Code = (int)LexemeCodes.NOTHING;
                }
            }

            _scope = lexemes.Where(x => x.Is(LexemeCodes.ID))
                .Distinct()
                .ToDictionary(x => x.Body, x => "0");

            var token = _cts.Token;
            Task.Run(() =>
            {
                for (int i = 0; i < lexemes.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    ExecuteCommand(lexemes[i], ref i);
                }
                WriteConsole("Done");
            }, token);
        }

        private void WriteConsole(string text)
        {
            textBox1.Invoke((Action)(() => textBox1.AppendText(text + Environment.NewLine)));
        }

        private void ExecuteCommand(Lexeme lexeme, ref int i)
        {
            if (lexeme.Is(LexemeCodes.ID))
            {
                _stack.Push(lexeme.Body);
                return;
            }

            if (lexeme.Is(LexemeCodes.CONSTANT))
            {
                _stack.Push(lexeme.Body);
                return;
            }

            if (lexeme.Is(LexemeCodes.LABEL))
            {
                _stack.Push(lexeme.Body);
                return;
            }

            if (lexeme.Is(LexemeCodes.WRITE))
            {
                var variable = _stack.Pop();
                WriteConsole($"{variable}: {_scope[variable]}");
                return;
            }

            if (lexeme.Is(LexemeCodes.ASSIGN))
            {
                var value = _stack.Pop();
                var variable = _stack.Pop();

                if (!int.TryParse(value, out var parsedValue))
                {
                    parsedValue = int.Parse(_scope[value]);
                }
                _scope[variable] = parsedValue.ToString();
                return;
            }

            if (lexeme.Is(LexemeCodes.NOTHING))
            {
                if (lexeme.Body.Equals("БП"))
                {
                    var label = _stack.Pop();
                    i = _poliz.IndexOf($"{label}:");
                }

                if (lexeme.Body.Equals("УПЛ"))
                {
                    var label = _stack.Pop();
                    var compareResult = _stack.Pop();
                    if (compareResult.Equals(false.ToString()))
                    {
                        i = _poliz.IndexOf($"{label}:");
                    }
                }

                return;
            }

            if (lexeme.Is(LexemeCodes.READ))
            {
                var variable = _stack.Pop();
                var value = new Prompt().ShowDialog(variable, "Enter value for variable");
                _scope[variable] = int.Parse(value).ToString();
                return;
            }

            switch ((LexemeCodes)lexeme.Code)
            {
                case LexemeCodes.PLUS: PopAndPush((x, y) => x + y); break;
                case LexemeCodes.MINUS: PopAndPush((x, y) => x - y); break;
                case LexemeCodes.DIVISION: PopAndPush((x, y) => x / y); break;
                case LexemeCodes.MULTIPLY: PopAndPush((x, y) => x * y); break;
                case LexemeCodes.AND: PopAndPush((bool x, bool y) => x & y); break;
                case LexemeCodes.OR: PopAndPush((bool x, bool y) => x | y); break;
                case LexemeCodes.NOT: PopAndPush(x => !x); break;
                case LexemeCodes.LESS_EQUAL: PopAndPush((x, y) => x <= y); break;
                case LexemeCodes.LESS_THAN: PopAndPush((x, y) => x < y); break;
                case LexemeCodes.MORE_EQUAL: PopAndPush((x, y) => x >= y); break;
                case LexemeCodes.MORE_THAN: PopAndPush((x, y) => x > y); break;
                case LexemeCodes.NOT_EQUAL: PopAndPush((int x, int y) => x != y); break;
                case LexemeCodes.EQUAL: PopAndPush((int x, int y) => x == y); break;
                default: throw new Exception(lexeme.Body);
            }
        }

        private void PopAndPush(Func<bool, bool> func)
        {
            var left = bool.Parse(_stack.Pop());

            _stack.Push(func(left).ToString());
        }

        private void PopAndPush(Func<bool, bool, bool> func)
        {
            var right = bool.Parse(_stack.Pop());
            var left = bool.Parse(_stack.Pop());

            _stack.Push(func(left, right).ToString());
        }

        private void PopAndPush(Func<int, int, bool> func)
        {
            ParseStackTop(out var right);
            ParseStackTop(out var left);

            _stack.Push(func(left, right).ToString());
        }

        private void PopAndPush(Func<int, int, int> func)
        {
            ParseStackTop(out var right);
            ParseStackTop(out var left);

            _stack.Push(func(left, right).ToString());
        }

        void ParseStackTop(out int value)
        {
            var stackTop = _stack.Pop();
            if (!int.TryParse(stackTop, out value))
            {
                value = int.Parse(_scope[stackTop]);
            }
        }
    }

    public class Prompt : Form
    {
        private TextBox textBox;
        private Label label;
        public Prompt()
        {
            Width = 250;
            Height = 100;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;

            label = new Label() { Left = 10, Top = 22 };
            textBox = new TextBox() { Left = 70, Top = 20, Width = 90 };
            Button confirmation = new Button() { Text = "OK", Left = 170, Width = 40, Top = 18, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => Close();
            Controls.Add(textBox);
            Controls.Add(confirmation);
            Controls.Add(label);
            AcceptButton = confirmation;
        }

        public string ShowDialog(string text, string caption)
        {
            label.Text = text;
            Text = caption;
            return ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
