using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VakunTranslatorVol2
{
    public class FileManager
    {
        public string Text { get; private set; }
        private string fileName = null;

        public bool OpenFileDialog()
        {
            var dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
                Text = File.ReadAllText(fileName, Encoding.Unicode);
                return true;
            }
            else
            {
                fileName = null;
                return false;
            }
        }
    }
}