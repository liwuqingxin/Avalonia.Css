# 如何使用 Acss.Controls

## 安装依赖库

```bash
dotnet add package Nlnet.Avalonia.Css.Controls --version 1.0.0-beta.4
```

## 引用资源和样式

```xml
<Styles.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceInclude Source="avares://Nlnet.Avalonia.Css.Controls/Assets/Themes.Acss.axaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Styles.Resources>

<StyleInclude Source="avares://Nlnet.Avalonia.Css.Controls/Assets/Styles.Acss.axaml" />
```

## 可选扩展

* TemplatedControlExtension 使用后，会对所有 ControlTemplate 内部的控件加上 `:is-part` 伪类。
* SelectionDetailExtension 使用后，会对所有 SelectingItemsControl 对象加上切换选中项的具体状态伪类，包括&#x20;
  * `:sel-detail-leave-smaller`
  * `:sel-detail-leave-lager`
  * `:sel-detail-enter-lager`
  * `:sel-detail-enter-lager`

```csharp
TemplatedControlExtension.Use();
SelectionDetailExtension.Use();
```

## 使用 Acss 定义自己的样式库

Nlnet.Avalonia.Css.Controls 针对 Avalonia 全部的原生控件定义了一套纯净的控件模板，可以十分友好地支持使用 Acss 扩展样式库。

{% hint style="info" %}
后续计划提供调试工具来支持控件库结构的速览。目前只能通过查看源代码来了解内部结构和一些 classes、name 等。
{% endhint %}

{% hint style="danger" %}
**最佳实践**

请注意，当你使用 Acss 代码时，请尽量将其放在你特有的名称的路径下，避免和其他库的 Acss 文件共存时，被放入同一目录，导致文件冲突或者错误加载。



例如，Nlnet.Avalonia.Css.Fluent 库的 Acss 代码文件的基路径为：

> "/Acss/Nlnet.Avalonia.Css.Fluent/"
{% endhint %}
