namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void ResourceTest()
        {
            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("brush(info): Blue 0.4;", out var resource);
            Assert.IsNotNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("brush(error): #fff 0.4;", out resource);
            Assert.IsNotNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("BRUSH(info): #cccc 0.4;", out resource);
            Assert.IsNotNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("Brush(accent): #ff0000 0.4;", out resource);
            Assert.IsNotNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("brush(info): var(accent) 0.4;", out resource);
            Assert.IsNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("brush(info): var(accent) 0.4 ;", out resource);
            Assert.IsNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("brush(info): var(accent) 0.4", out resource);
            Assert.IsNull(resource?.Value);

            ServiceLocator.GetService<ICssResourceFactory>().TryGetResourceInstance("brush(info): var(accent) 0.4 as", out resource);
            Assert.IsNull(resource?.Value);
        }
    }
}