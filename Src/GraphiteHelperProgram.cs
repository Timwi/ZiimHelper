using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RT.Util;

[assembly: AssemblyTitle("GraphiteHelper")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("GraphiteHelper")]
[assembly: AssemblyCopyright("Copyright © CuteBits 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("ce61939b-1b63-4503-bab9-41053d419c1c")]
[assembly: AssemblyVersion("1.0.9999.9999")]
[assembly: AssemblyFileVersion("1.0.9999.9999")]

namespace GraphiteHelper
{
    static class GraphiteHelperProgram
    {
        public static Settings Settings;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SettingsUtil.LoadSettings(out GraphiteHelperProgram.Settings);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mainform());

            SettingsUtil.SaveSettings(Settings, SettingsUtil.OnFailure.ShowRetryOnly);
        }
    }
}
