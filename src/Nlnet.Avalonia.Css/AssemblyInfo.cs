using Avalonia.Metadata;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Nlnet.Avalonia.Css.Test")]

// 不需要修改
[assembly: XmlnsPrefix("https://www.nlnet.com/avalonia", "nlnet")]

// 添加新的命名空间在此添加对应代码行
[assembly: XmlnsDefinition("https://www.nlnet.com/avalonia", "Nlnet.Avalonia.Css")]
