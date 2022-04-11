using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace azuredCreateClient.Middleware
{
    class DatabaseConnectioncs
    {
        private static List<string> retList;
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

        public static List<string> GetFirewallfromDB(string firewall)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(builder.ConnectionString);
                Console.WriteLine("\nQuery data example:");
                Console.WriteLine("=========================================\n");

                String sql = "SELECT s.subscriptionId," +
                    " s.tenantId," +
                    " s.displayName," +
                    " f.name," +
                    " f.uri " +
                    "from subscriptions s inner join firewalls f on s.subscriptionId = f.subscriptionId";

                using SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                List<FirewallClass> results = new List<FirewallClass>();
                while (reader.Read())
                {
                    var retList = new List<string>();
                    FirewallClass newItem = new FirewallClass();
                    newItem.subscriptionId = (string)reader.GetValue(0);
                    newItem.subscriptionId = (string)reader.GetValue(1);
                    newItem.subscriptionId = (string)reader.GetValue(2);
                    newItem.subscriptionId = (string)reader.GetValue(3);
                    newItem.subscriptionId = (string)reader.GetValue(4);
                    results.Add(newItem);
                }
                return retList;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }

}
