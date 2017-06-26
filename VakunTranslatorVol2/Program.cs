using System;
using System.Windows.Forms;
using VakunTranslatorVol2.Views;

namespace VakunTranslatorVol2
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var view = new MainForm();
            var controller = new Controller(view);
            Application.Run(view);
        }
    }
}
