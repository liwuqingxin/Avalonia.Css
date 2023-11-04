# 过程和概念

Acss 的原理比较简单。其运行过程如下图所示。

<figure><img src="../../.gitbook/assets/Process.png" alt=""><figcaption></figcaption></figure>

## AcssContext

使用 Acss 时，需要构建一个 Acss 的上下文环境，我们称之为 AcssContext。这个上下文环境负责提供 Acss 运行过程中所需要的所有服务，包括语法分析服务、语法解释服务、类型解析服务、资源等各类工厂、Acss 加载服务、文件监控服务、Acss 配置服务、Rider 配置服务等。这些服务当中，大部分在 Acss 类库当中内置了。部分服务需要用户参与组建，例如类型解析服务。

同时，Acss 还在运行时提供了所有动态资源的访问功能，包括 Acss 文件、Acss Tokens 等。

## TypeResolver

TypeResolver 是一个类型解析服务，部署在 AcssContext 中。用户在构建 AcssContext 时，需要将自己的想要使用 Acss 的类型注册到 TypeResolver 当中。没有被注册的类型出现在 Acss 代码中时，会无法被解析而跳过，并且会输出调试信息，关于调试请查看[这里](tiao-shi.md)。

## Acss Source

Acss Source 是 Acss 代码的定义，在 Acss 中表现为一个名为 ISource 的接口。我们目前内置了两种代码源，本地文件（File Source）和内嵌资源（EmbeddedSource）。具体请参考[代码源](../zhu-ti-bang-zhu/ru-he-shi-yong-acss/dai-ma-yuan.md)。

## AcssTokens

当你使用 Acss 加载器加载一个代码源时，代码源首先被读取解析到内存中，形成原始的模型结构 AcssTokens。

{% hint style="success" %}
注意，Acss 是支持继承和重用的，AcssTokens 也是可重用的。针对一个特定的源，AcssTokens 只会被加载一次。
{% endhint %}

## Avalonia Object

Acss 加载器加载过程中，AcssTokens 创建后，会继续被解析为具体的 Avalonia 对象，例如 Style、Resource、Animation、其他 Object 等。这些对象会按照一定的规则被加载到 Avalonia 的 UI 当中，具体规则参考[加载范围](../zhu-ti-bang-zhu/ru-he-shi-yong-acss/jia-zai-fan-wei.md)。
