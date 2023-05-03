using Avalonia.Metadata;

// 不需要修改
[assembly: XmlnsPrefix("https://www.nlnet.com/avalonia.css", "nlnet")]

// 添加新的命名空间在此添加对应代码行
[assembly: XmlnsDefinition("https://www.nlnet.com/avalonia.css", "Nlnet.Avalonia.Css.Fluent")]
