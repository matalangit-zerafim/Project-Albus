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
using Oracle.DataAccess.Client;
using System.Data;

namespace Project_Albus
{
    /// <summary>
    /// Interaction logic for Master_Commodity.xaml
    /// </summary>
    public partial class Master_Commodity : Window
    {
        private DataSet ds;
        private void editMode()
        {
            insert.IsEnabled = false;
            update.IsEnabled = true;
            delete.IsEnabled = true;
        }
        private void exitEditMode()
        {
            insert.IsEnabled = true;
            update.IsEnabled = false;
            delete.IsEnabled = false;
        }
        public Master_Commodity()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            OracleDataAdapter adapter = new OracleDataAdapter("select * from commodity", App.Connection);
            ds = new DataSet();
            adapter.Fill(ds, "commodity");

            dataGrid.ItemsSource = ds.Tables["commodity"].DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into commodity(name,price) values(:name,:price)", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":price", price.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                DataRow dr = ds.Tables[0].Rows[dataGrid.SelectedIndex];
                id.Text = dr["id"].ToString();
                name.Text = dr["name"].ToString();
                price.Text = dr["price"].ToString();
                editMode();
            }
        }

       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            name.Text = "";
            price.Text = "";
            exitEditMode();
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE COMMODITY SET NAME=:name, PRICE=:price WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":price", price.Text);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();

        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("DELETE FROM COMMODITY WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();
            LoadData();
            exitEditMode();
        }
    }
    
}
