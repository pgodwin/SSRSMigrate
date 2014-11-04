using System;
using System.Windows.Forms;
using SSRSMigrate.Forms;

public class CoverageExcludeAttribute : System.Attribute { }

namespace SSRSMigrate
{
    [CoverageExcludeAttribute]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConnectInfoForm());
        }
    }
}
