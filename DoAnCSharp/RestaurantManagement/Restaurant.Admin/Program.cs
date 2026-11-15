using Restaurant.Admin.Services;

namespace Restaurant.Admin
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            while (true)
            {
                using var loginForm = new LoginForm();
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                using var mainForm = new Form1();
                System.Windows.Forms.Application.Run(mainForm);
                if (!ApiSession.IsLogoutRequested)
                {
                    return;
                }
            }
        }
    }
}