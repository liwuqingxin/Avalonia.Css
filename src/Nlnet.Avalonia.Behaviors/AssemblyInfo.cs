﻿using Avalonia.Metadata;
using System.Runtime.CompilerServices;

// 不需要修改
[assembly: XmlnsPrefix("https://www.nlnet.com/avalonia.behaviors", "nlnet")]

// 添加新的命名空间在此添加对应代码行
[assembly: XmlnsDefinition("https://www.nlnet.com/avalonia.behaviors", "Nlnet.Avalonia.Css.Fluent")]
