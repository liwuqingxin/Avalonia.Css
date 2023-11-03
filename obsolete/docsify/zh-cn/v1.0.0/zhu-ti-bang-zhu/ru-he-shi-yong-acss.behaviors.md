# 如何使用 Acss.Behaviors

作为示例，我们在当前版本提供了两个内置行为。具体使用方式请参考[行为](../acss-yu-fa/hang-wei.md)。

* **combobox.popup.align。**这个行为可以将下拉框的下拉列表的选中项对齐到下拉框。
* **window.esc.close。**这个行为可以让窗口支持相应 `Esc` 键来关闭窗口。

{% hint style="success" %}
更多的内置行为正在计划当中。如果对此你有任何需求，欢迎在[这里](https://github.com/liwuqingxin/Avalonia.Css/issues/new)告诉我们。
{% endhint %}

## 安装依赖库

```bash
dotnet add package Nlnet.Avalonia.Css.Behaviors --version 1.0.0-beta.4
```

## 自定义行为

* 引用包 Nlnet.Avalonia.Css.CompileGenerator。

```bash
dotnet add package Nlnet.Avalonia.Css.CompileGenerator --version 1.0.0-beta.4
```

* 创建行为声明类，继承 AvaloniaObject 类和 IBehaviorDeclarer 接口。

```csharp
public partial class CustomA : AvaloniaObject, IBehaviorDeclarer
{
        
}
```

* 提供使用行为声明类 CustomA 的扩展类。

```csharp
/// <summary>
/// Use customA behavior feature for default css context.
/// </summary>
/// <param name="builder"></param>
/// <returns></returns>
public static AppBuilder UseCustomABehaviorForDefaultContext(this AppBuilder builder)
{
    AcssBuilder.Default.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<CustomA>());
    AcssBuilder.Default.BehaviorDeclarerManager.RegisterDeclarer<CustomA>(nameof(CustomA).ToLower());
    AcssBuilder.Default.BehaviorDeclarerManager.RegisterDeclarer<CustomA>(nameof(CustomA));
    return builder;
}
```

* 创建自定义行为类，该行为类挂在在行为声明类 CustomA 下。

```csharp
[Behavior("window.esc.close", typeof(CustomA))]
public class WindowEscCloseBehavior: AcssBehavior<WindowEscCloseBehavior>
{
    protected override void OnAttached(AvaloniaObject target)
    {
        if (target is not Window window)
        {
            return;
        }

        window.KeyDown -= WindowOnKeyDown;
        window.KeyDown += WindowOnKeyDown;
    }
    
    protected override void OnDetached(AvaloniaObject target)
    {
        if (target is not Window window)
        {
            return;
        }

        window.KeyDown -= WindowOnKeyDown;
    }
    
    private static void WindowOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not Window window)
        {
            return;
        }
        if (e.Key == Key.Escape)
        {
            window.Close();
        }
    }
}
```

* 在程序中使用 CustomA 行为声明类。

```csharp
private static AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        .UsePlatformDetect()

        // Use avalonia css stuff.
        .UseAcssDefaultBuilder()

        // [Optional] Use acss behavior.
        .UseAcssBehaviorForDefaultBuilder();
    	
    	// [Optional] Use customA behavior.
    	.UseCustomABehaviorForDefaultBulder();
}
```

* 在 Acss 代码中使用行为类。

```css
ComboBoxPage ComboBox#C1{
    .acss:combobox.popup.align;
}

NtWindow#RootWindow{
    .acss:window.esc.close;
}
```

