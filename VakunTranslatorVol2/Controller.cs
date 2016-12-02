using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VakunTranslatorVol2.Analyzers;

namespace VakunTranslatorVol2
{
    public class Controller
    {
        private IView view;
        private ILexicalAnalyzer lexicalAnalyzer;
        private ISyntaxAnalyzer syntaxAnalyzer;
        private FileManager fileManager;

        public Controller(IView view)
        {
            this.view = view;
            view.AnalyzeRequired += View_AnalyzeRequired;
            view.OpenFileClick += View_OpenFileClick;

            lexicalAnalyzer = new LexicalAnalyzer();
            //syntaxAnalyzer = new RecursiveSyntaxAnalyzer();
            syntaxAnalyzer = new PDASyntaxAnalyzer();

            view.SetColorizer(new Colorizer<Lexeme>(new LexemePainter())
            {
                SourceFilter = x => x.Is(LexemeFlags.Reserved | LexemeFlags.TypeDefinition | LexemeFlags.Const),
                WordSelector = x => x.Body
            });

            fileManager = new FileManager();
        }

        private void View_OpenFileClick()
        {
            if(fileManager.OpenFileDialog())
            {
                view.SetSourceCode(fileManager.Text);
            }
        }


        private void View_AnalyzeRequired(string source)
        {
            view.HideConsole();

            using(lexicalAnalyzer)
            using(syntaxAnalyzer)
            {
                var lexemes = lexicalAnalyzer.Analyze(source);

                view.HighlightSourceCode(lexemes);

                view.DisplayConstants(lexicalAnalyzer.Constants.Select(x => new { Code = x.ConstCode, Body = x.Body, Type = x.Type }));
                view.DisplayIdentificators(lexicalAnalyzer.Identificators.Select(x => new { Code = x.ConstCode, Body = x.Body, Type = x.Type }));
                view.DisplayLexemes(lexemes.Select(x => new { Body = x.Body, Type = x.Flags, Line = x.Line, Code = x.Code, ConstCode = x.ConstCode }));

                if(lexicalAnalyzer.HasErrors)
                {
                    ShowErrors(lexicalAnalyzer.Errors);
                }
                else
                {
                    syntaxAnalyzer.Analyze(lexemes);

                    if(syntaxAnalyzer.HasErrors)
                    {
                        ShowErrors(syntaxAnalyzer.Errors);
                    }
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