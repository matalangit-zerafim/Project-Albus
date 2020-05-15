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
    /// Interaction logic for Master_Status.xaml
    /// </summary>
    /// 

    public partial class Master_Status : Window
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

        private void LoadData()
        {
            OracleDataAdapter adapter = new OracleDataAdapter("select * from status", App.Connection);
            ds = new DataSet();
            adapter.Fill(ds, "status");

            dataGrid.ItemsSource = ds.Tables["status"].DefaultView;
        }

        public Master_Status()
        {
            InitializeComponent();
            LoadData();
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into status(name) values(:name)", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
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
                editMode();
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE STATUS SET NAME=:name WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("DELETE FROM STATUS WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();
            LoadData();
            exitEditMode();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            name.Text = "";
            exitEditMode();
        }
    }
}
