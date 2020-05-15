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
    /// Interaction logic for Master_Purchase.xaml
    /// </summary>
    public partial class Master_Purchase : Window
    {
        public Master_Purchase()
        {
            InitializeComponent();
            LoadData();
        }

        private DataSet ds;

        private class Commodity
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
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT P.ID \"ID\", C.NAME \"COMMODITY\", P.LOG_DATE \"LOG DATE\", P.AMOUNT \"AMOUNT\"  FROM PURCHASE P JOIN COMMODITY C ON P.FK_COMMODITY_ID = C.ID", App.Connection);
            ds = new DataSet();
            adapter.Fill(ds, "purchase");
            dataGrid.ItemsSource = ds.Tables["purchase"].DefaultView;

            App.Connection.Open();

            OracleCommand cmd = new OracleCommand("select * from commodity", App.Connection);
            OracleDataReader dr = cmd.ExecuteReader();
            List<Commodity> commodity = new List<Commodity>();
            while (dr.Read())
            {

                commodity.Add(new Commodity()
                {
                    ID = dr.GetInt32(0).ToString(),
                    Name = dr.GetString(1)
                });
            }
            dr.Close();

            commodityCB.ItemsSource = commodity;
            commodityCB.DisplayMemberPath = "Name";
            commodityCB.SelectedValuePath = "ID";
            commodityCB.SelectedIndex = 0;
            App.Connection.Close();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            commodityCB.SelectedIndex = 0;
            amount.Text = "";
            exitEditMode();
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into purchase(FK_COMMODITY_ID,amount,log_date) values(:commodity,:amount,CURRENT_TIMESTAMP)", App.Connection);
            cmd.Parameters.Add(":commodity", commodityCB.SelectedValue.ToString());
            cmd.Parameters.Add(":amount", amount.Text);
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
                amount.Text = dr["amount"].ToString();
                commodityCB.Text = dr["commodity"].ToString();
                editMode();
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE PURCHASE SET FK_COMMODITY_ID=:commodity, AMOUNT=:amount WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":commodity", commodityCB.SelectedValue);
            cmd.Parameters.Add(":amount", amount.Text);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("DELETE FROM PURCHASE WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();
            LoadData();
            exitEditMode();
        }
    }
}
