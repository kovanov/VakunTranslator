using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace VakunTranslatorVol2.CodeHighlight
{
    public class Colorizer<T>
    {
        public Func<T, bool> SourceFilter { get; set; }
        public Func<T, string> WordSelector { get; set; }
        public RichTextBoxFinds SearchOptions { get; set; }
        public bool ResetColors { get; set; }

        private IPainter<T> painter;

        public Colorizer(IPainter<T> painter)
        {
            this.painter = painter;
            SourceFilter = x => true;
            WordSelector = x => x as string ?? string.Empty;
            SearchOptions = RichTextBoxFinds.WholeWord;
            ResetColors = true;
        }

        public void Highlight(RichTextBox box, IEnumerable<T> source)
        {
            var selectionColor = box.SelectionColor;
            var selectionStart = box.SelectionStart;
            source = source.Where(SourceFilter).Distinct();
            
            box.Enabled = false;

            if(ResetColors)
            {
                box.SelectAll();
                box.SelectionColor = painter.DefaultColor;
                box.DeselectAll();
                box.SelectionStart = selectionStart;
            }

            foreach(var item in source)
            {
                var lastFound = 0;
                var value = WordSelector(item);
                var color = painter.GetColor(item);

                while(box.TextLength > lastFound && (lastFound = box.Find(value, lastFound, SearchOptions)) >= 0)
                {
                    box.SelectionStart = lastFound;
                    box.SelectionLength = value.Length;
                    box.SelectionColor = color;
                    lastFound += value.Length;
                }
            }
            box.DeselectAll();
            box.SelectionStart = selectionStart;
            box.SelectionColor = selectionColor;
            box.Enabled = true;
            box.Focus();
        }
    }
}