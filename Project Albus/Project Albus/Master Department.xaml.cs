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
    /// Interaction logic for Master_Department.xaml
    /// </summary>
    public partial class Master_Department : Window
    {
        public Master_Department()
        {
            InitializeComponent();
            LoadData();
        }


        private DataSet ds;

        private class Salary
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }


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
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT *  FROM DEPARTMENT", App.Connection);
            ds = new DataSet();
            adapter.Fill(ds, "department");
            dataGrid.ItemsSource = ds.Tables["department"].DefaultView;

        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into department(name,rank,salary) values(:name,:rank,:price)", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":rank", rank.Text);
            cmd.Parameters.Add(":salary", salary.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void dataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                DataRow dr = ds.Tables[0].Rows[dataGrid.SelectedIndex];
                id.Text = dr["id"].ToString();
                name.Text = dr["name"].ToString();
                rank.Text = dr["rank"].ToString();
                salary.Text = dr["salary"].ToString();
                editMode();
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE DEPARTMENT SET NAME=:name, RANK=:rank, SALARY=:salary WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":rank", rank.Text);
            cmd.Parameters.Add(":salary", salary.Text);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("DELETE FROM DEPARTMENT WHERE ID=:id", App.Connection);
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
            rank.Text = "";
            salary.Text = "";
            exitEditMode();
        }
    }
}
