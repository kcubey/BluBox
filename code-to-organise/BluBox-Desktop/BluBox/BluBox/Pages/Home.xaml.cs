using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fiddler;
using System.Data.SqlClient;
using SharpAdbClient;
using System.Net;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace BluBox.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Page));
            runADB();
        }

        
        void runADB()
        {
            var devices = AdbClient.Instance.GetDevices();
            foreach (var device in devices)
            {
                Console.WriteLine("** Getting device...: " + device.Name);
                var receiver = new ConsoleOutputReceiver();
            }
        }

        void DownloadFile()
        {
            Console.WriteLine("** Downloading file...");
            var device = AdbClient.Instance.GetDevices().First();

            using (SyncService service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), device))
            using (Stream stream = File.OpenWrite(@"C:\Users\Kezia- Main\Desktop\NUS\Y1S2\avventure\coding\BluBox-Desktop\BluBox\BluBox\blubox.txt"))
            {
                service.Pull("/sdcard/blubox.txt", stream, null, CancellationToken.None);
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            string redirectAdd = ((Button)sender).CommandParameter.ToString();
            this.NavigationService.Navigate(new Uri(redirectAdd, UriKind.Relative));
            Console.WriteLine("** Redirect to " + redirectAdd);
        }

        private void clickClear(object sender, RoutedEventArgs e)
        {
            medName.Text = "";
            medExp.Text = "";
        }

        private void clickRefresh(object sender, RoutedEventArgs e)
        {
            DownloadFile();

            string[] lines = File.ReadAllLines(@"C:\Users\Kezia- Main\Desktop\NUS\Y1S2\avventure\coding\BluBox-Desktop\BluBox\BluBox\blubox.txt");

            int counter = 0;
            foreach (string line in lines)
            {
                if(counter == 0)
                {
                    medName.Text = line;
                }
                else if(counter == 1)
                {
                    medExp.Text = line;
                }

                counter++;
            }
            
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BluBoxCon"].ConnectionString);
            connection.Open();
            return connection;
        }

        private DispatcherTimer dispatcherTimer;

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            string medNameStr = medName.Text;
            string medExpStr = medExp.Text;

            Console.WriteLine("** Med Name: " + medNameStr);
            Console.WriteLine("** Med Expiry: " + medExpStr);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Insert Into medicine(medName, medExpiry) Values(@medName, @medExpiry);";
            cmd.Parameters.AddWithValue("@medName", medNameStr);
            cmd.Parameters.AddWithValue("@medExpiry", medExpStr);
            cmd.Connection = GetConnection();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            Console.WriteLine("** Saving to database...");

            medName.Text = "";
            medExp.Text = "";

            updateMessage.Visibility = Visibility.Visible;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sendere, EventArgs e2)
        {
            updateMessage.Visibility = Visibility.Hidden;

            //Disable the timer
            dispatcherTimer.IsEnabled = false;
            Console.WriteLine("** Timer end");
        }




    }
}
