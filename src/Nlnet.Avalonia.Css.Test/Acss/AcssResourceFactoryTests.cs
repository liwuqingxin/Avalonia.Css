using Avalonia;

namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class AcssResourceFactoryTests
    {
        [TestMethod]
        public void ParseResourceTest()
        {
            // TODO 不具备可测试性（依赖了Application.Current）
            AcssBuilder.UseDefaultBuilder();
            var builder = AcssBuilder.Default;
            builder.ResourceFactory.TryGetResourceInstance("brush(info)", " Blue 0.4", out var resource);
            Assert.IsNotNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("brush(error)", "#fff 0.4", out resource);
            Assert.IsNotNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("BRUSH(info)", " #cccc 0.4", out resource);
            Assert.IsNotNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("Brush(accent)", "#ff0000 0.4", out resource);
            Assert.IsNotNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("brush(info)", "var(accent) 0.4", out resource);
            Assert.IsNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("brush(info)", " var(accent) 0.4 ", out resource);
            Assert.IsNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("brush(info)", " var(accent) 0.4", out resource);
            Assert.IsNull(resource?.BuildValue(builder));

            builder.ResourceFactory.TryGetResourceInstance("brush(info)", " var(accent) 0.4 as", out resource);
            Assert.IsNull(resource?.BuildValue(builder));
        }
    }
}