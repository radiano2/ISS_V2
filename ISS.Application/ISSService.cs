using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SQLitePCL;


namespace ISS.Application
{
    public class ISSService
    {
        private readonly ILogger _logger;


        private string connectionString =
            (@"Data Source=C:\Users\aradionov\RiderProjects\ISS_V2\ISS.WebApi\Database\issCoordinates.db");


        public ISSService(ILogger logger)
        {
            _logger = logger;
        }

        public void Create()
        {
            Console.WriteLine("Method created started");
            try
            {
                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();

                    var createCommand = connection.CreateCommand();

                    createCommand.CommandText =
                        @"CREATE TABLE tracking (id INTEGER PRIMARY KEY AUTOINCREMENT,latitude DOUBLE,longitude DOUBLE,timestamp TEXT)";

                    createCommand.ExecuteNonQuery();

                    connection.Close();
                }

                Console.WriteLine("Method Add finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogInformation(ex.ToString());
                throw;
            }

            Console.WriteLine("Method created ended");
        }

        public void Add()
        {
            Console.WriteLine("Method Add started");
            try
            {
                using (SqliteConnection connection = new(connectionString))
                {
                    var webclient = new WebClient();
                    var jsonData = webclient.DownloadString("http://api.open-notify.org/iss-now.json");

                    Console.WriteLine(jsonData);

                    dynamic data = JObject.Parse(jsonData);

                    connection.Open();

                    var addCommand = connection.CreateCommand();

                    addCommand.CommandText = @"INSERT INTO tracking (latitude, longitude, timestamp)
                 VALUES (@param1,@param2,@param3)";

                    addCommand.Parameters.Add(new SqliteParameter("@param1",
                        Convert.ToDouble(data.iss_position.latitude)));
                    addCommand.Parameters.Add(new SqliteParameter("@param2",
                        Convert.ToDouble(data.iss_position.longitude)));
                    addCommand.Parameters.Add(new SqliteParameter("@param3", data.timestamp.ToString()));

                    addCommand.ExecuteNonQuery();

                    connection.Close();
                }

                Console.WriteLine("Method Add finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogInformation(ex.ToString());
                throw;
            }
        }

        public void Drop()
        {
            Console.WriteLine("Method drop started");
            try
            {
                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();

                    var dropCommand = connection.CreateCommand();
                    dropCommand.CommandText = @"DROP TABLE tracking";
                    dropCommand.ExecuteNonQuery();

                    connection.Close();
                }

                Console.WriteLine("Method drop finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogInformation(ex.ToString());
                throw;
            }
        }

        public List<string?> Select()
        {
            try
            {
                Console.WriteLine("Method select started");
                List<string?> importedFiles = new();

                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();

                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = @"SELECT timestamp FROM tracking";
                    selectCommand.CommandType = CommandType.Text;
                    SqliteDataReader reader = selectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        importedFiles.Add(Convert.ToString(reader["timestamp"]));
                    }

                    connection.Close();

                    Console.WriteLine("Method select Ended");

                    return importedFiles;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogInformation(ex.ToString());
                throw;
            }
        }
    }
}