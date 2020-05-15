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
    /// Interaction logic for Master_Pegawai.xaml
    /// </summary>
    public partial class Master_Pegawai : Window
    {
        public Master_Pegawai()
        {
            InitializeComponent();
            LoadData();
        }

        private DataSet ds;

        private class Department
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }


        private void editMode()
        {
            insert.IsEnabled = false;
            update.IsEnabled = true;
            suspend.IsEnabled = true;
        }
        private void exitEditMode()
        {
            insert.IsEnabled = true;
            update.IsEnabled = false;
            suspend.IsEnabled = false;
        }
        private void LoadData()
        {
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT E.ID \"ID\", E.NAME \"NAME\", D.NAME \"DEPARTMENT\", E.ADDRESS, E.USERNAME, E.PASSWORD, E.STATUS FROM EMPLOYEE E JOIN DEPARTMENT D ON E.FK_DEPARTMENT_ID = D.ID", App.Connection);
            ds = new DataSet();
            adapter.Fill(ds, "employee");
            dataGrid.ItemsSource = ds.Tables["employee"].DefaultView;

            App.Connection.Open();

            OracleCommand cmd = new OracleCommand("select * from department", App.Connection);
            OracleDataReader dr = cmd.ExecuteReader();
            List<Department> department = new List<Department>();
            while (dr.Read())
            {

                department.Add(new Department()
                {
                    ID = dr.GetInt32(0).ToString(),
                    Name = dr.GetString(1)
                });
            }
            dr.Close();

            departmentCB.ItemsSource = department;
            departmentCB.DisplayMemberPath = "Name";
            departmentCB.SelectedValuePath = "ID";
            departmentCB.SelectedIndex = 0;
            App.Connection.Close();
        }
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                DataRow dr = ds.Tables[0].Rows[dataGrid.SelectedIndex];
                id.Text = dr["id"].ToString();
                name.Text = dr["name"].ToString();
                departmentCB.Text = dr["department"].ToString();
                address.Text = dr["address"].ToString();
                username.Text = dr["username"].ToString();
                password.Text = dr["password"].ToString();
                var bc = new BrushConverter();
                if (dr["status"].ToString() == "0")
                {
                    suspend.Content = "ACTIVATE";
                    suspend.Background = (Brush)bc.ConvertFrom("#FF4BA84F");
                }
                else
                {
                    suspend.Content = "SUSPEND";
                    suspend.Background = (Brush)bc.ConvertFrom("#FFE74848");
                }
                editMode();
            }
        }
        private void insert_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("insert into employee(FK_DEPARTMENT_ID,name,username,password,address,status) values(:department,:name,:username,:password,:address,'1')", App.Connection);
            cmd.Parameters.Add(":department", departmentCB.SelectedValue.ToString());
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":username", username.Text);
            cmd.Parameters.Add(":password", password.Text);
            cmd.Parameters.Add(":address", address.Text);
            App.Connection.Open();
            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = new OracleCommand("UPDATE EMPLOYEE SET FK_DEPARTMENT_ID=:department,name=:name,username=:username,password=:password,address=:address WHERE ID=:id", App.Connection);
            cmd.Parameters.Add(":department", departmentCB.SelectedValue.ToString());
            cmd.Parameters.Add(":name", name.Text);
            cmd.Parameters.Add(":username", username.Text);
            cmd.Parameters.Add(":password", password.Text);
            cmd.Parameters.Add(":address", address.Text);
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();

            LoadData();
        }

        private void suspend_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd;
            if (suspend.Content == "SUSPEND")
            {
                cmd = new OracleCommand("UPDATE EMPLOYEE SET STATUS='0' WHERE ID=:id", App.Connection);
            }
            else
            {
                cmd = new OracleCommand("UPDATE EMPLOYEE SET STATUS='1' WHERE ID=:id", App.Connection);
            }
            cmd.Parameters.Add(":id", id.Text);
            App.Connection.Open();

            cmd.ExecuteNonQuery();
            App.Connection.Close();
            LoadData();
            exitEditMode();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            departmentCB.SelectedIndex = 0;
            id.Text = "";
            name.Text = "";
            username.Text = "";
            password.Text = "";
            address.Text = "";
            exitEditMode();
        }
    }
}
