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
    /// Interaction logic for Master_Service.xaml
    /// </summary>
    public partial class Master_Service : Window
    {
        private DataSet dsService;
        private DataSet dsStock;

        private class Commodity
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        public Master_Service()
        {
            InitializeComponent();
            LoadData();
        }
        private void editMode()
        {
            insert.IsEnabled = false;
            update.IsEnabled = true;
            delete.IsEnabled = true;
            commodityCB.IsEnabled = true;
            amount.IsEnabled = true;
            add.IsEnabled = true;
        }
        private void exitEditMode()
        {
            insert.IsEnabled = true;
            update.IsEnabled = false;
            delete.IsEnabled = false;
            commodityCB.IsEnabled = false;
            amount.IsEnabled = false;
            add.IsEnabled = false;
        }

        private void editStockMode()
        {
            add.IsEnabled = false;
            edit.IsEnabled = true;
            remove.IsEnabled = true;
        }
        private void exitEditStockMode()
        {
            add.IsEnabled = true;
            edit.IsEnabled = false;
            remove.IsEnabled = false;
        }
        private void LoadData()
        {
            OracleDataAdapter adapter = new OracleDataAdapter("select * from service", App.Connection);
            dsService = new DataSet();
            adapter.Fill(dsService, "service");
            dataGrid.ItemsSource = dsService.Tables["service"].DefaultView;

            adapter = new OracleDataAdapter("select s.id, c.name, s.amount from stock s join commodity c on s.fk_commodity_id = c.id where s.fk_service_id = '" + id.Text + "'", App.Connection);
            dsStock = new DataSet();
            adapter.Fill(dsStock, "stock");
            dataGridStock.ItemsSource = dsStock.Tables["stock"].DefaultView;


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

            App.Connection.Open();

            cmd = new OracleCommand("SELECT SUM(C.PRICE * S.AMOUNT) FROM STOCK S JOIN COMMODITY C ON S.FK_COMMODITY_ID = C.ID WHERE S.FK_SERVICE_ID = :id", App.Connection);
            cmd.Parameters.Add(":id", id.Text);
            total.Content = cmd.ExecuteScalar().ToString();
            App.Connection.Close();


        }
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (dataGrid.SelectedIndex != -1)
            {
                DataRow dr = dsService.Tables[0].Rows[dataGrid.SelectedIndex];
                id.Text = dr["id"].ToString();
                name.Text = dr["name"].ToString();
                price.Text = dr["price"].ToString();
                LoadData();
                editMode();
            }
        }
        private void insert_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into service(name,price) values(:name,:price)", App.Connection);
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":price", price.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE service SET name=:name,price=:price WHERE ID=:id", App.Connection);
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
            OracleCommand cmd = new OracleCommand("DELETE FROM SERVICE WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();
            LoadData();
            exitEditMode();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            id.Text = "";
            name.Text = "";
            price.Text = "";
            LoadData();
            exitEditMode();
        }

        private void dataGridStock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridStock.SelectedIndex != -1)
            {
                DataRow dr = dsStock.Tables[0].Rows[dataGridStock.SelectedIndex];
                idStock.Text = dr["id"].ToString();
                commodityCB.Text = dr["name"].ToString();
                amount.Text = dr["amount"].ToString();
                editStockMode();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into stock(FK_COMMODITY_ID,FK_SERVICE_ID,amount) values(:commodity,:id,:amount)", App.Connection);
            cmd.Parameters.Add(":commodity", commodityCB.SelectedValue.ToString());
            cmd.Parameters.Add(":id", id.Text);
            cmd.Parameters.Add(":amount", amount.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE stock SET FK_COMMODITY_ID=:commodity,amount=:amount WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":commodity", commodityCB.SelectedValue.ToString());
            cmd.Parameters.Add(":amount", amount.Text);
            cmd.Parameters.Add(":id", idStock.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("DELETE FROM STOCK WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":id", idStock.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();
            LoadData();
            exitEditStockMode();
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            idStock.Text = "";
            commodityCB.SelectedIndex = 0;
            amount.Text = "";
            LoadData();
            exitEditStockMode();
        }
    }
}
