using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azuredCreateClient.Middleware
{
    class DatabaseConnectioncs
    {
        string server;
        string user;
        string password;
        string database;
        private MySqlConnection _connect;

        public DatabaseConnectioncs(string server, string user, string password, string database)
        {
            this.server = server;
            this.user = user;
            this.password = password;
            this.database = database;
            this._connect = new MySqlConnection(server + ";" + user + ";" + password + ";" + database);
        }

        public async Task GetConnectionAsync()
        {
            await _connect.OpenAsync();
            using var command = new MySqlCommand("SELECT field FROM table;", _connect);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var value = reader.GetValue(0);
                // do something with 'value'
            }
        }

        public async Task InsertIntoSubscriptions()
        {

            await _connect.OpenAsync();

            // Insert some data
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = _connect;
                cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                cmd.Parameters.AddWithValue("p", "Hello world");
                await cmd.ExecuteNonQueryAsync();
            }

            // Retrieve all rows
            using (var cmd = new MySqlCommand("SELECT some_field FROM data", _connect))
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    Console.WriteLine(reader.GetString(0));
            
        }
    }
}
