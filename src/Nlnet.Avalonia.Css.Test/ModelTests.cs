using System.Diagnostics;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void ResourceTest()
        {
            CssResourceFactory.TryGetResourceInstance("brush(info): Blue 0.4;", out var resource);
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(error): #fff 0.4;", out resource);
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("BRUSH(info): #cccc 0.4;", out resource);
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("Brush(accent): #ff0000 0.4;", out resource);
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4;", out resource);
            Assert.IsNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4 ;", out resource);
            Assert.IsNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4", out resource);
            Assert.IsNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4 as", out resource);
            Assert.IsNull(resource?.Value);
        }
    }
}