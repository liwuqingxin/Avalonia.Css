# Nlnet.Avalonia.Css.Behaviors

This library provides extended behaviors for Acss. We have provided two behaviors, those belong to Acss, for instruction. We will continue to provide common behaviors for Acss. Any request for common behaviors could be sent to us at [here](https://github.com/liwuqingxin/Avalonia.Css/issues/new).

- **combobox.popup.align**. This behavior will align the selected item of the drop down list to the ComboBox.
- **window.esc.close**. This behavior let the window accept esc key to close it.

> Note that we haven't used the Avalonia.Xaml.Interactivity component. You can still independently use Acss Behavior and Avalonia.Xaml.Interactivity without worrying about any mutual impact or conflicts.

## Custom Behaviors

Acss supports custom behaviors. Nlnet.Avalonia.Css.CompileGenerator package could be used for that.

1. Add package Nlnet.Avalonia.Css.CompileGenerator.

2. Create behavior declarer class that derived from `AvaloniaObject` and implement the `IBehaviorDeclarer` interface.

```csharp
public partial class CustomA : AvaloniaObject, IBehaviorDeclarer
{
        
}
```

3. Provide a extension method for users to use CustomA.

```csharp
    /// <summary>
    /// Use customA behavior feature for default css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseCustomABehaviorForDefaultBuilder(this AppBuilder builder)
    {
        AcssBuilder.Default.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<CustomA>());
        AcssBuilder.Default.BehaviorDeclarerManager.RegisterDeclarer<CustomA>(nameof(CustomA).ToLower());
        AcssBuilder.Default.BehaviorDeclarerManager.RegisterDeclarer<CustomA>(nameof(CustomA));

        return builder;
    }

    /// <summary>
    /// Use customA behavior feature.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cssBuilder"></param>
    /// <returns></returns>
    public static AppBuilder UseCustomABehavior(this AppBuilder builder, IAcssBuilder cssBuilder)
    {
        cssBuilder.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<CustomA>());
        cssBuilder.BehaviorDeclarerManager.RegisterDeclarer<CustomA>(nameof(CustomA).ToLower());
        cssBuilder.BehaviorDeclarerManager.RegisterDeclarer<CustomA>(nameof(CustomA));

        return builder;
    }
```

3. Create behavior class derived from AcssBehavior<> and add BehaviorAttribute to it. The name 'window.esc.close' will be the name that used in acss file.

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

## Usage

- Use the acss behaviors for default builder in `BuildAvaloniaApp()`. Also you can use the behaviors in your own AcssBuilder with method `AcssBehaviorsExtensions.UseAcssBehavior()`.

```csharp
private static AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace()
        .UseReactiveUI()
        .LogToLocalFile()

        // Use avalonia css stuff.
        .UseAcssDefaultBuilder()

        // [Optional] Use acss behavior.
        .UseAcssBehaviorForDefaultBuilder();
    	
    	// [Optional] Use customA behavior.
    	.UseCustomABehaviorForDefaultBulder();
}
```

- Use the behaviors in acss.

```css
ComboBoxPage ComboBox#C1{
    .acss:combobox.popup.align;
}

NtWindow#RootWindow{
    .acss:window.esc.close;
}
```

