using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Avalonia;

namespace Nlnet.Avalonia.Css;

public interface IRiderSettingsBuilder : IService
{
    /// <summary>
    /// Build a rider settings file to support acss language according to current acss context. <br/>
    /// Now we do not provide language supporting plugin in rider. Use this simple rider setting instead now. <br/>
    /// This function will try to create a setting file in rider's folder which is like
    /// "C:\Users\user\AppData\Roaming\JetBrains\Rider2023.1\filetypes\Acss.xml". <br/>
    /// If failed, you could handle it with the <see cref="output"/>, <see cref="setting"/> and <see cref="exceptionHandler"/>.
    /// </summary>
    /// <param name="output">The out put file path if succeed.</param>
    /// <param name="setting">The setting file xml content.</param>
    /// <param name="exceptionHandler">Exception handler that could be null.</param>
    /// <returns></returns>
    public bool TryBuildRiderSettingsForAcss(out string? output, out string? setting, Action<Exception>? exceptionHandler = null);
}

internal class RiderSettingsBuilder : IRiderSettingsBuilder
{
    private readonly IAcssContext _context;

    public RiderSettingsBuilder(IAcssContext context)
    {
        _context = context;
    }

    public void Initialize()
    {

    }

    public bool TryBuildRiderSettingsForAcss(out string? output, out string? setting, Action<Exception>? exceptionHandler = null)
    {
        try
        {
            var typeResolverManager = _context.GetService<ITypeResolverManager>();
            var types = typeResolverManager
                .GetAllTypes()
                .Distinct()
                .ToList();

            // classes
            var typeNames = types
                .Select(t => t.Name)
                .ToList();
            var classBuilder = new StringBuilder();
            var classKeywords = classBuilder.AppendJoin(';', typeNames).ToString();

            // properties
            var fieldsBuilder = new StringBuilder();
            var list = new List<string>();
            foreach (var t in types)
            {
                var fields = t.GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where(f => f.FieldType.IsAssignableTo(typeof(AvaloniaProperty)))
                    .Select(f => f.Name[..^8]);

                list.AddRange(fields);
            }

            fieldsBuilder.AppendJoin(';', list.Distinct());
            var propertyKeywords = fieldsBuilder.ToString();

            // rider setting
            var riderSetting = new RiderSetting();
            riderSetting.highlighting.keywords3.keywords = classKeywords;
            riderSetting.highlighting.keywords4.keywords = propertyKeywords;

            setting = SerializeRiderSetting(riderSetting);

            // Write to file [C:\Users\72975\AppData\Roaming\JetBrains\Rider2023.1\filetypes\Acss.xml]
            var jetBrainsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/JetBrains/";
            var riderPath = Directory.GetDirectories(jetBrainsPath).FirstOrDefault(d => d.Contains("Rider"));
            var fileTypePath = $"{riderPath}/filetypes";
            output = $"{fileTypePath}/Acss.xml";
            Directory.CreateDirectory(fileTypePath);

#if DEBUG
            File.WriteAllText(output, setting);
#endif

            return true;
        }
        catch (Exception e)
        {
            setting = null;
            output = null;
            exceptionHandler?.Invoke(e);
            return false;
        }
    }
    
    private static string SerializeRiderSetting(RiderSetting setting)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            NewLineChars = "\r\n",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = true
        };

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        var builder = new StringBuilder();
        using var writer = XmlWriter.Create(builder, settings);
        var serializer = new XmlSerializer(typeof(RiderSetting));
        serializer.Serialize(writer, setting, namespaces);
        var xml = builder.ToString();

        return xml;
    }
}