using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// A manager for resource providers. If you have a <see cref="IResourceProvider"/>, which has not been
    /// put into the application's resources, that would be resource provider for other consumers,
    /// just register it to this manager.
    /// </summary>
    public interface IResourceProvidersManager
    {
        /// <summary>
        /// Register a <see cref="IResourceProvider"/>. If you have a <see cref="IResourceProvider"/>, which
        /// has not been put into the application's resources, that would be resource provider for other consumers,
        /// just register it to this manager.
        /// </summary>
        /// <param name="provider"></param>
        public void RegisterResourceProvider(IResourceProvider provider);

        /// <summary>
        /// Unregister a <see cref="IResourceProvider"/>.
        /// </summary>
        /// <param name="provider"></param>
        public void UnregisterResourceProvider(IResourceProvider provider);

        /// <summary>
        /// Try find a resource by key. Registered <see cref="IResourceProvider"/> list will be checked first, Application.Current next.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="theme"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryFindResource<T>(object key, ThemeVariant theme, out T? result);

        /// <summary>
        /// Try find a resource by key. Registered <see cref="IResourceProvider"/> list will be checked first, Application.Current next.
        /// Use Application's ThemeVariant.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryFindResource<T>(object key, out T? result);

        /// <summary>
        /// Try find a resource by key. Registered <see cref="IResourceProvider"/> list will be checked first, Application.Current next.
        /// Use Application's ThemeVariant.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryFindResource(object key, out object? result);
    }
}
