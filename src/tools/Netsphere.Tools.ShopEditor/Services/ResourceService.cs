using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Media.Imaging;
using ImageMagick;
using Netsphere.Resource;
using Netsphere.Resource.xml;
using Netsphere.Resource.Xml;
using Reactive.Bindings;
using ReactiveUI;

namespace Netsphere.Tools.ShopEditor.Services
{
    public class ResourceService : ReactiveObject
    {
        public static ResourceService Instance { get; } = new ResourceService();

        private readonly S4Zip _zip;

        public Effect[] Effects { get; private set; }
        public Item[] Items { get; private set; }

        public ResourceService()
        {
            _zip = AvaloniaLocator.Current.GetService<S4Zip>();
        }

        public void Load()
        {
            var itemEffectDto = Deserialize<EffectListDto>("xml/item_effect.x7");
            var stringTableDto = Deserialize<StringTableDto>("language/xml/item_effect_string_table.x7");
            Effects = itemEffectDto.item_effect.Select(effectDto =>
            {
                var id = effectDto.effect_id;
                var name = stringTableDto.@string.FirstOrDefault(x => x.key == effectDto.name_key)?.eng ?? effectDto.name_key;
                return new Effect(id, name);
            }).ToArray();

            var itemInfoDto = Deserialize<ItemListDto>("xml/item.x7");
            stringTableDto = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.x7");
            var items = new List<Item>();

            // Filter out entries we definitely dont need to make individual image loading faster
            var imageEntries = _zip.Values
                .Where(x => x.FullName.StartsWith("resources/image/", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (var itemDto in itemInfoDto.item)
            {
                var itemNumber = new ItemNumber(itemDto.item_key);
                var name = stringTableDto.@string.FirstOrDefault(x =>
                               x.key == itemDto.@base.name_key)?.eng ?? itemDto.@base.name;

                var imageName = itemDto.graphic?.icon_image ?? "";
                imageName = Path.GetFileNameWithoutExtension(imageName);
                items.Add(new Item(itemNumber, name, imageName, imageEntries));
            }

            Items = items.ToArray();
        }

        private TDto Deserialize<TDto>(string path)
        {
            var serializer = new XmlSerializer(typeof(TDto));
            using (var ms = new MemoryStream(_zip[path].GetData()))
                return (TDto)serializer.Deserialize(ms);
        }
    }

    public class Effect
    {
        public uint Id { get; }
        public string Name { get; }

        public Effect(uint id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Item
    {
        private readonly ReactiveProperty<Bitmap> _image;
        private readonly S4ZipEntry[] _imageEntries;
        private readonly string _imageName;
        private bool _loadedImage;

        public ItemNumber ItemNumber { get; }
        public string Name { get; }
        public ReactiveProperty<Bitmap> Image
        {
            get
            {
                // Async lazy loading to improve startup performance and reduce memory usage
                if (!_loadedImage)
                {
                    _loadedImage = true;
                    Task.Run(() => LoadImage());
                }

                return _image;
            }
        }

        public Item(ItemNumber itemNumber, string name, string imageName, S4ZipEntry[] imageEntries)
        {
            ItemNumber = itemNumber;
            Name = name;
            _imageEntries = imageEntries;
            _imageName = imageName;
            _image = new ReactiveProperty<Bitmap>();
        }

        private void LoadImage()
        {
            // The resource files not always contain the right extension so just test for both dds and tga
            var ddsImage = $"{_imageName}.dds";
            var tgaImage = $"{_imageName}.tga";
            var imageEntry = _imageEntries.FirstOrDefault(x =>
                x.FullName.EndsWith(ddsImage, StringComparison.OrdinalIgnoreCase) ||
                x.FullName.EndsWith(tgaImage, StringComparison.OrdinalIgnoreCase));

            if (imageEntry != null)
            {
                var readSettings = new MagickReadSettings();

                // Magick.Net seems to have problems detecting tga by itself so manually set the format to tga
                if (imageEntry.Name.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
                    readSettings.Format = MagickFormat.Tga;

                using (var pngStream = new MemoryStream())
                using (var magickImage = new MagickImage(imageEntry.GetData(), readSettings))
                {
                    magickImage.Write(pngStream, MagickFormat.Png32);
                    pngStream.Position = 0;
                    var imageLoaded = new Bitmap(pngStream);

                    RxApp.MainThreadScheduler.Schedule((imageLoaded, Image), (scheduler, state) =>
                    {
                        state.Image.Value = state.imageLoaded;
                        return Disposable.Empty;
                    });
                }
            }
        }
    }
}
