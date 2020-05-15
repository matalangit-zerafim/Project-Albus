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
using System.Windows.Controls.Primitives;

namespace Project_Albus
{
    /// <summary>
    /// Interaction logic for Transaction.xaml
    /// </summary>
    public partial class Transaction : Window
    {
        private DataSet cartTableData;
        private DataSet transactionData;
        private DataSet statusData;
        private DataTable cartData;
        int count = 0;

        private class ComboData
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }

        public Transaction()
        {
            InitializeComponent();

            cartData = new DataTable();
            cartData.Columns.Add("ID");
            cartData.Columns.Add("SERVICE");
            cartData.Columns.Add("AMOUNT");
            LoadData();

        }


        private void editMode()
        {
            add.IsEnabled = false;
            remove.IsEnabled = true;
        }
        private void exitEditMode()
        {
            add.IsEnabled = true;
            remove.IsEnabled = false;
        }

        private void statusEditMode()
        {
            add.IsEnabled = false;
            remove.IsEnabled = false;
            clear.IsEnabled = false;
            proceed.IsEnabled = false;
            updateStatus.IsEnabled = true;
        }

        private void exitStatusEditMode()
        {
            add.IsEnabled = true;
            remove.IsEnabled = false;
            clear.IsEnabled = true;
            proceed.IsEnabled = true;
            updateStatus.IsEnabled = false;
            dataGridStatus.ItemsSource = null;
            dataGridCart.ItemsSource = cartData.DefaultView;
            dataGridCart.Columns[0].Visibility = Visibility.Collapsed;
        }
        private void LoadData()
        {

            // Filling status combobox

            OracleDataAdapter adapter = new OracleDataAdapter("SELECT *  FROM TRANSACTION_MASTER", App.Connection);
            transactionData = new DataSet();
            adapter.Fill(transactionData, "transaction");
            dataGridTransaction.ItemsSource = transactionData.Tables["transaction"].DefaultView;


            App.Connection.Open();

            OracleCommand cmd = new OracleCommand("select * from status", App.Connection);
            OracleDataReader dr = cmd.ExecuteReader();
            List<ComboData> status = new List<ComboData>();
            while (dr.Read())
            {

                status.Add(new ComboData()
                {
                    ID = dr.GetInt32(0).ToString(),
                    Name = dr.GetString(1)
                });
            }
            dr.Close();

            statusCB.ItemsSource = status;
            statusCB.DisplayMemberPath = "Name";
            statusCB.SelectedValuePath = "ID";
            statusCB.SelectedIndex = 0;
            App.Connection.Close();

            // Filling customer combobox

            App.Connection.Open();
            cmd = new OracleCommand("select * from customer", App.Connection);
            dr = cmd.ExecuteReader();
            List<ComboData> customer = new List<ComboData>();
            while (dr.Read())
            {

                customer.Add(new ComboData()
                {
                    ID = dr.GetInt32(0).ToString(),
                    Name = dr.GetString(1)
                });
            }
            dr.Close();

            customerCB.ItemsSource = customer;
            customerCB.DisplayMemberPath = "Name";
            customerCB.SelectedValuePath = "ID";
            customerCB.SelectedIndex = 0;
            App.Connection.Close();

            // Filling service combobox

            App.Connection.Open();
            cmd = new OracleCommand("select * from service", App.Connection);
            dr = cmd.ExecuteReader();
            List<ComboData> service = new List<ComboData>();
            while (dr.Read())
            {

                service.Add(new ComboData()
                {
                    ID = dr.GetInt32(0).ToString(),
                    Name = dr.GetString(1)
                });
            }
            dr.Close();

            serviceCB.ItemsSource = service;
            serviceCB.DisplayMemberPath = "Name";
            serviceCB.SelectedValuePath = "ID";
            serviceCB.SelectedIndex = 0;
            App.Connection.Close();

        }

        private void dataGridCart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridCart.SelectedIndex != -1)
            {
                DataRow dr = cartData.Rows[dataGridCart.SelectedIndex];
                serviceCB.Text = dr["id"].ToString();
                amount.Text = dr["amount"].ToString();
                LoadData();
                editMode();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

            //jangan lakukan insert dulu, tapi masukkan ke cart terlebih dahulu, jadi kalau ada yang salah,
            //bisa di buang tanpa harus melakukan operasi delete pada oracle
            //kita buat dataRow dulu, yang nanti berisi 1 row dalam tabel
            DataRow dr = cartData.NewRow();
            //ID_SEPATU
            //MERK_SEPATU
            //NAMA_SEPATU
            //HARGA_SEPATU
            //ID_CABANG
            //QUANTITY

            /*
            cartData.Columns.Add("ID");
            cartData.Columns.Add("SERVICE");
            cartData.Columns.Add("AMOUNT");
            */
            dr[0] = serviceCB.SelectedValue.ToString();
            dr[1] = serviceCB.Text;
            dr[2] = amount.Text;
            cartData.Rows.Add(dr);
            //jangan lupa untuk menambahkan total, agar tidak perlu dihitung lagi

            App.Connection.Open();
            OracleCommand cmd = new OracleCommand("SELECT PRICE FROM SERVICE WHERE ID = :id", App.Connection);
            cmd.Parameters.Add(":id", serviceCB.SelectedValue.ToString());
            int price = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            App.Connection.Close();
            count += (Convert.ToInt32(amount.Text) * price);
            total.Text = count.ToString();
            //refresh data grid cart anda
            dataGridCart.ItemsSource = cartData.DefaultView;
            dataGridCart.Columns[0].Visibility = Visibility.Collapsed;

        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            // Subtracting the total
            App.Connection.Open();
            DataRow dr = cartData.Rows[dataGridCart.SelectedIndex];
            OracleCommand cmd = new OracleCommand("SELECT PRICE FROM SERVICE WHERE ID = :id", App.Connection);
            cmd.Parameters.Add(":id", dr["id"].ToString());
            int price = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            App.Connection.Close();
            count -= (Convert.ToInt32(dr["amount"].ToString()) * price);
            total.Text = count.ToString();
            //Removing the data from the dataGridCart
            cartData.Rows.RemoveAt(Convert.ToInt32(dataGridCart.SelectedIndex.ToString()));
            dataGridCart.ItemsSource = cartData.DefaultView;
            exitEditMode();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            serviceCB.SelectedIndex = 0;
            amount.Text = "";
            exitEditMode();
        }

        private void proceed_Click(object sender, RoutedEventArgs e)
        {
            //deklarasi dlu variable transaction nya
            OracleTransaction trans;
            //buka koneksi nya ke oracle
            App.Connection.Open();
            //kita mulai transaction dengan begin transaction
            //beda antara transaction dengan query biasa adalah apapun yang dilakukan selama begin transaction hingga commit
            //smua query baik insert update atau delete, akan ditahan hingga transaction berjalan menemui commit(berhasil) atau rollback(tidak berhasil)
            trans = App.Connection.BeginTransaction();
            try
            {
                OracleCommand cmd = new OracleCommand("insert into transaction_master(fk_customer_id,total) values(:customer,:total) returning id into :lastID", App.Connection);
                cmd.Parameters.Add(":customer", customerCB.SelectedValue.ToString());
                cmd.Parameters.Add(":total", total.Text);
                cmd.Parameters.Add(new OracleParameter("lastID", OracleDbType.Decimal, ParameterDirection.ReturnValue));

                cmd.ExecuteNonQuery();
                String lastID = cmd.Parameters["lastID"].Value.ToString();
                //sekarang kita akan mengisi bagian detail, anda bisa menggunakan foreach atau for biasa
                foreach (DataRow item in cartData.Rows)
                {
                    cmd = new OracleCommand("insert into transaction_detail(fk_transaction_master_id,fk_service_id,amount) values(:master,:service,:amount)", App.Connection);
                    cmd.Parameters.Add(":master", lastID);
                    cmd.Parameters.Add(":service", item["id"].ToString());
                    cmd.Parameters.Add(":amount", item["amount"].ToString());
                    cmd.ExecuteNonQuery();
                }

                //kita bersihkan datagrid dan tabel cart nya
                cartData.Clear();
                dataGridCart.ItemsSource = cartData.DefaultView;

                //jangan lupa sebelum menutup koneksi, agar transaction berjalan dengan baik, maka berikan syntax commit


                count = 0;
                total.Text = count.ToString();
                trans.Commit();
                App.Connection.Close();
                LoadData();
            }
            catch (OracleException ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message);
                //gunakan perintah ini untuk memastikan apabila ada error, maka transaction tidak akan di insert kan.
                trans.Rollback();
                App.Connection.Close();
            }
        }

        private void dataGridTransaction_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGridTransaction.SelectedIndex != -1)
            {
                DataRow dr = transactionData.Tables[0].Rows[dataGridTransaction.SelectedIndex];
                OracleDataAdapter adapter = new OracleDataAdapter("SELECT S.NAME \"SERVICE\",T.AMOUNT FROM TRANSACTION_DETAIL T JOIN SERVICE S ON T.FK_SERVICE_ID = S.ID WHERE FK_TRANSACTION_MASTER_ID = " + dr["id"].ToString(), App.Connection);
                cartTableData = new DataSet();
                adapter.Fill(cartTableData, "TRANSACTION_DETAIL");
                dataGridCart.ItemsSource = cartTableData.Tables["TRANSACTION_DETAIL"].DefaultView;
                statusEditMode();
                loadStatusData();
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            exitStatusEditMode();
            dataGridCart.ItemsSource = cartData.DefaultView;
            dataGridCart.Columns[0].Visibility = Visibility.Collapsed;

        }

        private void updateStatus_Click(object sender, RoutedEventArgs e)
        {

            DataRow dr = transactionData.Tables[0].Rows[dataGridTransaction.SelectedIndex];
            OracleCommand cmd = new OracleCommand("insert into TRANSACTION_STATUS(FK_TRANSACTION_MASTER_ID,FK_STATUS_ID,log_date) values(:master,:status,CURRENT_TIMESTAMP)", App.Connection);

            cmd.Parameters.Add(":master", dr["id"].ToString());
            cmd.Parameters.Add(":status", statusCB.SelectedValue.ToString());
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();
            loadStatusData();
            exitStatusEditMode();
        }
        private void loadStatusData()
        {
            DataRow dr = transactionData.Tables[0].Rows[dataGridTransaction.SelectedIndex];
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT S.NAME, T.LOG_DATE FROM TRANSACTION_STATUS T JOIN STATUS S ON T.FK_STATUS_ID = S.ID WHERE FK_TRANSACTION_MASTER_ID = " + dr["id"].ToString() + "ORDER BY T.ID DESC", App.Connection);
            statusData = new DataSet();
            adapter.Fill(statusData, "status");
            dataGridStatus.ItemsSource = statusData.Tables["status"].DefaultView;
        }
    }
}
