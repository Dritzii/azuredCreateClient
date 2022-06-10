using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace azuredCreateClient.Middleware
{
    class DatabaseConnectioncs
    {
        string connectionstring;
        public DatabaseConnectioncs(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }
        // https://githubhot.com/repo/Azure/Azure-Functions/issues/2064 add this to fix 
        /*
         * The type initializer for 'Microsoft.Data.SqlClient.LocalAppContextSwitches' threw an exception. Could not load file or assembly 'System.Configuration.ConfigurationManager, Version=4.0.3.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'. The system cannot find the file specified.
         * 
         * 
         */
        public List<FirewallClass> GetFirewallfromDB(string firewall = "testdevice")
        {
            Console.WriteLine(firewall);
            List<FirewallClass> list = new List<FirewallClass>();
            string connection = this.connectionstring;
            string sql = String.Format("SELECT subscriptionId, tenantId, displayName, name from firewallseq where name like '%{0}%';", firewall);
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
        public void InsertIntoHistory(string tenantId, string deviceName, string ipAddress, string managementResourcePath, string subscriptionId, string displayName, string fullManagementResourcePath, string jsonbody)
        {
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string connection = this.connectionstring;
            string sql = String.Format("INSERT INTO [dbo].[historyUpdates] VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", tenantId, deviceName, ipAddress, Timestamp, managementResourcePath, subscriptionId, displayName, fullManagementResourcePath, jsonbody);
            using (var cn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();
                }
            }


        }

        public void InsertIntoTenantandSubscriptions(string subscriptionId, string displayName, string tenantId)
        {
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string connection = this.connectionstring;
            string sql = String.Format("INSERT INTO [dbo].[subscriptions] VALUES ('{0}', '{1}', 'Enabled', '{2}', '2019-05-29 18:47:03.0000000', '2019-05-29 18:47:03.0000000')", subscriptionId, displayName, tenantId);
            using (var cn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();
                }
            }


        }

        public void InsertIntoFirewall(string subscriptionId, string name)
        {
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string connection = this.connectionstring;
            string sql = String.Format("INSERT INTO [dbo].[firewalls] VALUES ('{0}', '{1}', 'createdurl.com', '0.0.0', '0.0.0', '0 days', '2019-05-28 12:13:52.0000000', '2019-05-28 12:13:52.0000000')", subscriptionId, name);
            using (var cn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();
                }
            }
        }
        public bool subInDb(string subscriptionId = "")
        {
            string connection = this.connectionstring;
            string sql = String.Format("SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS [Value] FROM firewallseq where subscriptionId like '%{0}%';", subscriptionId);
            using (var cn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();


                    while (reader.Read()) {
                        if (reader.GetInt32(0) == 0)
                        {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public bool tenantInDB(string tenantId = "")
        {
            string connection = this.connectionstring;
            string sql = String.Format("SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS [Value] FROM [firewallseq] where tenantId like '%{0}%';", tenantId);
            using (var cn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public bool firewallInDB(string firewall = "")
        {
            string connection = this.connectionstring;
            string sql = String.Format("SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS [Value] FROM firewalls where name like '%{0}%';", firewall);
            using (var cn = new SqlConnection(connection))
            {
                using (var cmd = new SqlCommand() { Connection = cn, CommandText = sql })
                {
                    cn.Open();

                    var reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
    }

}
