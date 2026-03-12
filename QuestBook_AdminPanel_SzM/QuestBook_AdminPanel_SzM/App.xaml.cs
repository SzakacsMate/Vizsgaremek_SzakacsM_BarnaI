using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuestBook_AdminPanel_SzM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();

            if (result == true && loginWindow.IsAdminAuthenticated)
            {
                var mainWindow = new MainWindow(loginWindow.AccessToken);
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Bejelentkezés sikertelen vagy megszakítva.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                Shutdown();
            }
        }
    }
}
