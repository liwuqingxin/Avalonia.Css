# 如何使用 MessageBox

## 安装依赖库

```bash
dotnet add package Nlnet.Avalonia.MessageBox --version 1.0.0-beta.4
```

## 使用独立样式的 MessageBox

* 引用资源。

```xml
<ResourceInclude Source="avares://Nlnet.Avalonia.MessageBox/Assets/Themes.axaml" />
```

* 同步使用。

```csharp
// WPF Standard: call messagebox synchronous.
private void OnClick(object? sender, RoutedEventArgs e)
{
    var result = MessageBox.Show("Hello, this is Nlnet MessageBox!", 
        "Welcome", Buttons.OkCancel, Images.Info);
    
    TbxResult.Text = result.ToString();
}
```

* 异步使用。

```csharp
// Avalonia Standard: call messagebox asynchronous.
private async void OnClick(object? sender, RoutedEventArgs e)
{
    var result  = await MessageBox.ShowAsync("Hello, this is Nlnet MessageBox :)", 
         "Welcome", Buttons.OkCancel, Images.Info);
    
    TbxResult.Text = result.ToString();
}
```

## 使用基于 Acss 的 MessageBox

和独立使用的 MessageBox 样式不同之处在于，基于 Acss 的样式加载的样式资源不一样。调用方式则没有区别。

```xml
<ResourceInclude 
    Source="avares://Nlnet.Avalonia.MessageBox/Assets/Themes.Acss.axaml" />
```

## 使用自定义样式

你仍然可以自定义 MessageBox 的主题和样式，方法和重写和其他 Avalonia 控件/窗口的主题样式一样。

## 示例代码

{% hint style="success" %}
我们提供了独立的 MessageBox 示例代码，请在 [Github](https://github.com/liwuqingxin/Avalonia.Css/tree/main/samples/Nlnet.Avalonia.MessageBox.Samples) 上访问它。
{% endhint %}
