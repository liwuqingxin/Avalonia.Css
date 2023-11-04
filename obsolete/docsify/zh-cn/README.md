# Format Page for Docsify

:cry: 这里是 docsify 的格式测试页面。不属于文档的一部分。如果你浏览到此，那就是孽缘。

# header 1

## header 2

### header 3

#### header 4

##### Header 5 that not display in sidebar. <!-- {docsify-ignore} -->

This is body.

> This is quote.

>> This is quote of second level.

# Code

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

# Emoji
- do not use emoji.

&#58;100:
- use emoji.

:100:

:smile:

# Docsify attension
!> 1. first</br>2. second line</br>3. third line</br>

# Docsify Plugin: Flexible Alerts

Docs see [here](https://github.com/fzankl/docsify-plugin-flexible-alerts).

> [!NOTE]
> An alert of type 'note' using global style 'callout'.

> [!TIP]
> An alert of type 'tip' using global style 'callout'.

> [!WARNING]
> An alert of type 'warning' using global style 'callout'.

> [!ATTENTION]
> An alert of type 'attention' using global style 'callout'.


# Tabs

Docs see [here](https://jhildenbiddle.github.io/docsify-tabs/#/).

<!-- tabs:start -->

#### **English**

Hello!

#### **French**

Bonjour!

#### **Italian**

Ciao!

<!-- tabs:end -->



<!-- tabs:start -->

<!-- tab:English -->

Hello!

<!-- tab:French -->

Bonjour!

<!-- tab:Italian -->

Ciao!

<!-- tabs:end -->

<!-- tabs:start -->

#### **Bold**

...

#### **<em>Italic</em>**

...

#### **<span style="color: red;">Red</span>**

...

#### **:smile:**

...

#### **😀**

...

#### **Badge <span class="tab-badge">New!</span>**

...

<!-- tabs:end -->


