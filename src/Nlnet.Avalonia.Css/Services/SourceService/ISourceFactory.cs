using System;

namespace Nlnet.Avalonia.Css
{
    public interface ISourceFactory : IService
    {
        public ISource CreateDependentSource(ISource source, string keyPath, bool alignPathToCurrent);
    }

    internal class DefaultSourceFactory : ISourceFactory
    {
        ISource ISourceFactory.CreateDependentSource(ISource source, string keyPath, bool alignPathToCurrent)
        {
            if (keyPath.StartsWith("avares://"))
            {
                return new EmbeddedSource(new Uri(keyPath));
            }

            return source.CreateFromPath(keyPath, alignPathToCurrent);
        }

        void IService.Initialize()
        {
            
        }
    }
}
