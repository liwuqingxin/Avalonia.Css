using System.Diagnostics;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class CssParserTests
    {
        [TestMethod]
        public void RemoveCommentsTest()
        {
            var s1     = CssParser.RemoveComments("/**/abc");
            var s2     = CssParser.RemoveComments("a/*abc*/bc");
            var s3     = CssParser.RemoveComments("abc/*abc*/");
            var s4     = CssParser.RemoveComments("abc//*abc*/");
            var s5     = CssParser.RemoveComments("abc/*a/bc*/");
            var s6     = CssParser.RemoveComments("abc/*a/*bc*/");
            var s7     = CssParser.RemoveComments("abc/*a*/bc*/-");
            var s8     = CssParser.RemoveComments("abc/*abc**//-");
            Assert.AreEqual(s1, "abc");
            Assert.AreEqual(s2, "abc");
            Assert.AreEqual(s3, "abc");
            Assert.AreEqual(s4, "abc/");
            Assert.AreEqual(s5, "abc");
            Assert.AreEqual(s6, "abc");
            Assert.AreEqual(s7, "abc-");
            Assert.AreEqual(s8, "abc/-");
        }

        [TestMethod]
        public void EfficientCssParserTest()
        {
            var cssFile = File.ReadAllText("./Assets/nlnet.blog.css");
            var parser  = new CssParser(cssFile);
            var styles  = parser.TryGetStyles().ToList();
            foreach (var cssStyle in styles)
            {
                Trace.WriteLine(cssStyle.ToString());
            }
        }

        [TestMethod]
        public void TypeProviderTest()
        {
            var cssFile = File.ReadAllText("./Assets/avalonia.controls.css");
            var parser  = new CssParser(cssFile);
            var styles  = parser.TryGetStyles().ToList();

            foreach (var cssStyle in styles)
            {
                var style    = (cssStyle.ToAvaloniaStyle(false) as Style);
                var selector = style!.Selector;
                if (selector != null)
                {
                    Trace.WriteLine(selector.ToString());
                }
                else
                {
                    Trace.WriteLine("<selector is null.>");
                }
            }
        }

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
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4 ;", out resource);
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4", out resource);
            Assert.IsNotNull(resource?.Value);

            CssResourceFactory.TryGetResourceInstance("brush(info): var(accent) 0.4 as", out resource);
            Assert.IsNotNull(resource?.Value);
        }
    }
}