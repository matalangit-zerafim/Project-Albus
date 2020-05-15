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
using Oracle.DataAccess.Client;

namespace Project_Albus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using (OracleCommand cmd = new OracleCommand("SELECT * FROM employee WHERE username = :username and password=:password", App.Connection))
            {
                App.Connection.Open();

                cmd.Parameters.Add(":username", username.Text);
                cmd.Parameters.Add(":password", password.Text);

                var result = cmd.ExecuteScalar();
                if (result == null && (username.Text != "admin" || password.Text != "admin")) {
                    MessageBox.Show("Username or password is incorrect");
                }
                else
                {
                    new MainMenuWindow().ShowDialog();
                }


                App.Connection.Close();

            }
        }
    }
}
