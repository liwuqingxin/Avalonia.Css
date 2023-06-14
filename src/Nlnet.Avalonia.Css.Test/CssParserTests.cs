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
            var parser = ServiceLocator.GetService<ICssParser>();

            var s1 = parser.RemoveComments("/**/abc".ToCharArray());
            var s2 = parser.RemoveComments("a/*abc*/bc".ToCharArray());
            var s3 = parser.RemoveComments("abc/*abc*/".ToCharArray());
            var s4 = parser.RemoveComments("abc//*abc*/".ToCharArray());
            var s5 = parser.RemoveComments("abc/*a/bc*/".ToCharArray());
            var s6 = parser.RemoveComments("abc/*a/*bc*/".ToCharArray());
            var s7 = parser.RemoveComments("abc/*a*/bc*/-".ToCharArray());
            var s8 = parser.RemoveComments("abc/*abc**//-".ToCharArray());

            Assert.AreEqual(s1.ToString(), "abc");
            Assert.AreEqual(s2.ToString(), "abc");
            Assert.AreEqual(s3.ToString(), "abc");
            Assert.AreEqual(s4.ToString(), "abc/");
            Assert.AreEqual(s5.ToString(), "abc");
            Assert.AreEqual(s6.ToString(), "abc");
            Assert.AreEqual(s7.ToString(), "abc-");
            Assert.AreEqual(s8.ToString(), "abc/-");
        }

        [TestMethod]
        public void EfficientCssParserTest()
        {
            var cssFile  = File.ReadAllText("./Assets/nlnet.blog.css");
            var parser   = ServiceLocator.GetService<ICssParser>();
            var sections = parser.ParseSections(cssFile);
            var styles   = sections.OfType<ICssStyle>();
            foreach (var cssStyle in styles)
            {
                Trace.WriteLine(cssStyle.ToString());
            }
        }

        [TestMethod]
        public void TypeProviderTest()
        {
            var cssFile  = File.ReadAllText("./Assets/avalonia.controls.css");
            var parser   = ServiceLocator.GetService<ICssParser>();
            var css      = parser.RemoveComments(new Span<char>(cssFile.ToCharArray()));
            var sections = parser.ParseSections(css);
            var styles   = sections.OfType<ICssStyle>();

            foreach (var cssStyle in styles)
            {
                var style    = cssStyle.ToAvaloniaStyle();
                var selector = style!.Selector;
                Trace.WriteLine(selector != null ? selector.ToString() : "<selector is null.>");
            }
        }
    }
}