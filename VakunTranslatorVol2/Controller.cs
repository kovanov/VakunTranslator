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
        private ILexicalAnalyzer lexicalAnalyzer;
        private ISyntaxAnalyzer syntaxAnalyzer;
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

            lexicalAnalyzer = new LexicalAnalyzer();
            syntaxAnalyzer = new PDASyntaxAnalyzer(view.SetPDAOutput);
            _polizGenerator = new PolizGenerator(lexicalAnalyzer);
        }

        private void View_BuildRequired()
        {
            _polizGenerator = new PolizGenerator(lexicalAnalyzer);
            view.RunProgram(_polizGenerator.MakePoliz());
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
            lexicalAnalyzer = new LexicalAnalyzer();
            syntaxAnalyzer = new PDASyntaxAnalyzer(view.SetPDAOutput);

            var lexemes = lexicalAnalyzer.Analyze(source);

            view.HighlightSourceCode(lexemes);

            view.DisplayConstants(lexicalAnalyzer.Constants.Select(x => new { Code = x.ConstCode, Body = x.Body, Type = x.Type }));
            view.DisplayIdentificators(lexicalAnalyzer.Identificators.Select(x => new { Code = x.ConstCode, Body = x.Body, Type = x.Type }));
            view.DisplayLexemes(lexemes.Select(x => new { Body = x.Body, Type = x.Flags, Line = x.Line, Code = x.Code, ConstCode = x.ConstCode }));

            if (lexicalAnalyzer.HasErrors)
            {
                ShowErrors(lexicalAnalyzer.Errors.Select(x=>$"Lexical error: {x}"));
                view.DisableRunButton();
            }
            else
            {
                view.EnableRunButton();
                syntaxAnalyzer.Analyze(lexemes);

                if (syntaxAnalyzer.HasErrors)
                {
                    ShowErrors(syntaxAnalyzer.Errors.Select(x => $"Syntax error: {x}"));
                    view.DisableRunButton();
                }
            }
        }

        private void ShowErrors(IEnumerable<string> errors)
        {
            view.ClearConsole();
            view.ShowConsole();
            view.WriteConsole(errors);
        }
    }
}