using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace EmploymentAgency
{
    class db
    {
        private MySqlConnection conn;
        private string connStr = "Server=aksib.space; database=u0829589_employment; User=u0829589_samars; password=violin146S; charset=utf8mb4";

        public MySqlConnection newConnection()
        {
            conn = new MySqlConnection(connStr);
            conn.Open();
            return conn;
        }
        public void closeConnection()
        {
            conn.Close();
        }
    }
}
