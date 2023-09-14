using System.Diagnostics;

namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class AcssParserTests
    {
        private class AcssSectionFactoryForTest : IAcssSectionFactory
        {
            public IAcssSection Build(IAcssParser parser, AcssTokens tokens, IAcssSection? parent, string selector, ReadOnlySpan<char> content)
            {
                return null!;
            }
        }

        private static IAcssParser GetParserForTest()
        {
            var parser = new AcssParser(new AcssSectionFactoryForTest());
            return parser;
        }
        
        private static IAcssParser GetParser()
        {
            var builder = AcssBuilder.Default;
            var parser = builder.Parser;
            return parser;
        }
        
        [TestMethod]
        public void TestRemoveComments()
        {
            var parser = GetParserForTest();

            var s1 = parser.RemoveComments("/**/abc".ToCharArray());
            var s2 = parser.RemoveComments("a/*abc*/bc".ToCharArray());
            var s3 = parser.RemoveComments("abc/*abc*/".ToCharArray());
            var s4 = parser.RemoveComments("abc//*abc*/".ToCharArray());
            var s5 = parser.RemoveComments("abc/*a/bc*/".ToCharArray());
            var s6 = parser.RemoveComments("abc/*a/*bc*/".ToCharArray());
            var s7 = parser.RemoveComments("abc/*a*/bc*/-".ToCharArray());
            var s8 = parser.RemoveComments("abc/*abc**//-".ToCharArray());
            var s9 = parser.RemoveComments("ab/*c//asdf*/asd".ToCharArray());
            var s10 = parser.RemoveComments("a//bc\rdef".ToCharArray());
            var s11 = parser.RemoveComments("abc//d\nef".ToCharArray());
            var s12 = parser.RemoveComments("ab//c\r\ndef".ToCharArray());
            var s13 = parser.RemoveComments("ab//c\r\n//d\rdef\nghi\r\njk\n\rlmn".ToCharArray());
            
            Assert.AreEqual(s1.ToString(), "abc");
            Assert.AreEqual(s2.ToString(), "abc");
            Assert.AreEqual(s3.ToString(), "abc");
            Assert.AreEqual(s4.ToString(), "abc");
            Assert.AreEqual(s5.ToString(), "abc");
            Assert.AreEqual(s6.ToString(), "abc");
            Assert.AreEqual(s7.ToString(), "abc-");
            Assert.AreEqual(s8.ToString(), "abc/-");
            Assert.AreEqual(s9.ToString(), "abasd");
            Assert.AreEqual(s10.ToString(), "a\rdef");
            Assert.AreEqual(s11.ToString(), "abc\nef");
            Assert.AreEqual(s12.ToString(), "ab\r\ndef");
            Assert.AreEqual(s13.ToString(), "ab\r\n\rdef\nghi\r\njk\n\rlmn");
        }

        [TestMethod]
        public void TestParseImportsAndRelies()
        {
            var parser = GetParserForTest();

            parser.ParseImportsBasesAndRelies("import ./button/btn.acss;\r\nimport ./button.acss;\r\nrely ./button/btn.acss;\r\nrely ./button/btn.acss\r\ncontent", out var imports, out var bases, out var relies, out var contentSpan);
            var importsList = imports.ToList();
            var reliesList = relies.ToList();
            Assert.AreEqual(importsList.Count, 2);
            Assert.AreEqual(importsList[0], "./button/btn.acss");
            Assert.AreEqual(importsList[1], "./button.acss");
            Assert.AreEqual(reliesList.Count, 2);
            Assert.AreEqual(reliesList[0], "./button/btn.acss");
            Assert.AreEqual(reliesList[1], "./button/btn.acss");
            Assert.AreEqual(contentSpan.ToString(), "content");
            
            
            parser.ParseImportsBasesAndRelies("import ./button/btn.acss;\r\nimport  s\r\nimport      s", out var imports2, out var bases2, out var relies2, out var contentSpan2);
            var importsList2 = imports2.ToList();
            var reliesList2 = relies2.ToList();
            Assert.AreEqual(importsList2.Count, 2);
            
            
            parser.ParseImportsBasesAndRelies("rely ./button/btn.acss;\r\nrely  s\r\nrely      s", out var imports3, out var bases3, out var relies3, out var contentSpan3);
            var importsList3 = imports3.ToList();
            var reliesList3 = relies3.ToList();
            Assert.AreEqual(reliesList3.Count, 2);
        }
        
        [TestMethod]
        public void TestParseCollectionObjects()
        {
            var parser = GetParserForTest();

            var s1 = parser.ParseCollectionObjects("children1[ Background: Red;  Foreground: Green; \r\n]\r\nchildren2[ \r\nBackground: Red;  Foreground: Green; ]").ToList();
            var s2 = parser.ParseCollectionObjects(" children1 \r\n[\r\n Background: Red;  Foreground: Green; ]children2 [ Background: Red;  Foreground: Green; ]").ToList();
            
            // foreach (var tuple in s1)
            // {
            //     Trace.WriteLine($"'{tuple.Item1}', '{tuple.Item2}'");
            // }
            
            Assert.AreEqual(s1.Count, 2);
            Assert.AreEqual(s1[0].Item1, "children1");
            Assert.AreEqual(s1[0].Item2, "Background: Red;  Foreground: Green;");
            Assert.AreEqual(s1[1].Item1, "children2");
            Assert.AreEqual(s1[1].Item2, "Background: Red;  Foreground: Green;");
            
            Assert.AreEqual(s2.Count, 2);
            Assert.AreEqual(s2[0].Item1, "children1");
            Assert.AreEqual(s2[0].Item2, "Background: Red;  Foreground: Green;");
            Assert.AreEqual(s2[1].Item1, "children2");
            Assert.AreEqual(s2[1].Item2, "Background: Red;  Foreground: Green;");
        }

        [TestMethod]
        public void TestParseSections()
        {
            // TODO 9.3
        }

        [TestMethod]
        public void TestParseSettersAndChildren()
        {
            var parser = GetParserForTest();

            parser.ParseSettersAndChildren("background: red;children :\r\n[\r\nvar(stBack); var(stFore); \r\nvar(stBorder)\r\n]\r\n[[\r\n selector{background:red;fore:green} \r\n]] foreground: yellow", out var s1, out var s2);
            
            Assert.AreEqual(s1.ToString(), "background: red;children :\r\n[\r\nvar(stBack); var(stFore); \r\nvar(stBorder)\r\n]\r\n");
            Assert.AreEqual(s2.ToString(), "\r\n selector{background:red;fore:green} \r\n");
        }

        [TestMethod]
        public void TestParsePairs()
        {
            var parser = GetParserForTest();

            var s1 = parser.ParsePairs("Backgroud:red").ToList();
            var s2 = parser.ParsePairs("Backgroud:red;  \r\nforeground \r\n :  \r\ngreen; margin \r\n:12,2,2,1 \r\n").ToList();
            var s3 = parser.ParsePairs("Backgroud:red; fore:green;children :\r\n[\r\nvar(stBack); var(stFore); \r\nvar(stBorder)\r\n]").ToList();
            
            Assert.AreEqual(s1.Count, 1);
            Assert.AreEqual(s1[0].Item1, "Backgroud");
            Assert.AreEqual(s1[0].Item2, "red");
            
            Assert.AreEqual(s2.Count, 3);
            Assert.AreEqual(s2[0].Item1, "Backgroud");
            Assert.AreEqual(s2[0].Item2, "red");
            Assert.AreEqual(s2[1].Item1, "foreground");
            Assert.AreEqual(s2[1].Item2, "green");
            Assert.AreEqual(s2[2].Item1, "margin");
            Assert.AreEqual(s2[2].Item2, "12,2,2,1");
            
            Assert.AreEqual(s3.Count, 3);
            Assert.AreEqual(s3[0].Item1, "Backgroud");
            Assert.AreEqual(s3[0].Item2, "red");
            Assert.AreEqual(s3[1].Item1, "fore");
            Assert.AreEqual(s3[1].Item2, "green");
            Assert.AreEqual(s3[2].Item1, "children");
            Assert.AreEqual(s3[2].Item2, "[\r\nvar(stBack); var(stFore); \r\nvar(stBorder)\r\n]");
        }
    }
}