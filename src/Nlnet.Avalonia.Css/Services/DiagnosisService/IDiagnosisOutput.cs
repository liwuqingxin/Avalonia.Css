using System.Runtime.CompilerServices;

namespace Nlnet.Avalonia.Css
{
    public interface IDiagnosisOutput : IService
    {
        public void OnInfo(string message, [CallerMemberName] string? caller = null);

        public void OnError(AcssErrors error, string message, [CallerMemberName] string? caller = null);
    }
}
