using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Oracle.DataAccess.Client;

namespace Project_Albus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application    {
        public static OracleConnection Connection =
            new OracleConnection("Data source=orcl2;User id=langit;Password=langit");
    }
}
