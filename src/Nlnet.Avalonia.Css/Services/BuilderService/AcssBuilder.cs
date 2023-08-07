using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

public class AcssBuilder : IAcssBuilder
{
    #region Default ICssBuilder

    private static int           _prepared;
    private static IAcssBuilder? _default;

    public static IAcssBuilder Default
    {
        get
        {
            if (_default == null)
            {
                throw new InvalidOperationException($"{nameof(AcssExtension)}.{nameof(AcssExtension.UseAcssDefaultBuilder)}() or {nameof(AcssBuilder)}.{nameof(AcssBuilder.UseDefaultBuilder)}() must be called before accessing the {nameof(AcssBuilder)}.{nameof(Default)}.");
            }
            return _default;
        }
    }

    /// <summary>
    /// Use default <see cref="IAcssBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static IAcssBuilder UseDefaultBuilder()
    {
        if (Interlocked.Exchange(ref _prepared, 1) == 0)
        {
            _default = new AcssBuilder();
        }

        return _default!;
    }

    #endregion

    
    
    public AcssBuilder()
    {
        _parser          = new AcssParser(this);
        _interpreter     = new AcssInterpreter(this);
        _sectionFactory  = new AcssSectionFactory(this);
        _resourceFactory = new AcssResourceFactory(this);
    }



    #region ICssBuilder

    private IAcssBuilder Internal => (IAcssBuilder)this;

    private readonly IAcssParser _parser;
    private readonly IAcssInterpreter _interpreter;
    private readonly IAcssSectionFactory _sectionFactory;
    private readonly IAcssResourceFactory _resourceFactory;



    private IAcssLoader? _loader;
    private readonly ConcurrentDictionary<string, IAcssFile> _files = new();

    IAcssParser IAcssBuilder.Parser => _parser;

    IAcssInterpreter IAcssBuilder.Interpreter => _interpreter;

    IAcssSectionFactory IAcssBuilder.SectionFactory => _sectionFactory;

    IAcssResourceFactory IAcssBuilder.ResourceFactory => _resourceFactory;

    ITypeResolverManager IAcssBuilder.TypeResolver { get; } = new TypeResolverManager();

    IValueParsingTypeAdapterManager IAcssBuilder.ValueParsingTypeAdapter { get; } = new ValueParsingTypeAdapterManager();

    IResourceProvidersManager IAcssBuilder.ResourceProvidersManager { get; } = new ResourceProvidersManager();

    IBehaviorDeclarerManager IAcssBuilder.BehaviorDeclarerManager { get; } = new BehaviorDeclarerManager();

    IBehaviorResolverManager IAcssBuilder.BehaviorResolverManager { get; } = new BehaviorResolverManager();

    bool IAcssBuilder.TryAddAcssFile(IAcssFile file)
    {
        if (_files.TryGetValue(file.StandardFilePath, out _))
        {
            return false;
        }

        _files.TryAdd(file.StandardFilePath, file);
        return true;
    }

    bool IAcssBuilder.TryRemoveAcssFile(IAcssFile file)
    {
        return _files.TryRemove(file.StandardFilePath, out _);
    }

    bool IAcssBuilder.TryGetAcssFile(string standardFilePath, out IAcssFile? file)
    {
        if (_files.TryGetValue(standardFilePath, out var f))
        {
            file = f;
            return true;
        }

        file = null;
        return false;
    }

    IAcssConfiguration IAcssBuilder.Configuration { get; } = new AcssConfiguration();

    IAcssLoader IAcssBuilder.BuildLoader()
    {
        lock (this)
        {
            _loader ??= new AcssLoader(this);    
        }
        
        return _loader;
    }

    public bool TryBuildRiderSettingsForAcss(out string? output, out string? setting, Action<Exception>? exceptionHandler = null)
    {
        try
        {
            var typeResolver = ((IAcssBuilder)this).TypeResolver;
            var types = typeResolver
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
        
            File.WriteAllText(output, setting);
            
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

    #endregion
}
