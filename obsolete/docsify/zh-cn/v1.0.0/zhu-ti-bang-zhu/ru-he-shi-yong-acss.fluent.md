# 如何使用 Acss.Fluent

## 安装依赖库

```bash
dotnet add package Nlnet.Avalonia.Css.Fluent --version 1.0.0-beta.4
```

## 使用主题

可以简单地使用 Acss.Fluent 主题。

```xml
<Application.Styles>
    <fluent:AcssFluentTheme />
</Application.Styles>
```

{% hint style="info" %}
AcssFluentTheme 使用了嵌入的 Avalonia 资源作为代码源，主题下所有的 Acss 代码将会从资源中提取，这意味着不可更改。
{% endhint %}

{% hint style="success" %}
使用嵌入的代码源的优点是，发布时不需要携带更多的本地代码文件，都单文件发布的发布方式更加友好。
{% endhint %}

## 使用默认的本地资源

AcssFluentTheme 支持优先使用本地资源。可以通过设置 `UseRecommendedPreferSource` 属性让 AcssFluentTheme 优先加载嵌入资源的 Uri 的 AbsolutePath 作为相对路径的本地文件，相对目录的锚定目录为当前程序目录。

{% hint style="warning" %}
需要注意的是，当前版本并未对外提供控制嵌入代码源导出的 API，因此使用本地资源的同时，需要设置 AutoExportSourceToLocal 属性为 true，让 AcssFluentTheme 尝试将所有嵌入的代码资源导出到本地。
{% endhint %}

```xml
<Application.Styles>
    <fluent:AcssFluentTheme AutoExportSourceToLocal="True" 
                            UseRecommendedPreferSource="True" />
</Application.Styles>
```

## 使用自定义本地资源

如果不希望使用默认的本地资源路径，可以通过设置 `PreferLocalPath` 的方式来自定义本地资源路径。这种方式设置的代码源将会有更高的优先级。

举例来说，如果我们希望调试环境和发布环境使用不同的本地目录，例如，调试环境下希望使用源代码目录，发布环境下希望使用当前程序目录，则可以这样使用：

```xml
<Application.Styles>
    <fluent:AcssFluentTheme AutoExportSourceToLocal="True"
                            UseRecommendedPreferSource="True"
                            PreferLocalPath="{x:Static a:Cdt.PreferLocalPath}" />
</Application.Styles>
```

```csharp
public static class Cdt
{
    public static string? PreferLocalPath { get; set; }

    static DebugThing()
    {
#if DEBUG
        PreferLocalPath = "../../src/Nlnet.Avalonia.Css.Fluent/";
#else
        PreferLocalPath = null;
#endif
    }
}
```

以上代码中，DEBUG 模式下，PreferLocalPath 使用了 "../../src/Nlnet.Avalonia.Css.Fluent/" 目录，在程序运行时 Acss 会尝试将嵌入的代码源导出到这个目录下，资源目录结构不变，且优先从这个目录加载。

而非 DEBUG 模式下，PreferLocalPath 则为 null，UseRecommendedPreferSource 生效，Acss 会使用当前目录作为导出和加载目录。

{% hint style="success" %}
优先使用本地文件的优点是，可以修改本地文件来修改主题样式。
{% endhint %}
