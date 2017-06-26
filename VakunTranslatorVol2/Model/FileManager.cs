using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VakunTranslatorVol2.Model
{
    public class FileManager
    {
        public string Text { get; private set; }

        public bool OpenFileDialog()
        {
            if(TryGetFilePath<OpenFileDialog>())
            {
                Text = File.ReadAllText(filePath, Encoding.Unicode);
                return true;
            }

            return false;
        }

        public void SaveFile(string text)
        {
            if(!string.IsNullOrEmpty(filePath))
            {
                WriteToFile(text);
            }
            else
            {
                SaveFileAs(text);
            }
        }

        public void SaveFileAs(string text)
        {
            if(TryGetFilePath<SaveFileDialog>())
            {
                WriteToFile(text);
            }
        }

        private void WriteToFile(string text)
        {
            File.WriteAllText(filePath, text, Encoding.Unicode);
        }

        private bool TryGetFilePath<T>() 
            where T : FileDialog, new()
        {
            using(var dialog = new T())
            {
                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                    return true;
                }
            }
            
            return false;
        }

        private string filePath = null;
    }
}