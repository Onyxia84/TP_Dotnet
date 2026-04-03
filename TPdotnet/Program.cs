using TPdotnet.Data;
using System;
using System.Windows.Forms;

namespace TPdotnet
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
