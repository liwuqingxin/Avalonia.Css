---
description: Acss 编辑器
---

# 配置开发环境

Acss 属于解释性语言，支持使用任何文本编辑器对其进行编辑。最简单地，你可以在记事本中编辑它。不过我们推荐使用 IDE 来获得更好的编写体验。

## 使用 Rider

{% hint style="warning" %}
由于开发工作量比较大，目前并未提供 Rider 的 Acss 语言支持插件，后续能够支持的时间目前也不能确定。如果你对此感兴趣并且愿意加入我们的开发工作，非常欢迎通过邮箱 yangqi1990917@163.com 联系我们。
{% endhint %}

目前，对于 Rider，我们提供了简陋的使用方式。我们需要在 IAcssContext 构建完成后，执行以下代码来生成当前环境的 Rider 配置文件。我们会尝试将它自动放置到 Rider 的安装目录下。如果执行失败，请通过参数检查异常情况，并自行处理配置文件的创建。

{% code lineNumbers="true" %}
```csharp
var riderBuilder = AcssContext.Default.GetService<IRiderSettingsBuilder>();
riderBuilder.TryBuildRiderSettingsForAcss(out _, out _, null);
```
{% endcode %}

{% hint style="info" %}
Rider 配置文件的目录一般为：

C:/Users/**{user}**/AppData/Roaming/JetBrains/**Rider{version}**/filetypes/**Acss.xml**
{% endhint %}

{% hint style="info" %}
Rider 配置文件依赖于 AcssContext，不同的上下文环境会产生不同的配置，主要体现在注册了不同的类型到类型解析服务，会产生不同的类型代码高亮和提示。
{% endhint %}

{% hint style="warning" %}
Rider 的配置文件不会热加载，该配置更新后，需要重新启动 Rider 才能生效。重启后，代码高亮则表示文件生效。
{% endhint %}

## 使用 Visual Studio Code

我们对 Visual Studio Code 提供了简陋的 Acss 的语言插件。可以访问[这里](https://marketplace.visualstudio.com/items?itemName=nlnet.avalonia-css-extension)下载安装，或者在 VSCode 应用当中搜索 ‘avalonia-css-extension’ 进行安装。

和 Rider 一样，该插件目前暂时没有更新计划，效果一般。😥😥😥

{% hint style="warning" %}
Rider 和 Visual Studio Code 保存文件时会触发两次文件保存动作，目前版本我们并未做过滤处理。
{% endhint %}
