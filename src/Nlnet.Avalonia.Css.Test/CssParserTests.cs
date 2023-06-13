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
            var parser = new CssParser();
          
            var s1 = CssParser.RemoveComments("/**/abc".ToCharArray());
            var s2 = CssParser.RemoveComments("a/*abc*/bc".ToCharArray());
            var s3 = CssParser.RemoveComments("abc/*abc*/".ToCharArray());
            var s4 = CssParser.RemoveComments("abc//*abc*/".ToCharArray());
            var s5 = CssParser.RemoveComments("abc/*a/bc*/".ToCharArray());
            var s6 = CssParser.RemoveComments("abc/*a/*bc*/".ToCharArray());
            var s7 = CssParser.RemoveComments("abc/*a*/bc*/-".ToCharArray());
            var s8 = CssParser.RemoveComments("abc/*abc**//-".ToCharArray());

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
            var parser   = new CssParser();
            var sections = parser.GetSections(cssFile);
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
            var parser   = new CssParser();
            var css      = CssParser.RemoveComments(new Span<char>(cssFile.ToCharArray()));
            var sections = parser.GetSections(css);
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