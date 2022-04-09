using MySqlConnector;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azuredCreateClient.Middleware
{
    class DatabaseConnectioncs
    {
        readonly string server;
        readonly string user;
        readonly string password;
        readonly string database;
        private SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        public DatabaseConnectioncs(string server, string user, string password, string database)
        {
            this.server = server;
            this.user = user;
            this.password = password;
            this.database = database;
            builder.DataSource = this.server;
            builder.UserID = this.user;
            builder.Password = this.password;
            builder.InitialCatalog = this.database;
        }

        public void GetConnection()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(builder.ConnectionString);
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = "SELECT name, collation_name FROM sys.databases";

                using SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }

}
