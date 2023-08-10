<div align="left">
  <a href="https://github.com/microsoft/dotnet">
    <img src="https://img.shields.io/badge/-.NET-red">
  </a>
  <a href="https://avaloniaui.net/">
    <img src="https://img.shields.io/badge/Avalonia-8245A9">
  </a>
  <a href="https://dotnet.microsoft.com/zh-cn/languages/csharp">
    <img src="https://img.shields.io/badge/-C%23-yellow">
  </a>
  <a href="http://www.gnu.org/licenses/mit.en.html">
    <img src="https://img.shields.io/badge/License-MIT-green">
  </a>
  <a href="https://www.cnblogs.com/liwuqingxin/">
    <img src="https://img.shields.io/badge/Blog-NLNet-orange">
  </a>
  <a href="https://github.com/liwuqingxin">
    <img src="https://img.shields.io/badge/Github-Liwuqingxin-blue?logo=GitHub">
  </a>
</div>



![acss-brand](img/brand.svg)

Avalonia.Css is not a library that fully adheres to the standard CSS (Cascading Style Sheets). The primary purpose is to **separate the structural and visual definitions** of Avalonia UI and empower the Avalonia framework with the ability to **dynamically modify visual styles quickly**. 

:smile: Yes, it follows a pattern similar to **Html+CSS**.

<img src="src/Nlnet.Avalonia.Css.App/Assets/brand.svg" height="60"/>

## Scenarios

Let's provide some examples to illustrate its use cases.

🌰 Imagine we have a standard UI control library, where the functionality of its internal controls depends on the structure within their templates. When we want to modify the visual styles of the entire control library, we usually need to rewrite all ControlTheme. This means we have to mix control structure and visual styles, even rewriting resources. It's a painful process.

Hence, the idea of Acss was born. Picture this: By adopting the Acss pattern, we extract the visual styles from the standard theme of the control library, retaining only the skeletal structure, i.e., ControlTemplate. Any new visual library can be based on this standard themed structure, defining its own Acss library to achieve the desired visual styles. Creators of the visual library don't need to painfully copy existing ControlTheme code for modifications, they don't need to worry about whether the control structure is correct. They only need to adjust visual styles based on UI design!

> Theme = UI Structure + Visual Styles

🌰 Another example: When we create a new page, the traditional workflow usually involves layout, content population, business logic writing, style adjustments, and fine-tuning based on UI design. This is typically a developer's task, requiring continuous attention until the page is completed. Now, with the Acss pattern, the approach would involve functional personnel populating page content, writing business logic, and then handing it over to those who focus on visuals to harmonize the visual styles.

> Focusing separately on macro functions or visual details will make our work smoother and more comprehensive!

<img src="src/Nlnet.Avalonia.Css.App/Assets/brand.svg" height="60"/>

## Showcase

🌰 Fluent theme for Avalonia controls powered by Acss. 

![acss](img/fluent.gif)



🌰 Ability of hot reloading.

![acss](img/acss.gif)



🌰 Here is a button definition of structure and visual style.

```xaml
<!-- 
	Button Structure in xaml. 
-->
<ControlTheme x:Key="{x:Type Button}" TargetType="Button">
    <Setter Property="Button.Template">
        <ControlTemplate TargetType="Button">
            <ContentPresenter x:Name="PART_ContentPresenter"
                              Padding="{TemplateBinding Padding}"
                              HorizontalContentAlignment="{TemplateBinding HorizontalContentAlignment}"
                              VerticalContentAlignment="{TemplateBinding VerticalContentAlignment}"
                              Background="{TemplateBinding Background}"
                              BorderBrush="{TemplateBinding BorderBrush}"
                              BorderThickness="{TemplateBinding BorderThickness}"
                              Content="{TemplateBinding Content}"
                              ContentTemplate="{TemplateBinding ContentTemplate}"
                              CornerRadius="{TemplateBinding CornerRadius}" />
        </ControlTemplate>
    </Setter>
</ControlTheme>
```

```css
// 
// Button Visual Style in acss.
//
^Button {
    Foreground: var(fore);
    Background: var(control-back);
    BorderBrush:var(control-border-fluent);
    BorderThickness: 1;
    CornerRadius: 4;
    Padding: 8,4,8,5;
    Opacity: 1;
    MinHeight:28;
    RenderTransform: none;
    HorizontalContentAlignment: Center;
    HorizontalAlignment: Center;
    VerticalContentAlignment: Center;
    VerticalAlignment: Center;
    ClipToBounds:False;
    Transitions:[
        var(stRenderTransform);
        var(stBackground);
        var(stBorderBrush);
        var(stOpacity);
    ]

    [[
        :pointerover{
            Foreground: var(fore-hover);
            Background: var(control-back-hover);
        }
        :pressed{
            /* RenderTransform:scale(0.97,0.97); */
            //Opacity:0.6;
            Foreground:var(fore-pressed);
            BorderBrush:var(control-border-pressed-fluent);
        }
        :disabled{
            Opacity:0.4;
        }
    
        // Primary class and Accent style.
        .primary, .Primary, [(ButtonStyles.Style)=Accent] {
            Background: var(Accent);
            Foreground: #fff;
            BorderBrush: var(control-border-accent-fluent);
            [[
                :pointerover{
                    Background: var(Accent09);
                    Foreground: #fff;
                }
                :pressed{
                    Opacity:0.8;
                    BorderBrush: var(control-border-accent-fluent-pressed);
                }
            ]]
        }
    ]]
}
```



<img src="src/Nlnet.Avalonia.Css.App/Assets/brand.svg" height="60"/>

## Features

### 🟢 Separation of Concerns

As mentioned earlier, Acss promotes the separation of structural and visual definitions of UI. Building upon the MVVM pattern, it further divides the view into structure and style.

### 🟢 Dynamic Changes

Acss functions as an interpreted language, where all style objects are created at runtime, naturally supporting dynamic changes to styles during runtime. We can easily switch global or local styles or collections of styles for an application.

> :fire: Importantly, it enables **hot reloading** of styles, allowing us to run the program, simultaneously write style code, and see real-time effects and feedback. No need for previews as we can see the results directly.

### 🟢 Intercept and Filter

Acss maps interpretive code to Avalonia style memory objects. During this mapping, we can intercept specified types of styles, properties, animations, resources, etc., and filter or replace them.

### 🟢 Behavioral Extensions

We provide a behavioral pattern that supports custom extensions, and it comes with built-in behaviors (Acss Behavior). Examples include "**esc to close window**", "**align the selected item to ComboBox**", **adding shortcuts,** **adding predefined animations,** etc. We will continue expanding and updating these built-in behaviors to enhance ease of use for Acss.

> Note that we haven't used the Avalonia.Xaml.Interactivity component. You can still independently use Acss Behavior and Avalonia.Xaml.Interactivity without worrying about any mutual impact or conflicts.

### 🟢 Custom Drawing

We will offer a built-in AcssBorder, which, while implementing the same functionality as Border, provides external drawing API and access to these API within the Acss syntax. This means we can perform some custom drawing within Acss code.

### 🟢 Syntax Extensions

Based on the aforementioned code and object mapping process, we can extend usage beyond standard Avalonia syntax. For instance, we can define Transition resources and use them directly in the Transitions property. While this falls under static resources, it greatly supports unified resource management needs.

### 🟢 Style Debugging and Rewriting

We will provide an Acss-related style debugger, showcasing the entire process from parsing, loading, applying, to detaching. You can clearly see all Acss style objects presented in the program and perform actions on them, including disabling, loading, etc.

> Additionally, we will provide rewriting functionality, generating change code for manually modified styles and offering the ability to write back to code files or specified output streams.

### 🟢 Security Concerns

Since Acss code files may be exposed in external static text files, potential security issues may arise. Please refer to the [Plans | Security](### Security) section for more details. If this issue is crucial to you, carefully decide whether to use Acss based on our instructions and your specific situation.

<img src="src/Nlnet.Avalonia.Css.App/Assets/brand.svg" height="60"/>

# Usage

- Installation.

```bash
dotnet add package Nlnet.Avalonia.Css --version 11.0.0
```

- Use default Acss builder in Avalonia's AppBuilder. Then load all types.

```csharp
private static AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        .UsePlatformDetect()
        ...
        
        // Use default avalonia css builder.
        .UseAcssDefaultBuilder()
        
        // Type resolver for 'Your.Lib'. The GenericTypeResolver<T> will load all types 
        // those belong to the assembly who contains the T class.
        .WithTypeResolverForAcssDefaultBuilder(new GenericTypeResolver<Icon>())
        
        // [Optional] Use avalonia behavior.
        .UseAcssBehaviorForDefaultBuilder();
}
```

- Initialize the builder.

```csharp
private class void Initialize()
{
    ...
	
    // [Optional] Set the current theme.
    AcssBuilder.Default.Configuration.Theme = "blue";
    
    // [Optional] 
    // Build the rider settings file. We do not provide Rider plugin to support
    // Acss language till now. 
    // Use this for a temporaty replacement.
    AcssBuilder.Default.TryBuildRiderSettingsForAcss(out _, out _, null);
    
    // Load acss files. You can keep the cssFile for more operations.
    var loader = AcssBuilder.Default.BuildLoader();
	var cssFile = loader.Load(Application.Current.Styles, "Acss/app.acss");
    
    // Or load acss files from a folder.
    loader.LoadFolder(this, "Acss/");
}
```

>  More about using of Acss, see the code of [Nlnet.Avalonia.Css.App](src/Nlnet.Avalonia.Css.App) and [Nlnet.Avalonia.Css.Fluent](src/Nlnet.Avalonia.Css.Fluent).

<img src="src/Nlnet.Avalonia.Css.App/Assets/brand.svg" height="60"/>

## Contribution

We hope more interested partners will join us in enhancing and expanding Acss, making it more vibrant and capable.

If you're interested in getting involved, have ideas, suggestions, or feedback, please send an email to yangqi1990917@163.com. In the email, you could tell us about:

- About you.
- What can we do for you?
- How would you like to get involved in this project?
- Something else you would like to say.

<img src="src/Nlnet.Avalonia.Css.App/Assets/brand.svg" height="60"/>

## Plans

Plans see [here](doc/Plans.md).