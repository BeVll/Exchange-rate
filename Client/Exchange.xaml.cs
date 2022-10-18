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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exchanger;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Exchange.xaml
    /// </summary>
    public partial class Exchange : Page
    {
        Exchanger.Exchanger exchange = new Exchanger.Exchanger();
        Exchanger.Client client;
        public Exchange(Exchanger.Client client)
        {
            InitializeComponent();
            this.client = client;
            cur1.ItemsSource = exchange.currencies;
            cur2.ItemsSource = exchange.currencies;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Currency cur_name1 = cur1.SelectedItem as Currency;
            Currency cur_name2 = cur2.SelectedItem as Currency;
            string cur = cur_name1.Name + "-" + cur_name2.Name;
            string result = client.GetExchange(cur);
            
            if(result.Contains("{Error}"))
            {
                client.Close();
                NavigationService.Navigate(new Login(result));
            }
            else
                tot.Text = $"{cur_name1.Name}-{cur_name2.Name}: {result}";
        }
        
    }
}
