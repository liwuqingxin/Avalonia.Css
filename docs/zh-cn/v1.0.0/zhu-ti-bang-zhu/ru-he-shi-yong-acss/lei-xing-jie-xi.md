# 类型解析

类型解析的服务称为类型解析管理器（ITypeResolverManager）。它管理着多个 Acss 内置的和用户注册的类型解析器（ITypeResolver）。

Acss 除了内置的类型解析外，你需要自行注册所有用到的类型到 AcssContext 当中。未注册的类型将会无法识别而被略过。我们提供了多种类型解析的注册方式。

{% hint style="info" %}
Acss 内置了 Avalonia.Controls，Avalonia.Base 两个程序集内所有 AvaloniaObject 子类类型的解析器，以及部分必要的类型的解析器。
{% endhint %}

{% hint style="danger" %}
**特别注意**

目前我们没有做命名空间的映射，因此类型重名的几率比较大。如果有这种情况，请自行管理重名类型的别名，避免冲突。
{% endhint %}

## 程序集类型解析器

我们提供了非常方便的程序集类型解析器 `GenericTypeResolver<TTypeSink>`。它可以通过锚定的类型，将其所在程序集中所有 AvaloniaObject 类型的子类都加入到解析器当中。例如：

```csharp
// Add all types, which are derived from AvaloniaObject, in the Avalonia.Controls.dll
// to the type resolver.
var resolver = new GenericTypeResolver<Button>();
```

## 自定义类型解析器

用户可以继承默认实现类 `Resolver`，也可以自行实现类型解析器接口 `IResolver` 来定义一个类型解析器。例如：

```csharp
// #1 Use Resolver.
public class CustomTypeResolver1 : Resolver
{
    // Nothing to do.
}

// #2 Implement IResolver.
public class CustomTypeResolver2 : IResolver
{
    protected Dictionary<string, Type> Types;

    protected Resolver()
    {
        Types = new Dictionary<string, Type>();
    }

    public bool TryAddType(string name, Type type)
    {
        if (Types.ContainsKey(name))
        {
            return false;
        }

        Types.Add(name, type);

        return true;
    }

    public bool TryAddType<T>(string name)
    {
        if (Types.ContainsKey(name))
        {
            return false;
        }

        Types.Add(name, typeof(T));

        return true;
    }

    public bool TryGetType(string name, out Type? type)
    {
        return Types.TryGetValue(name, out type);
    }

    public IEnumerable<Type> GetAllTypes()
    {
        return Types.Values;
    }
}
```

## 添加类型到解析器

有了类型解析器后，可以将类型添加到该解析器当中。

```csharp
IResolver resolver = new CustomTypeResolver1();

// Map 'Button' to Button.
resolver.TryAddType("Button", typeof(Button));

// Map 'TextBlock' to TextBlock.
resolver.TryAddType<TextBlock>("TextBlock");

// map 'text' to TextBlock.
resolver.TryAddType<TextBlock>("text");
```

{% hint style="info" %}
**注意**

* 一个类型可以注册多个别名，可以简化使用。例如上述代码中 'text' 和 'TextBlock' 都可以指代 TextBlock。
* Acss 的类型解析对**大小写敏感**。
{% endhint %}

## 单个类型

目前版本我们暂时不支持简单的单个类型的注册，必须借助 `ITypeResolver`。后续考虑提供更加简便的类型注册 API。

## 注册类型解析器到管理服务

如下代码所示。

```csharp
var typeResolverManager = AcssContext.Default.GetService<ITypeResolverManager>();

IResolver resolver = new CustomTypeResolver1();

typeResolverManager.LoadResolver(resolver);
typeResolverManager.LoadResolver(new GenericTypeResolver<App>());
```

## 在构建 Avalonia 服务时注册类型

如代码所示，添加默认的 AcssContext 后，可以直接针对程序集添加类型解析器。

```csharp
private static AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        .UsePlatformDetect()

        // Use avalonia css stuff.
        .UseAcssDefaultContext()
        
        // Type resolver for Nlnet.Avalonia.Svg
        .WithTypeResolverForAcssDefaultBuilder(new GenericTypeResolver<Icon>())
        
        // Type resolver for Nlnet.Avalonia.SampleAssistant
        .WithTypeResolverForAcssDefaultBuilder(new GenericTypeResolver<Case>());
}
```
