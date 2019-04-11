using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Netsphere.Resource;

namespace DataExtractor
{
    internal class Program
    {
        private static void Main()
        {
            var filesToExtract = new[]
            {
                "language/xml/gameinfo_string_table.x7",
                "language/xml/item_effect_string_table.x7",
                "language/xml/iteminfo_string_table.x7",

                "resources/mapinfo/(.*).ini$",

                "xml/_eu_gameinfo.x7",
                "xml/constant_info.x7",
                "xml/default_item.x7",
                "xml/equip_limit.x7",
                "xml/experience.x7",
                "xml/effect_list.x7",
                "xml/effect_match_list.x7",
                "xml/action.x7",
                "xml/_eu_weapon.x7",
                "xml/item.x7",
                "xml/enchant_data.x7",
                "xml/monster_status.x7",
                "xml/monster_wave/monster_map_middle.x7"
            };
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource.s4hd");
            if (!File.Exists(path))
            {
                Error("You have to place the extractor in the S4 League folder!");
                Exit();
            }

            S4Zip zip = null;
            try
            {
                zip = S4Zip.OpenZip(path);
            }
            catch
            {
                Error("Failed to read resources. Are you using the right S4 League version?");
                Exit();
            }

            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (Directory.Exists(dataPath))
                Directory.Delete(dataPath, true);

            Directory.CreateDirectory(dataPath);

            foreach (var toExtract in filesToExtract)
            {
                var entries = zip.Where(pair => Regex.IsMatch(pair.Key, toExtract)).Select(pair => pair.Value).ToArray();

                if (entries.Length == 0)
                {
                    Warn($"'{toExtract}' not found!");
                    continue;
                }

                foreach (var entry in entries)
                {
                    byte[] data;
                    try
                    {
                        data = entry.GetData();
                    }
                    catch (Exception ex)
                    {
                        Error($"Failed to extract '{entry.FullName}'");
                        Error(ex.ToString());
                        continue;
                    }

                    Info($"Extracting '{entry.FullName}'...");
                    var saveTo = entry.FullName
                        .Replace('\\', Path.DirectorySeparatorChar)
                        .Replace('/', Path.DirectorySeparatorChar);
                    saveTo = Path.Combine(dataPath, saveTo);

                    var dir = Path.GetDirectoryName(saveTo);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.WriteAllBytes(saveTo, data);
                }
            }

            Info("Done.");
            Exit();
        }

        private static void Info(string message)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("INFO");
            Console.ResetColor();
            Console.Write("] ");
            Console.WriteLine(message);
        }

        private static void Warn(string message)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("WARNING");
            Console.ResetColor();
            Console.Write("] ");
            Console.WriteLine(message);
        }

        private static void Error(string message)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
            Console.ResetColor();
            Console.Write("] ");
            Console.WriteLine(message);
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
