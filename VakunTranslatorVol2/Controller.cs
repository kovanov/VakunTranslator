using System;
using System.Collections.Generic;
using System.Linq;
using VakunTranslatorVol2.CodeHighlight;
using VakunTranslatorVol2.Model;
using VakunTranslatorVol2.Model.Analyzers;
using VakunTranslatorVol2.Views;

namespace VakunTranslatorVol2
{
    public class Controller
    {
        private IMainForm view;
        private ILexicalAnalyzer _lexicalAnalyzer;
        private ISyntaxAnalyzer _syntaxAnalyzer;
        private FileManager fileManager;
        private GrammarChecker grammarChecker;
        private PolizGenerator _polizGenerator;

        public Controller(IMainForm view)
        {
            this.view = view;
            view.SourceCodeAnalyzeRequired += View_SourceCodeAnalyzeRequired;
            view.GrammarAnalyzeRequired += View_GrammarAnalyzeRequired;
            view.OpenSourceCodeFileClick += View_OpenSourceCodeFileClick;
            view.SaveFileClick += View_SaveFileClick;
            view.BuildRequired += View_BuildRequired;

            view.SetColorizer(new Colorizer<Lexeme>(new LexemePainter())
            {
                SourceFilter = x => x.Is(LexemeFlags.Reserved | LexemeFlags.TypeDefinition | LexemeFlags.Const),
                WordSelector = x => x.Body
            });

            fileManager = new FileManager();

            _lexicalAnalyzer = new LexicalAnalyzer();
            _syntaxAnalyzer = new PDASyntaxAnalyzer(view.SetPDAOutput);
            _polizGenerator = new PolizGenerator(_lexicalAnalyzer);
        }

        private void View_BuildRequired()
        {
            view.RunProgram(_polizGenerator.Poliz);
        }

        private void View_SaveFileClick(string text)
        {
            fileManager.SaveFile(text);
        }

        private void View_GrammarAnalyzeRequired()
        {
            var fileManager = new FileManager();

            if (fileManager.OpenFileDialog())
            {
                try
                {
                    grammarChecker = new GrammarChecker();
                    var table = grammarChecker.Check(fileManager.Text);
                    view.ShowGrammarTable(table);
                }
                catch (Exception e)
                {
                    view.ShowGrammarError(e.Message);
                    grammarChecker = null;
                }
            }
        }

        private void View_OpenSourceCodeFileClick()
        {
            if (fileManager.OpenFileDialog())
            {
                view.SetSourceCode(fileManager.Text);
            }
        }

        private void View_SourceCodeAnalyzeRequired(string source)
        {
            view.EnableRunButton();
            view.HideConsole();
            _lexicalAnalyzer = new LexicalAnalyzer();
            _syntaxAnalyzer = new PDASyntaxAnalyzer(view.SetPDAOutput);
            _polizGenerator = new PolizGenerator(_lexicalAnalyzer);

            var lexemes = _lexicalAnalyzer.Analyze(source);

            view.HighlightSourceCode(lexemes);

            view.DisplayConstants(_lexicalAnalyzer.Constants.Select(x => new { Code = x.ConstCode, Body = x.Body, Type = x.Type }));
            view.DisplayIdentificators(_lexicalAnalyzer.Identificators.Select(x => new { Code = x.ConstCode, Body = x.Body, Type = x.Type }));
            view.DisplayLexemes(lexemes.Select(x => new { Body = x.Body, Type = x.Flags, Line = x.Line, Code = x.Code, ConstCode = x.ConstCode }));

            if (_lexicalAnalyzer.HasErrors)
            {
                ShowErrors(_lexicalAnalyzer.Errors.Select(x=>$"Lexical error: {x}"));
                view.DisableRunButton();
                _polizGenerator.Clear();
            }
            else
            {
                view.EnableRunButton();
                _syntaxAnalyzer.Analyze(lexemes);

                if (_syntaxAnalyzer.HasErrors)
                {
                    ShowErrors(_syntaxAnalyzer.Errors.Select(x => $"Syntax error: {x}"));
                    view.DisableRunButton();
                    _polizGenerator.Clear();
                }
                else
                {
                    _polizGenerator.MakePoliz();
                }
            }
            view.SetPoliz(_polizGenerator.Poliz);
        }

        private void ShowErrors(IEnumerable<string> errors)
        {
            view.ClearConsole();
            view.ShowConsole();
            view.WriteConsole(errors);
        }
    }
}