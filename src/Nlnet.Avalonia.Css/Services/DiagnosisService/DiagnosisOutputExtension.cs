using System.Runtime.CompilerServices;

namespace Nlnet.Avalonia.Css;

internal static class DiagnosisOutputExtension
{
    public static void OnInfo(this IAcssContext ctx, string message, [CallerMemberName] string? caller = null)
    {
        ctx.GetServiceIfExist<IDiagnosisOutput>()?.OnInfo(message, caller);
    }

    public static void OnError(this IAcssContext ctx, AcssErrors error, string message, [CallerMemberName] string? caller = null)
    {
        ctx.GetServiceIfExist<IDiagnosisOutput>()?.OnError(error, message, caller);
    }
}