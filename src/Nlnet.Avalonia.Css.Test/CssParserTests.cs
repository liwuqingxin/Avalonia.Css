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
            var method = typeof(CssParser).GetNonPublicStaticMethod("RemoveComments");

            var s1 = method.Invoke(typeof(CssParser), new object[] { "/**/abc" });
            var s2 = method.Invoke(typeof(CssParser), new object[] { "a/*abc*/bc" });
            var s3 = method.Invoke(typeof(CssParser), new object[] { "abc/*abc*/" });
            var s4 = method.Invoke(typeof(CssParser), new object[] { "abc//*abc*/" });
            var s5 = method.Invoke(typeof(CssParser), new object[] { "abc/*a/bc*/" });
            var s6 = method.Invoke(typeof(CssParser), new object[] { "abc/*a/*bc*/" });
            var s7 = method.Invoke(typeof(CssParser), new object[] { "abc/*a*/bc*/-" });
            var s8 = method.Invoke(typeof(CssParser), new object[] { "abc/*abc**//-" });

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
            var        cssFile = File.ReadAllText("./Assets/nlnet.blog.css");
            ICssParser parser  = new CssParser(cssFile);
            var        styles  = parser.TryGetStyles().ToList();
            foreach (var cssStyle in styles)
            {
                Trace.WriteLine(cssStyle.ToString());
            }
        }

        [TestMethod]
        public void TypeProviderTest()
        {
            var        cssFile = File.ReadAllText("./Assets/avalonia.controls.css");
            ICssParser parser  = new CssParser(cssFile);
            var        styles  = parser.TryGetStyles().ToList();

            foreach (var cssStyle in styles)
            {
                var style    = (cssStyle.ToAvaloniaStyle(false) as Style);
                var selector = style!.Selector;
                Trace.WriteLine(selector != null ? selector.ToString() : "<selector is null.>");
            }
        }
    }
}