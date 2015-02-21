using System;
using System.Windows.Forms;

namespace UpdaterSetup_2._0._0._0 {
    internal static class Program {
        public static MainForm MainForm;

        [STAThread]
        private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0) {
                if (args[0].ToLower() == "/uninstall") {
                    MainForm = new MainForm(true);
                }
            }
            else {
                MainForm = new MainForm();
            }
            Application.Run(MainForm);
        }
    }
}
