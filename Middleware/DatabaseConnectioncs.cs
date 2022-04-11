using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

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
            
        }

        public static List<string> GetFirewallfromDB(string firewall)
        {
            return List<string>;
        }
    }

}
