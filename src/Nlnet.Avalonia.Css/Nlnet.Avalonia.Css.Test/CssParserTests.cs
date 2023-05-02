using System.Diagnostics;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class CssParserTests
    {
        [TestMethod]
        public void CssParserTest()
        {
            var parser  = new CssParser();
            var cssFile = File.ReadAllText("./Assets/nlnet.blog.css");
            var styles  = parser.TryGetStyles(cssFile).ToList();
            foreach (var cssStyle in styles)
            {
                Trace.WriteLine(cssStyle.ToString());
            }
        }

        [TestMethod]
        public void EfficientCssParserTest()
        {
            var parser = new EfficientCssParser();
            var s1     = EfficientCssParser.RemoveComments("/**/abc");
            var s2     = EfficientCssParser.RemoveComments("a/*abc*/bc");
            var s3     = EfficientCssParser.RemoveComments("abc/*abc*/");
            var s4     = EfficientCssParser.RemoveComments("abc//*abc*/");
            var s5     = EfficientCssParser.RemoveComments("abc/*a/bc*/");
            var s6     = EfficientCssParser.RemoveComments("abc/*a/*bc*/");
            var s7     = EfficientCssParser.RemoveComments("abc/*a*/bc*/-");
            var s8     = EfficientCssParser.RemoveComments("abc/*abc**//-");
            Assert.AreEqual(s1, "abc");
            Assert.AreEqual(s2, "abc");
            Assert.AreEqual(s3, "abc");
            Assert.AreEqual(s4, "abc/");
            Assert.AreEqual(s5, "abc");
            Assert.AreEqual(s6, "abc");
            Assert.AreEqual(s7, "abc-");
            Assert.AreEqual(s8, "abc/-");

            var cssFile = File.ReadAllText("./Assets/nlnet.blog.css");
            var styles = parser.TryGetStyles(cssFile).ToList();
            foreach (var cssStyle in styles)
            {
                Trace.WriteLine(cssStyle.ToString());
            }
        }

        [TestMethod]
        public void TypeProviderTest()
        {
            var parser  = new EfficientCssParser();
            var cssFile = File.ReadAllText("./Assets/avalonia.controls.css");
            var styles  = parser.TryGetStyles(cssFile).ToList();

            foreach (var cssStyle in styles)
            {
                var style    = (cssStyle.ToAvaloniaStyle() as Style);
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

            var instance = CssTypeProviderManager.Instance;
        }
    }
}