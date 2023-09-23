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
            AcssContext.UseDefaultContext();
            var ctx = AcssContext.Default;
            var resourceFactory = ctx.GetService<IAcssResourceFactory>();
            resourceFactory.TryGetResourceInstance("brush(info)", " Blue 0.4", out var resource);
            Assert.IsNotNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("brush(error)", "#fff 0.4", out resource);
            Assert.IsNotNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("BRUSH(info)", " #cccc 0.4", out resource);
            Assert.IsNotNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("Brush(accent)", "#ff0000 0.4", out resource);
            Assert.IsNotNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("brush(info)", "var(accent) 0.4", out resource);
            Assert.IsNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("brush(info)", " var(accent) 0.4 ", out resource);
            Assert.IsNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("brush(info)", " var(accent) 0.4", out resource);
            Assert.IsNull(resource?.BuildValue(ctx));

            resourceFactory.TryGetResourceInstance("brush(info)", " var(accent) 0.4 as", out resource);
            Assert.IsNull(resource?.BuildValue(ctx));
        }
    }
}