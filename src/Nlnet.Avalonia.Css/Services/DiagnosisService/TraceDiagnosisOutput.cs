using System;
using System.Diagnostics;

namespace Nlnet.Avalonia.Css;

public class TraceDiagnosisOutput : IDiagnosisOutput
{
    void IService.Initialize()
    {

    }

    void IDiagnosisOutput.OnInfo(string message, string? caller)
    {
        Trace.WriteLine($"[{DateTime.Now:HH:mm:ss fff}] ({caller}): {message}");
    }

    void IDiagnosisOutput.OnError(AcssErrors error, string message, string? caller)
    {
        Trace.WriteLine($"[{DateTime.Now:HH:mm:ss fff}]     [Error] ({caller}): {message}");
    }
}