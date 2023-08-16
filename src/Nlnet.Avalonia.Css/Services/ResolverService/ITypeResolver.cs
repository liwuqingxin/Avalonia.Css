using System;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Interface that can provide the service of resolving type from string.
/// </summary>
public interface ITypeResolver : IResolver
{
    
}

/// <summary>
/// A generic implementation for <see cref="ITypeResolver"/> that can provide type resolving service for assembly who contains the class of <see cref="TTypeSink"/>.
/// </summary>
/// <typeparam name="TTypeSink"></typeparam>
public class GenericTypeResolver<TTypeSink> : Resolver<TTypeSink>, ITypeResolver
{

}