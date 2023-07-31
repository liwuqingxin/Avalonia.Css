using Avalonia.Controls;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

public interface IBehaviorResolverManager : IResolverManager<IBehaviorResolver>
{

}

internal class BehaviorResolverManager : ResolverManager<IBehaviorResolver>, IBehaviorResolverManager
{
    
}