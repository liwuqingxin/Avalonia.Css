using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Nlnet.Avalonia.SampleAssistant
{
    public static class XmlHelper
    {
        public static XmlWriter GetXmlWriter(StringBuilder builder)
        {
            var writer = XmlWriter.Create(builder, new XmlWriterSettings()
            {
                DoNotEscapeUriAttributes = true,
                NewLineOnAttributes      = true,
                OmitXmlDeclaration       = true,
                Indent                   = true,
                IndentChars              = "    ",
                NamespaceHandling        = NamespaceHandling.OmitDuplicates,
                NewLineHandling          = NewLineHandling.Replace,

                // Token StartElement in state EndRootElement would result in an invalid XML document.
                // Make sure that the ConformanceLevel setting is set to ConformanceLevel.Fragment or
                // ConformanceLevel.Auto if you want to write an XML fragment.
                ConformanceLevel = ConformanceLevel.Auto,
            });

            return writer;
        }
    }

    public interface ICaseXamlParser
    {
        Dictionary<string, string> ParseCases(string? xaml);
    }

    internal class DefaultCaseXamlParser : ICaseXamlParser
    {
        public Dictionary<string, string> ParseCases(string? xaml)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(xaml))
            {
                return dic;
            }

            var doc = new XmlDocument();
            doc.LoadXml(xaml);

            var childNodes = doc.ChildNodes;
            foreach (var childNode in childNodes)
            {
                if (childNode is XmlNode xmlNode)
                {
                    ParseNode(xmlNode, dic);
                }
            }

            return dic;
        }

        private static void ParseNode(XmlNode? node, IDictionary<string, string> results)
        {
            if (node == null)
            {
                return;
            }

            if (Regex.IsMatch(node.Name, ".*:Case$"))
            {
                var builder = new StringBuilder();
                using var writer = XmlHelper.GetXmlWriter(builder);
                foreach (var childNode in node.ChildNodes)
                {
                    if (childNode is XmlNode xmlNode)
                    {
                        xmlNode.WriteTo(writer);
                    }
                }

                writer.Flush();
                var xaml = builder.ToString();

                results.Add(node.Attributes?[nameof(Case.Header)]?.Value ?? Guid.NewGuid().ToString(), xaml);
            }

            foreach (var childNode in node.ChildNodes)
            {
                if (childNode is XmlNode xmlNode)
                {
                    ParseNode(xmlNode, results);
                }
            }
        }
    }

    public class XCaseXamlParser<TCase> : ICaseXamlParser where TCase : Case
    {
        public Dictionary<string, string> ParseCases(string? xaml)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(xaml))
            {
                return dic;
            }

            var xDocument  = LoadWithoutNamespace(xaml);
            var childNodes = xDocument.Elements().DescendantsAndSelf();
            var cases      = childNodes.Where(e => e.Name.LocalName == typeof(TCase).Name);
            foreach (var childNode in cases)
            {
                var header = childNode.Attribute(nameof(Case.Header))?.Value;
                if (header == null)
                {
                    continue;
                }

                var builder = new StringBuilder();
                using var writer = XmlHelper.GetXmlWriter(builder);
                childNode.WriteTo(writer);
                writer.Flush();
                dic.Add(header, builder.ToString());
            }

            return dic;
        }

        private static XDocument LoadWithoutNamespace(string xaml)
        {
            var xDocument = XDocument.Load(new StringReader(xaml));
            
            //return xDocument;

            foreach (var xe in xDocument.Elements().DescendantsAndSelf())
            {
                // Stripping the namespace by setting the name of the element to it's local name only
                xe.Name = xe.Name.LocalName;
                // replacing all attributes with attributes that are not namespaces and their names are set to only the local name
                xe.ReplaceAttributes(
                    xe.Attributes()
                        .Where(xa => !xa.IsNamespaceDeclaration)
                        .Select(xa => new XAttribute(xa.Name.LocalName, xa.Value))
                );
            }

            return xDocument;
        }
    }
}
