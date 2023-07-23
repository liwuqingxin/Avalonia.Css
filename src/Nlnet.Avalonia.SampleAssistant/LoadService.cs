using Avalonia.Platform;
using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nlnet.Avalonia.SampleAssistant
{
    public static class LoadService
    {
        public static ICaseXamlParser XmlParser = new XCaseXamlParser<Case>();



        #region Load Gallery

        public static async Task<IEnumerable<GalleryItem>> GetGalleryItemAsync(Type sink)
        {
            return await Task.Run(() =>
            {
                var list     = new List<GalleryItem>();
                var assembly = sink.Assembly;
                var types = assembly
                    .GetTypes()
                    .Where(t => !t.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true))
                    .Where(t => t.GetTypeInfo().IsDefined(typeof(GalleryItemAttribute), true))
                    .ToList();

                types.Sort((t1, t2) =>
                {
                    var attr1 = t1.GetCustomAttribute<GalleryItemAttribute>();
                    var attr2 = t2.GetCustomAttribute<GalleryItemAttribute>();

                    if (attr1!.Kind == attr2!.Kind)
                    {
                        return string.Compare(attr1.Name, attr2.Name, StringComparison.Ordinal);
                    }

                    return attr1.Kind - attr2.Kind;
                });

                foreach (var type in types)
                {
                    AddDemo(list, type);
                }

                return list;
            });
        }

        private static void AddDemo(ICollection<GalleryItem> list, Type type)
        {
            try
            {
                var attr = type.GetCustomAttribute<GalleryItemAttribute>();
                if (attr == null)
                {
                    return;
                }
                var attrDescription = type.GetCustomAttribute<DescriptionAttribute>();
                var item            = new GalleryItem(attr.Name, attr.Icon, type, attr.Kind, attrDescription?.Description, GetXaml(type));
                list.Add(item);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private static string? GetXaml(Type type)
        {
            try
            {
                var resourceUriString = $"resm:{type.FullName}.axaml";

                using var stream = AssetLoader.Open(new Uri(resourceUriString));

                var bytes = new byte[stream.Length];
                var length = stream.Read(bytes, 0, (int)stream.Length);
                var xaml = Encoding.Default.GetString(bytes);

                return xaml;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
        }

        #endregion
    }
}
