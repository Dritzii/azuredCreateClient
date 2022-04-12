using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace azuredCreateClient.Middleware
{
    class DatabaseConnectioncs
    {
        public DatabaseConnectioncs()
        {
        }

        public static List<FirewallClass> GetFirewallfromDB( string server= "arazured.database.windows.net", string user = "fwaasupdates_prod@azuredaseproddb", string password= "Aqualite12@", string database= "fwaasapplication")
        {
            List<FirewallClass> list = new List<FirewallClass>();
            SqlConnectionStringBuilder myBuilder = new SqlConnectionStringBuilder();
            myBuilder.DataSource = server;
            myBuilder.UserID = user;
            myBuilder.Password = password;
            myBuilder.InitialCatalog = database;
            using SqlConnection connection = new SqlConnection(myBuilder.ConnectionString);
            string sql = "SELECT subscriptionId, tenantId, displayName, name, uri from firewallseq;";
            Console.WriteLine(sql);
            using (var cn = new SqlConnection(sql))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
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
