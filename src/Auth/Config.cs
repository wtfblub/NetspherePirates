using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Netsphere
{
    public class Config
    {
        private static readonly string s_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.hjson");

        public static Config Instance { get; }

        [JsonProperty("listener")]
        [JsonConverter(typeof(IPEndPointConverter))]
        public IPEndPoint Listener { get; set; }

        [JsonProperty("max_connections")]
        public int MaxConnections { get; set; }

        [JsonProperty("api")]
        public APIConfig API { get; set; }

        [JsonProperty("noob_mode")]
        public bool NoobMode { get; set; }

        [JsonProperty("auto_register")]
        public bool AutoRegister { get; set; }

        [JsonProperty("database")]
        public DatabasesConfig Database { get; set; }

        static Config()
        {
            if (!File.Exists(s_path))
            {
                Instance = new Config();
                Instance.Save();
                return;
            }

            using (var fs = new FileStream(s_path, FileMode.Open, FileAccess.Read))
                Instance = JsonConvert.DeserializeObject<Config>(Hjson.HjsonValue.Load(fs).ToString(Hjson.Stringify.Plain));
        }

        public Config()
        {
            Listener = new IPEndPoint(IPAddress.Loopback, 28002);
            MaxConnections = 100;
            API = new APIConfig();
            NoobMode = true;
            AutoRegister = false;
            Database = new DatabasesConfig();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.None);
            File.WriteAllText(s_path, Hjson.JsonValue.Parse(json).ToString(Hjson.Stringify.Hjson));
        }
    }

    public class APIConfig
    {
        [JsonProperty("listener")]
        [JsonConverter(typeof(IPEndPointConverter))]
        public IPEndPoint Listener { get; set; }

        [JsonProperty("serverlist_timeout")]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Timeout { get; set; }

        public APIConfig()
        {
            Listener = new IPEndPoint(IPAddress.Loopback, 27000);
            Timeout = TimeSpan.FromSeconds(30);
        }
    }

    public class DatabasesConfig
    {
        [JsonProperty("auth")]
        public DatabaseConfig Auth { get; set; }

        public DatabasesConfig()
        {
            Auth = new DatabaseConfig
            { 
                Host = "localhost",
                Port = 3306,
                Username = "root",
                Password = "root",
                Database = "auth"
            };
        }

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

            public DatabaseConfig()
            {
                Host = "localhost";
                Port = 3306;
            }
        }
    }
}
