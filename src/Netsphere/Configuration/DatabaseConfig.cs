using Newtonsoft.Json;

namespace Netsphere.Configuration
{
    public class DatabaseConfig
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("database")]
        public string Database { get; set; }

        [JsonProperty("run_migration")]
        public bool RunMigration { get; set; }

        public DatabaseConfig()
        {
            Host = "localhost";
            Port = 3306;
            Username = "root";
            Password = "root";
        }
    }
}
