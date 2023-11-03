# 配置参数

Acss 支持一些配置，放在了配置服务（IAcssConfiguration）当中。可以通过 AcssContext 拿到配置进行读写。

```csharp
var cfg = AcssContext.Default.GetService<IAcssConfiguration>();

cfg.Accent = "green";
cfg.EnableTransitions = true;
```

{% hint style="info" %}
Acss 配置目前仅支持设置主题颜色（Accent），是否启用渐变（EnableTransitions）和主题（Theme）。前两个在 IAcssConfiguration 中有定义。最后的主题（Theme）则受 Avalonia 的 ThemeVariant 影响。

其他功能配置待续，欢迎在[这里](https://github.com/liwuqingxin/Avalonia.Css/issues)提供新的场景和需求。
{% endhint %}
