using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Netsphere
{
    public class Config
    {
        public static Config Instance { get; private set; }

        [JsonProperty("listener")]
        [JsonConverter(typeof(IPEndPointConverter))]
        public IPEndPoint Listener { get; set; }

        [JsonProperty("max_connections")]
        public int MaxConnections { get; set; }

        [JsonProperty("web_api")]
        public WebAPIConfig WebAPI { get; set; }

        [JsonProperty("noob_mode")]
        public bool NoobMode { get; set; }

        [JsonProperty("auto_register")]
        public bool AutoRegister { get; set; }

        [JsonProperty("auth_database")]
        public DatabaseSettings AuthDatabase { get; set; }

        static Config()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json");
            if (!File.Exists(path))
            {
                var json = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
                File.WriteAllText(path, json);
            }

            Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        public Config()
        {
            Listener = new IPEndPoint(IPAddress.Loopback, 28002);
            MaxConnections = 100;
            WebAPI = new WebAPIConfig();
            NoobMode = true;
            AutoRegister = false;
            AuthDatabase = new DatabaseSettings { Filename = "..\\db\\auth.db" };
        }
    }

    public class WebAPIConfig
    {
        [JsonProperty("listener")]
        public string Listener { get; set; }

        [JsonProperty("serverlist_timeout")]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Timeout { get; set; }

        public WebAPIConfig()
        {
            Listener = "http://localhost:27000";
            Timeout = TimeSpan.FromSeconds(30);
        }
    }

    public class DatabaseSettings
    {
        [JsonProperty("engine")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseEngine Engine { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("database")]
        public string Database { get; set; }

        public DatabaseSettings()
        {
            Engine = DatabaseEngine.SQLite;
        }
    }

    public enum DatabaseEngine
    {
        SQLite,
        MySQL,
        PostgreSQL
    }
}
