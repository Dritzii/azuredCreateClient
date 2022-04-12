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
        // https://githubhot.com/repo/Azure/Azure-Functions/issues/2064 add this to fix 
        /*
         * The type initializer for 'Microsoft.Data.SqlClient.LocalAppContextSwitches' threw an exception. Could not load file or assembly 'System.Configuration.ConfigurationManager, Version=4.0.3.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'. The system cannot find the file specified.
         * 
         * 
         */
        public static List<FirewallClass> GetFirewallfromDB(string firewall = "" ,string connectionstring = "Server=arazured.database.windows.net,1433;Initial Catalog=fwaasapplication;User ID=aradmin;Password=Aqualite12@;")
        {
            List<FirewallClass> list = new List<FirewallClass>();
            string connection = connectionstring;
            Console.WriteLine(connection);
            string sql = String.Format("SELECT subscriptionId, tenantId, displayName, name, uri from firewallseq where name like '%{0}%';", firewall);
            Console.WriteLine(sql);
            using (var cn = new SqlConnection(connection))
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
