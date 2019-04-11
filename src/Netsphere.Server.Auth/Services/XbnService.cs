using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using BlubLib.Collections.Generic;
using BlubLib.IO;
using Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Netsphere.Server.Auth.Services
{
    public class XbnService : IHostedService
    {
        private readonly ILogger<XbnService> _logger;
        private readonly AppOptions _options;
        private readonly Dictionary<XBNType, byte[]> _cache;

        public XbnService(ILogger<XbnService> logger, IOptions<AppOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _cache = new Dictionary<XBNType, byte[]>();
        }

        public byte[] GetData(XBNType xbnType)
        {
            return _cache.GetValueOrDefault(xbnType);
        }

        public IReadOnlyDictionary<XBNType, byte[]> GetData()
        {
            return _cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Generating xbn files...");

            foreach (XBNType xbnType in Enum.GetValues(typeof(XBNType)))
            {
                string fileName;
                switch (xbnType)
                {
                    case XBNType.ConstantInfo:
                        fileName = "xml/constant_info.x7";
                        break;

                    case XBNType.Actions:
                        fileName = "xml/action.x7";
                        break;

                    case XBNType.Weapons:
                        fileName = "xml/_eu_weapon.x7";
                        break;

                    case XBNType.Effects:
                        fileName = "xml/effect_list.x7";
                        break;

                    case XBNType.EffectMatch:
                        fileName = "xml/effect_match_list.x7";
                        break;

                    case XBNType.EnchantData:
                        fileName = "xml/enchant_data.x7";
                        break;

                    case XBNType.EquipLimit:
                        fileName = "xml/equip_limit.x7";
                        break;

                    case XBNType.MonsterStatus:
                        fileName = "xml/monster_status.x7";
                        break;

                    case XBNType.MonsterMapMiddle:
                        fileName = "xml/monster_wave/monster_map_middle.x7";
                        break;

                    default:
                        throw new Exception("Invalid xbn type: " + xbnType);
                }

                using (var xmlReader = new XmlTextReader(Path.Combine(_options.DataDir, fileName)))
                {
                    var xml = XDocument.Load(xmlReader);

                    using (var w = new MemoryStream().ToBinaryWriter(false))
                    {
                        w.Write(-1);
                        w.Write(0.1f);

                        if (xml.Root == null)
                        {
                            w.Write((ushort)0);
                            w.Write((ushort)0);
                        }
                        else
                        {
                            var nodeNames = GetNodeNames(xml.Root);
                            w.Write((ushort)nodeNames.Count);
                            foreach (var nodeName in nodeNames)
                                WriteXBNValue(w, nodeName);

                            var attribNames = GetAttributeNames(xml.Root);
                            w.Write((ushort)attribNames.Count);
                            foreach (var attribName in attribNames)
                                WriteXBNValue(w, attribName);

                            WriteXBNNode(w, xml.Root, nodeNames, attribNames);
                        }

                        _cache[xbnType] = w.ToArray();
                    }
                }
            }

            _logger.Information("Cached {Count} xbn files...", _cache.Count);
            return Task.CompletedTask;

            IList<string> GetNodeNames(XElement element)
            {
                var names = new List<string> { element.Name.LocalName };
                foreach (var subNode in element.Elements())
                    names.AddRange(GetNodeNames(subNode).Where(e => !names.Contains(e)));

                return names;
            }

            IList<string> GetAttributeNames(XElement element)
            {
                var names = element.Attributes().Select(attrib => attrib.Name.LocalName).ToList();
                foreach (var subNode in element.Elements())
                    names.AddRange(GetAttributeNames(subNode).Where(e => !names.Contains(e)));

                return names;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private static void WriteXBNValue(BinaryWriter w, string value)
        {
            var data = ProudNet.Constants.Encoding.GetBytes(value);

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)((data[i] >> 6) | (data[i] << 2));
                data[i] = (byte)~data[i];
            }

            w.Write((ushort)data.Length);
            w.Write(data);
        }

        private static void WriteXBNAttribute(BinaryWriter w, XAttribute attribute, IList<string> attribNames)
        {
            var nameIndex = attribNames.IndexOf(attribute.Name.LocalName);
            w.Write((ushort)nameIndex);
            WriteXBNValue(w, attribute.Value);
        }

        private static void WriteXBNNode(BinaryWriter w, XElement node, IList<string> nodeNames, IList<string> attribNames)
        {
            var nameIndex = nodeNames.IndexOf(node.Name.LocalName);

            w.Write((ushort)nameIndex);

            w.Write((ushort)node.Attributes().Count());
            w.Write((ushort)0); // TODO unk
            w.Write((ushort)node.Elements().Count());

            foreach (var attrib in node.Attributes())
                WriteXBNAttribute(w, attrib, attribNames);

            foreach (var subNode in node.Elements())
                WriteXBNNode(w, subNode, nodeNames, attribNames);
        }
    }
}
