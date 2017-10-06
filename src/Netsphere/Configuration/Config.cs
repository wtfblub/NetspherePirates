using System;
using System.IO;
using BlubLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Netsphere.Configuration
{
    public static class Config<T> where T : new()
    {
        private static string s_path;

        public static T Instance { get; private set; }
        public static JObject Json { get; private set; }

        public static void Initialize(string fileName, string env = null)
        {
            if (!string.IsNullOrWhiteSpace(env))
                env = Environment.GetEnvironmentVariable(env);

            s_path = env ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(s_path))
            {
                Instance = FastActivator<T>.Create();
                Save();
            }

            var str = File.ReadAllText(s_path);
            str = Hjson.HjsonValue.Parse(str).ToString(Hjson.Stringify.Plain);
            Json = JObject.Parse(str);
            Instance = Json.ToObject<T>();
        }

        public static void Save(JObject json = null)
        {
            var str = json == null
                ? JsonConvert.SerializeObject(Instance, Formatting.None)
                : json.ToString(Formatting.None);

            File.WriteAllText(s_path, Hjson.JsonValue.Parse(str).ToString(Hjson.Stringify.Hjson));
        }
    }
}
