using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace azuredCreateClient.Middleware
{
    class DatabaseConnectioncs
    {
        string server;
        string user;
        string password;
        string database;

        public DatabaseConnectioncs()
        {
        }

        public static List<FirewallClass> GetFirewallfromDB( string server, string user, string password, string database)
        {
            List<FirewallClass> list = new List<FirewallClass>();
            SqlConnectionStringBuilder myBuilder = new SqlConnectionStringBuilder();
            myBuilder.DataSource = server;
            myBuilder.UserID = user;
            myBuilder.Password = password;
            myBuilder.InitialCatalog = database;
            using SqlConnection connection = new SqlConnection(myBuilder.ConnectionString);
            String sql = "SELECT s.subscriptionId," +
                    " s.tenantId," +
                    " s.displayName," +
                    " f.name," +
                    " f.uri " +
                    "from subscriptions s inner join [firewalls] f on s.subscriptionId = f.subscriptionId";
            using (var cn = new SqlConnection(sql))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new FirewallClass()
                        {
                            subscriptionId = reader.GetString(0),
                            tenantId = reader.GetString(1),
                            displayName = reader.GetString(2),
                            name = reader.GetString(3),
                            uri = reader.GetString(4)
                        });
                    }
                }
            }

            return list;

        }
    }

}
