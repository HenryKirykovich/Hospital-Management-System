using System;
using System.Windows.Forms;
using HospitalManagement.Client.Forms;

namespace HospitalManagement.Client
{
    internal static class Program
    {
        /// <summary>
        /// Application entry point. Opens the Login form first.
        /// After successful login, LoginForm opens MainForm and hides itself.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
