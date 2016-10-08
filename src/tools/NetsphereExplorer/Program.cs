using System;
using System.Windows.Forms;
using NetsphereExplorer.Views;

namespace NetsphereExplorer
{
    internal static class Program
    {
        public static IWin32Window Window { get; private set; }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var view = new MainView();
            Window = view;
            Application.Run(view);
        }
    }
}
