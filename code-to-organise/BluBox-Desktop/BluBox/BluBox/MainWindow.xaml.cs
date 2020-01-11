using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Fiddler;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Diagnostics;
using SharpAdbClient;

namespace BluBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BluBox.Pages.Browser.Logs logsPage = new BluBox.Pages.Browser.Logs();

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));

            startADB();

        }

        //**NOTE: The application on the Android device must be started before opening the app. 
        void startADB()
        {
            Console.WriteLine("** Starting ADB...");
            AdbServer server = new AdbServer();
            var result = server.StartServer(@"C:\Users\Kezia- Main\Desktop\NUS\Y1S2\avventure\coding\BluBox-Desktop\BluBox\BluBox\adb.exe", restartServerIfNewer: false);
        }
    
        private void OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Source = new Uri(((Button)sender).CommandParameter.ToString(), UriKind.Relative);
        }


        protected override void OnClosed(EventArgs e)
        {
            Console.WriteLine("** Closing ADB...");
            AdbClient.Instance.KillAdb();
        }

        

    }
}
