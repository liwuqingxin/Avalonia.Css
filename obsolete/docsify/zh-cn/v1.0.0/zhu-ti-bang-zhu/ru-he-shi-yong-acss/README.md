# 如何使用 Acss

{% hint style="info" %}
**特别注意**

如果没有自定义扩展的需求，按照本页的示例即可使用 Acss。

本页面提供最基本的 Acss 使用示例。其他复杂的使用或者自定义场景请具体参考本小节内部其他章节。
{% endhint %}

## 安装依赖库

```bash
dotnet add package Nlnet.Avalonia.Css --version 1.0.0-beta.4
```

## 注册 Acss 和类型解析

这个过程也可以延后到初始化一起进行。

{% hint style="danger" %}
当前版本不支持使用多个 AcssContext，仅支持默认的 AcssContext.Default。
{% endhint %}

```csharp
private static AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        ...
        
        // Use avalonia css stuff.
        .UseAcssDefaultContext()
        
        // Type resolver for 'Your.Lib'. The GenericTypeResolver<TSink> will load all
        // types those belong to the assembly who contains the T class.
        .WithTypeResolverForAcssDefaultContext(new GenericTypeResolver<TSink>())
        ;
}
```

## 初始化 AcssContext

你可以在 Application.Initialize() 函数当中选择性地做这些事情。当然其他时机也不受影响。

* 使用 Acss。
* 配置参数。
* 注册类型到类型解释服务中。
* 创建 Rider 配置。如果你使用了 Rider 作为开发工具的话。
* 加载 Acss 源。

到此，正常来说，Acss 已经生效。

```csharp
private class void Initialize()
{
    ...
    
    // [Optional] Use default css builder. It has same effect to 
    // AcssExtension.UseAvaloniaCssDefaultBuilder().
    AcssContext.UseDefaultContext();
	
    // [Optional] Set the current accent and other settings.
    var cfg = AcssContext.Default.GetService<IAcssConfiguration>();
    cfg.Accent = "blue";
    cfg.EnableTransitions = true;
    
    // [Optional] Create rider settings for this Acss builder.
    var riderBuilder = AcssContext.Default.GetService<IRiderSettingsBuilder>();
    riderBuilder.TryBuildRiderSettingsForAcss(out _, out _, null);
    
    // Load acss files to Application.Current.Styles. 
    // You can keep the acssFile for more operations.
    var loader = AcssContext.Default.GetService<IAcssLoader>();
    var acssFile = loader.Load(Application.Current.Styles, new FileSource("Acss/Case.acss"));
    
    // Or load acss files from a folder.
    loader.LoadCollection(Application.Current.Styles, new FileSourceCollection("Acss/"));
    
    ...
}
```
