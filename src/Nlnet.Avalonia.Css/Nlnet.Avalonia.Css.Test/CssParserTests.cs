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
            var parser  = new CssParser();
            var cssFile = File.ReadAllText("./Assets/nlnet.blog.css");
            var styles  = parser.TryGetStyles(cssFile).ToList();
            foreach (var cssStyle in styles)
            {
                Trace.WriteLine(cssStyle.ToString());
            }
        }

        [TestMethod]
        public void TypeProviderTest()
        {
            var parser  = new CssParser();
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
        }
    }
}