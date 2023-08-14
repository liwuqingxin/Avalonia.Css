- Installation.

```bash
dotnet add package Nlnet.Avalonia.Css --version 11.0.0
```

- Use default Acss builder in Avalonia's AppBuilder. Then load all types. Alternatively, you can create your own Acss builder instead.

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

- Initialize the builder. In this section, you can setup the configuration like theme, create rider settings and load acss files.  

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