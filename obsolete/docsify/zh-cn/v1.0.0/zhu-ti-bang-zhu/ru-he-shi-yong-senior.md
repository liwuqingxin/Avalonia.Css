# 如何使用 Senior

## 安装依赖库

```bash
dotnet add package Nlnet.Avalonia.Senior --version 1.0.0-beta.4
```

## 使用独立样式

```xml
<ResourceInclude Source="avares://Nlnet.Avalonia.Senior/Assets/Themes.axaml" />
```

## 使用基于 Acss 的样式

```xml
<ResourceInclude Source="avares://Nlnet.Avalonia.Senior/Assets/Themes.Acss.axaml" />
```

## 使用自定义样式

你仍然可以自定义内部任何控件的主题和样式，方法和重写和其他 Avalonia 控件/窗口的主题样式一样。
