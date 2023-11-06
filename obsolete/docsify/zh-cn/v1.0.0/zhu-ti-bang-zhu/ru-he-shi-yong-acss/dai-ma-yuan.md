# 代码源

代码源（ISource）是 Acss 源代码的来源。目前我们提供了两种类型的代码源，本地代码源（FileSource）和嵌入代码源（EmbeddedSource）。

{% hint style="info" %}
Acss 的代码文件以小写的 acss 作为后缀名，与代码源的类型无关。
{% endhint %}

## 本地代码源

本地代码源使用本地文件作为代码输入。我们支持两种加载方式，按文件加载或者按目录加载。

### 1. 按文件加载

```csharp
var loader = AcssContext.Default.GetService<IAcssLoader>();

var source1 = new FileSource("Acss/Case.acss");
loader.Load(Application.Current.Styles, source1);

// Use prefer path $"{debugRelative}Acss/Case.acss" if it is valid. 
// Or use the path "Acss/Case.acss".
var source2 = new FileSource("Acss/Case.acss", $"{debugRelative}Acss/Case.acss");
loader.Load(Application.Current.Styles, source2);
```

### 2. 按目录加载

```csharp
var loader = AcssContext.Default.GetService<IAcssLoader>();

loader.LoadCollection(this, new FileSourceCollection("Acss/"));
loader.LoadCollection(this, new FileSourceCollection("Acss/", "../../Acss/"));
```

{% hint style="success" %}
无论按文件加载还是按目录加载，都支持优先路径。优先路径有效时，会被优先使用。这在调试环境下十分有用。

我们可以优先使用源代码所在目录，而把当前程序下的目录作为第二选择。
{% endhint %}

{% hint style="info" %}
注意，按照目录加载的方式，不会递归目录，只会加载指定目录当前目录下的所有 \*.acss 文件。
{% endhint %}

## 嵌入代码源

我们也支持将代码源以 Avalonia 资源的形式嵌入到程序当中。EmbeddedSource 的三个参数用法请参考[如何使用 Acss.Fluent](../ru-he-shi-yong-acss.fluent.md)。

{% hint style="success" %}
嵌入资源的方式对单个文件打包的方式更加友好。
{% endhint %}

### 1. 单个 Uri 加载

```csharp
var loader = AcssContext.Default.GetService<IAcssLoader>();

loader.Load(
    Application.Current.Styles, 
    new EmbeddedSource(
        new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/AccentColor.acss"), 
        PreferLocalPath, 
        UseRecommendedPreferSource, 
        AutoExportSourceToLocal));
```

### 2. 按 Uri 批量加载

```csharp
var loader = AcssContext.Default.GetService<IAcssLoader>();

loader.LoadCollection(
    Application.Current.Styles, 
    new EmbeddedSourceCollection(
        new Uri("avares://Nlnet.Avalonia.Css.Fluent/Acss/"), 
        PreferLocalPath, 
        UseRecommendedPreferSource, 
        AutoExportSourceToLocal));
```
