using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using Exchanger;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        Exchanger.Client client = new Exchanger.Client();
        int time = 10;
        public Login()
        {
            InitializeComponent();
            
        }
        public Login(string alert)
        {
            InitializeComponent();
            alerttext.Text = alert;
            Thread thread = new Thread(() => ShowAnimation());
            thread.Start();
            Thread thread1 = new Thread(() => TimeAdd());
            thread1.Start();
        }
        private void TimeAdd()
        {
            time = 0;
            while(time < 10)
            {
                Thread.Sleep(1000);
                time++;
            }
        }
        private void ShowAnimation()
        {     
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ThicknessAnimation animation = new ThicknessAnimation();
                    animation.From = new Thickness(0, 0, 15, -180);
                    animation.To = new Thickness(0, 0, 15, 15);
                    animation.Duration = TimeSpan.FromMilliseconds(500);
                    alertbox.BeginAnimation(Border.MarginProperty, animation);
                })); 
        }
        private void CloseAnimation()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {

                ThicknessAnimation animation = new ThicknessAnimation();
                animation.From = new Thickness(0, 0, 15, 15);
                animation.To = new Thickness(0, 0, 15, -180);
                animation.Duration = TimeSpan.FromMilliseconds(500);
                alertbox.BeginAnimation(Border.MarginProperty, animation);
            }));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (time == 10)
            {
                if (Log.Text != string.Empty && Pass.Text != string.Empty)
                {
                    string con = client.Connect("127.0.0.1", 8888, Log.Text, Pass.Text);
                    if (con == "connected")
                    {
                        NavigationService.Navigate(new Exchange(client));
                    }
                    else
                    {
                        alerttext.Text = con;
                        Thread thread = new Thread(() => ShowAnimation());
                        thread.Start();
                        client.Close();
                    }
                }
            }
            else
            {
                alerttext.Text = $"Wait {10-time}seconds!";
                Thread thread = new Thread(() => ShowAnimation());
                thread.Start();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => CloseAnimation());
            thread.Start();
        }
    }
}
