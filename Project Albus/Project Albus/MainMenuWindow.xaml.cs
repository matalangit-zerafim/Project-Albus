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
using System.Windows.Shapes;

namespace Project_Albus
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Pegawai().ShowDialog();
            this.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Service().ShowDialog();
            this.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Commodity().ShowDialog();
            this.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Department().ShowDialog();
            this.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Purchase().ShowDialog();
            this.Show();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Customer().ShowDialog();
            this.Show();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Master_Status().ShowDialog();
            this.Show();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new Transaction().ShowDialog();
            this.Show();
        }
    }
}
