%~dp0
cd ..\src\Nlnet.Avalonia.Css.App
dotnet msbuild Nlnet.Avalonia.Css.App.csproj /p:TargetFramework=net6.0 /p:RuntimeIdentifier=linux-x64 /p:Configuration=LinuxDebug

pause