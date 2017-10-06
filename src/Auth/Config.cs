using System;
using System.Net;
using Netsphere.Configuration;
using Newtonsoft.Json;

namespace Netsphere
{
    public class Config
    {
        public static Config Instance => Config<Config>.Instance;

        [JsonProperty("listener")]
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
            Config<Config>.Save();
        }
    }

    public class APIConfig
    {
        [JsonProperty("listener")]
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
            Auth = new DatabaseConfig { Database = "auth" };
        }
    }
}
