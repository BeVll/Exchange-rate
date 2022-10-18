using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exchanger;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Read> reads = new List<Read>();
        public Exchanger.Server server = new Exchanger.Server();
        bool check = true;
        Thread readThread;
        int id = 0;
        bool server_started = false;
        public MainWindow()
        {
            
            InitializeComponent();
            
        }
        public void AddMessage(string str)
        {
            
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageWPF messageWPF = new MessageWPF();
                messageWPF.Created = DateTime.Now;
                messageWPF.Title = str;

                messageWPF.Alig = HorizontalAlignment.Right;
                messageWPF.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#163B60"));

                mes.Items.Add(messageWPF);
                server.Log($"Server: {messageWPF.Title}");
            }));
           
        }
        public void AddMessage(string str, User user)
        {
            
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageWPF messageWPF = new MessageWPF();
                messageWPF.Created = DateTime.Now;
                messageWPF.Title = str;
                messageWPF.UserName = user.Login;

                messageWPF.Alig = HorizontalAlignment.Left;
                messageWPF.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#393E46"));
                mes.Items.Add(messageWPF);
                server.Log($"{user.Login}: {messageWPF.Title}");
            }));
           
        
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
        private void Read()
        {
            while (check)
            {
                id = server.id;
                foreach (User user in server.clients.ToList())
                {
                    
                    Read r = reads.Where(s => s.User == user).FirstOrDefault();
                    if(r == null)
                    {
                        Thread thread = new Thread(() => ReadUser(user));
                        thread.Start();
                        reads.Add(new Read(thread, user));
                    }
                    if (id != server.id)
                    {
                        break;
                    }
                    Thread.Sleep(50); // Щоб ненавантажувало процесор
                }
            }
        }
        private void ReadUser(User user)
        {
            while (server.working)
            {
                string mes = user.GetMessage();
                AddMessage(mes, user);
            }
           
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (server_started)
            {
                if (Log.Text != string.Empty && pass.Text != string.Empty)
                {
                    if (server.users.Exists(s => s.Login == Log.Text) == true)
                        AddMessage($"User {Log.Text} already exists");
                    else
                    {
                        User user = new User(Log.Text, pass.Text);
                        server.AddUser(user);
                        AddMessage($"User {Log.Text} added with password {pass.Text}");
                    }
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            server.max_req = Convert.ToInt32(Req.Text);
            server.max_users = Convert.ToInt32(MaxUsers.Text);
            server.Start();
            Thread waitcon = new Thread(server.Start);
            readThread = new Thread(() => Read());
            readThread.Start();
            server_started = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
