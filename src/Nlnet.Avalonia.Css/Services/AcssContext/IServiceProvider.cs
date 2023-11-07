using System.Collections.Generic;

namespace Nlnet.Avalonia.Css;

public interface IServiceProvider
{
    /// <summary>
    /// Add a <see cref="IService"/> to the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="service"></param>
    public void UseService(IService service);

    /// <summary>
    /// Add a <see cref="IService"/> to the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void UseService<T>() where T : IService, new();

    /// <summary>
    /// Add a <see cref="IService"/> that is implemented as <see cref="TImpl"/> to the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    public void UseService<TService, TImpl>() where TService : IService where TImpl : TService, new();

    /// <summary>
    /// Add a <see cref="IService"/> instance as service of <see cref="TService"/>.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="service"></param>
    public void UseService<TService>(TService service) where TService : IService;

    /// <summary>
    /// Get the first service of <see cref="T"/>.  If it does not exist, exception will be thrown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetService<T>() where T : class, IService;

    /// <summary>
    /// Get the first service of <see cref="T"/>.  If it does not exist return null instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetServiceIfExist<T>() where T : class, IService;

    /// <summary>
    /// Try to get the first service of <see cref="T"/> if it exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <returns></returns>
    public bool TryGetService<T>(out T service) where T : class, IService;

    /// <summary>
    /// Get all services of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> GetServices<T>() where T : class, IService;
}