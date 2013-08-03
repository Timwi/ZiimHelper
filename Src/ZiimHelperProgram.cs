using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RT.Util;

[assembly: AssemblyTitle("ZiimHelper")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ZiimHelper")]
[assembly: AssemblyCopyright("Copyright © CuteBits 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("ce61939b-1b63-4503-bab9-41053d419c1c")]
[assembly: AssemblyVersion("1.0.9999.9999")]
[assembly: AssemblyFileVersion("1.0.9999.9999")]

namespace ZiimHelper
{
    static class ZiimHelperProgram
    {
        public static Settings Settings;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--post-build-check")
                return Ut.RunPostBuildChecks(args[1], Assembly.GetExecutingAssembly());

            SettingsUtil.LoadSettings(out ZiimHelperProgram.Settings);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mainform());

            Settings.Save(onFailure: SettingsOnFailure.ShowRetryOnly);
            return 0;
        }
    }
}
