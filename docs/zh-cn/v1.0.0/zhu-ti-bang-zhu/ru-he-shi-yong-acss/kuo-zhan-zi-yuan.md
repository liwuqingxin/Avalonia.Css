# 扩展资源

Acss 的资源（AcssResource）支持扩展。目前我们内置了一些常用的资源类型，你也可以定义自己的资源类型。

## 内置的资源类型

1. Brush
2. Color
3. Double
4. Int
5. LinearBrush
6. Thickness
7. Transition
8. BoxShadows

{% hint style="success" %}
在后面的计划中会新增更多的内置资源类型。
{% endhint %}

## 自定义资源类型

Acss 的资源都是继承自抽象类 `AcssResourceBaseAndFac<T>`，定义如下：

```csharp
public abstract class AcssResourceBaseAndFac<T> : AcssResource, IResourceFactory 
    where T : AcssResource, new()
```

你可以继承这个类来扩展自己的资源类型，例如以下代码中，我们扩展定义了 Thickness 的资源类型。该类型的别名有 “Thickness”，“Thick”，“StrokeThickness”，“Margin”，“Padding”。在 Acss 代码中上述的任何一个别名都会被解析为 Thickness 的资源。

{% hint style="warning" %}
注意，别名对大小写<mark style="color:red;">**不**</mark>**敏感**。`thick` 和 `Thick` 指代相同的资源类型 `Thickness`。
{% endhint %}

```csharp
[ResourceType(nameof(Thickness))]
[ResourceType("Thick")]
[ResourceType("StrokeThickness")]
[ResourceType("Margin")]
[ResourceType("Padding")]
internal class ThicknessResource : AcssResourceBaseAndFac<ThicknessResource>
{
    protected override object? BuildValue(IAcssContext context, string valueString)
    {
        return valueString.TryParseThickness();
    }
}
```
