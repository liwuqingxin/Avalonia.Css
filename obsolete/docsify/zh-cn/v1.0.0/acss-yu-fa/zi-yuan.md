# 资源

## 资源集合语法

资源定义在资源集合里面。资源集合的语法如下：

<pre class="language-css"><code class="lang-css"><strong>/* Normal resources. */
</strong>::res {
    /* Resource here. */
}

/* Resources is available when accent is blue. */
::res[accent=blue] {
    /* Resource here. */
}

/* 
Resource is available when accent is blue. 
And description is "Resources description". 
*/
::res[accent=blue][desc=Resources description] {
    /* Resource here. */
}

/* 
Resource is available when theme is light and accent is blue. 
And description is "Resources description". 
*/
::res[theme=light][accent=blue][desc=Resources description] {
    /* Resource here. */
}
</code></pre>

{% hint style="info" %}
针对资源集合，目前我们仅支持“描述”（desc）、“主题颜色”（accent）和“主题”（theme）三个属性。其中 desc 是普通属性，accent 和 theme 是过滤属性，accent 过滤的有效参考值在 IAcssConfigration 当中，参考[配置参数](../zhu-ti-bang-zhu/ru-he-shi-yong-acss/pei-zhi-can-shu.md)，theme 过滤的有效参考值是 Avalonia 的 ThemeVariant。



**特别提示**

我们稍后计划针对 Acss 的[过滤控制](../cong-zhe-li-kai-shi/guan-yu-acss.md#4.-guo-lv-kong-zhi-kai-fa-zhong-...)的特性，提供更多的过滤属性。
{% endhint %}

## 资源语法

资源定义的语法形式如下：

```css
// resource-type(resource-key):value;                // Use value.
// resource-type(resource-key):var(resource-key);    // Use dynamic resource.

// Sample:
color(AccentColor): #0068B5;
brush(Accent): var(AccentColor);
```

## 自定义资源

自定义资源语法和内置资源语法一致，形式也是如[资源语法](zi-yuan.md#zi-yuan-yu-fa)所示。自定义资源参考[扩展资源](../zhu-ti-bang-zhu/ru-he-shi-yong-acss/kuo-zhan-zi-yuan.md)。

## Color

Acss 支持多种颜色表达方式，包括常用色名称、RGB、RGBA、HSL、HSV 等。

```css
::res {
    color(C01): red;                       /* r=f3, g=f4, b=f5 */
    color(C02): #f3f4f5;                   /* r=f3, g=f4, b=f5 */
    color(C03): #f6f3f4f5;                 /* r=f3, g=f4, b=f5, a=f6 */
    color(C04): #345;                      /* r=33, g=44, b=55 */
    color(C05): #f345;                     /* r=33, g=44, b=55, a=ff */
    
    /* Space is not allowed between the comma and value now. */
    color(C06): rgb(13,14,15);             /* r=13, g=14, b=15 */
    color(C07): rgba(13,14,15,16);         /* r=13, g=14, b=15, a=16 */
    color(C08): rgb(13%,14%,15%);          /* r=33, g=36, b=38 */
    color(C09): rgba(13%,14%,15%,16%);     /* r=33, g=36, b=38, a=41 */
    
    /* 色相: 60° (60.000), 饱和度: 70% (0.700), 亮度: 50% (0.500) */
    color(C10): hsl(60,70%,50%);
    
    /* 色相: 150° (150.000), 饱和度: 100% (1.000), 明度: 90% (0.900) */
    color(C11): hsv(150,100%,90%);
}
```

Acss 支持对所有值的表达形式额外定义透明度，这让我们可以更加灵活的定义颜色资源。例如：

```css
::res {
    color(C01): red 50%;              /* 红色带上 50% 的透明度 */
    color(C02): #f6f3f4f5 30%;        /* 最终透明度为 f6 * 30% */
    color(C03): hsl(60,70%,50%) 40%;  /* hsl 值解析成 Color 后，应用 40% 的透明度 */
}
```

{% hint style="danger" %}
颜色的额外透明度作为第二个参数，与颜色的值表达式用**空格**间隔。这也是目前值表达式内部不允许有空格存在的原因。
{% endhint %}

## Brush

对于 Color 支持的全部值表达形式，Brush 全部支持，包括常用色名称、RGB、RGBA、HSL、HSV等，也包括额外的透明度定义。

```css
::res {
    brush(B01): red;
    brush(B02): #f3f4f5;
    brush(B03): #f6f3f4f5;
    brush(B04): #345;
    brush(B05): #f345;
    brush(B06): rgb(13,14,15);
    brush(B07): rgba(13,14,15,16);
    brush(B08): rgb(13%,14%,15%);
    brush(B09): rgba(13%,14%,15%,16%);
    brush(B10): hsl(60,70%,50%);
    brush(B11): hsv(150,100%,90%);
    
    brush(B12): red 50%;
    brush(B13): #f6f3f4f5 30%;
    brush(B14): hsl(60,70%,50%) 40%;
}
```

同时，画刷支持对其 Color 属性应用动态资源。应用动态资源时仍然可以添加额外的透明度属性。

```css
::res {
    color(C01): red;
    brush(B01): var(C01);
    brush(B02): var(C01) 40%;
}
```

## Linear Brush

目前我们对 Linear Brush 的支持不够完善。初步使用方式如下代码所示。

{% hint style="danger" %}
Linear Brush 目前语法定义不完善，语法可能会发生破坏性更新。
{% endhint %}

```css
::res {
    linear(LB01): (0% 0% 0% 100%)[
        #ececec 0.8;
        #D2D3D4 1;
    ]
    linear(LB02): (0 0.5 0 1.4)[
        #ececec 0;
        red 0.8;
        #D2D3D4 1;
    ]
}

/*
- 上面的 LB01 资源定义的 LinearBrush 的 StartPoint 为 (0,0)，EndPoint 为 (0,1)。
  它有 2 个 GradientStop，分别是：
      Color = #ececec，Offset = 0.8
      Color = #D2D3D4，Offset = 1
      
- 上面的 LB02 资源定义的 LinearBrush 的 StartPoint 为 (0,0.5)，EndPoint 为 (0,1.4)。
  它有 3 个 GradientStop，分别是：
      Color = #ececec，Offset = 0
      Color = red，    Offset = 0.8
      Color = #D2D3D4，Offset = 1
*/
```

## Double & Int

```css
::res {
    int(max-length) : 120;
    double(button-height) : 30;
    double(scale-y) : 40%;
}
```

## Thickness

```css
::res {
    thick(button-border-thickness) : 2;
    thick(button-margin) : 2,2,2,2;
    margin(button-margin) : 2;
    padding(button-padding) : 2,2,2,2;
    thickness(button-padding) : 2,2,2,2;
}
```

## Transition

Acss 支持定义 Transition 资源，方便重用。这是 Avalonia 本身不具备的。Avalonia 支持定义 Transitions 资源，我们暂时没有进行支持。

{% hint style="danger" %}
Transition 目前语法定义不完善，语法可能会发生破坏性更新。
{% endhint %}

```css
::res {
    double(duration): 0.125;
    double(delay): 0.125;

    /* TargetType | Property | Duration | [Delay] | [Ease] */
    transition(trans01):
        Border.BorderThickness 0.2);
    transition(trans02):
        Border.BorderThickness 0.2 var(delay);
    transition(trans03):
        Border.BorderThickness var(duration) 0.4 QuadraticEaseOut);
    transition(trans04):
        Border.BorderThickness var(duration) 0.4 0,0,1,1);
}
```

{% hint style="warning" %}
Transition 资源的最后一个可选参数 \[Ease] 也是支持动态资源（var）的，但是目前我们没有支持定义缓动函数（Easing Function）的资源，因此实际上目前它是没办法使用动态资源的。
{% endhint %}

## BoxShadows

我们暂时对阴影资源的支持不完善，仅支持以下定义形式。

{% hint style="danger" %}
BoxShadows 目前语法定义不完善，语法可能会发生破坏性更新。
{% endhint %}

```css
::res[theme=light][desc=亮色资源] {
    /* x偏移量 | y偏移量 | 阴影模糊半径 | 阴影扩散半径 | 阴影颜色 */
    BoxShadow(popup-none): 0 0 0 0 #3666;
    BoxShadow(popup-shadow): 0 10 20 0 #3666;
}
```

