%~dp0
cd ..\src\Nlnet.Avalonia.Css.App
dotnet restore -r linux-x64
dotnet deb install

dotnet msbuild Nlnet.Avalonia.Css.App.csproj /p:TargetFramework=net6.0 /p:RuntimeIdentifier=linux-x64 /p:Configuration=LinuxDebug
::dotnet msbuild Nlnet.Avalonia.Css.App.csproj /t:CreateDeb /p:TargetFramework=net6.0 /p:RuntimeIdentifier=linux-x64 /p:Configuration=LinuxDebug

pause