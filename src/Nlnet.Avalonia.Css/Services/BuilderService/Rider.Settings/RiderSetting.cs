// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable CS8618

namespace Nlnet.Avalonia.Css;

[XmlRoot("filetype")]
public class RiderSetting
{
    [XmlAttribute]
    public bool binary { get; set; } = false;
    [XmlAttribute]
    public string default_extension { get; set; } = "acss";
    [XmlAttribute]
    public string description { get; set; } = "Avalonia Cascading Style Sheets.";
    [XmlAttribute]
    public string name { get; set; } = "Acss";

    public highlightingNode highlighting { get; set; } = new highlightingNode();

    public extensionMapNode extensionMap { get; set; } = new extensionMapNode();
}

public class highlightingNode
{
    [XmlArray("options")]
    public optionsNode optionsNode { get; set; } = new optionsNode();

    public keywordNode keywords { get; set; } = new keywordNode();

    public keywordNode keywords2 { get; set; } = new keywordNode();

    public keywordNode keywords3 { get; set; } = new keywordNode();

    public keywordNode keywords4 { get; set; } = new keywordNode();

    public highlightingNode()
    {
        keywords.keywords = new StringBuilder().AppendJoin(';', key1).ToString();
        keywords2.keywords = new StringBuilder().AppendJoin(';', key2).ToString();
    }

    private readonly string[] key1 = new[]
    {
        "var",
        "import ",
        "base ",
        "rely ",
        
        "accent",
        "theme",
        "desc",
        
        "color",
        "double",
        "int",
        "brush",
        "linear",
        "transition",
        
        "::animation",
        "::res",
        
        ".acss",
        
        "/template/",
        
        ":not",
        ":is-part",
        ":pointerover",
        ":pressed",
        ":selected",
        ":checked",
        ":focus",
        ":focus-within",
        ":disabled",
        ":blackout",
        ":expanded",
        ":dropdownopen",
        ":sortdescending",
        ":dragIndicator",
        ":horizontal",
        ":vertical",
        ":inactive",
        ":indeterminate",
        ":open",
        ":left",
        ":up",
        ":right",
        ":down",
        ":today",
        ":hasnodate",
        ":hasnotime",
        ":overlay",
        ":compactoverlay",
        ":compactinline",
        ":inline",
        ":lightdismiss",
        ":closed",
        ":information",
        ":success",
        ":warning",
        ":error",
    };

    private readonly string[] key2 = new[]
    {
        "#",
        "'",
        "(",
        ")",
        "[",
        "]",
        "{",
        "}",
        "-",
        ".",
        "/",
        ":",
        //";",
        "=",
        ">",
        "^",
        "|",
    };
}

public class optionsNode : List<optionNode>
{
    public optionsNode()
    {
        this.Add(new optionNode("LINE_COMMENT", "//"));
        this.Add(new optionNode("COMMENT_START", "/*"));
        this.Add(new optionNode("COMMENT_END", "*/"));
        this.Add(new optionNode("HEX_PREFIX", ""));
        this.Add(new optionNode("NUM_POSTFIXES", ""));
        this.Add(new optionNode("HAS_BRACES", "true"));
        this.Add(new optionNode("HAS_BRACKETS", "true"));
        this.Add(new optionNode("HAS_PARENS", "true"));
    }
}

[XmlType("option")]
public class optionNode
{
    [XmlAttribute]
    public string name { get; set; }
    [XmlAttribute]
    public string value { get; set; }

    public optionNode(string n, string v)
    {
        name = n;
        value = v;
    }

    public optionNode()
    {
        
    }
}

public class keywordNode
{
    [XmlAttribute]
    public string keywords { get; set; }
}

public class extensionMapNode
{
    public mappingNode mapping { get; set; } = new mappingNode();
}

public class mappingNode
{
    [XmlAttribute]
    public string ext { get; set; } = "acss";
}