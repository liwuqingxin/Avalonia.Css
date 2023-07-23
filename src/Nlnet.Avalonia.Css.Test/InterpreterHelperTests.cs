using System.Diagnostics;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css.Test
{
    [TestClass]
    public class InterpreterHelperTests
    {
        [TestMethod]
        public void ParseSettersTest()
        {
            List<CssSetter>? dic = null;

            ICssBuilder builder = new CssBuilder();

            var parser = builder.Parser;

            dic = parser.ParsePairs("back:red")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 1);
            Assert.IsTrue(dic[0].RawValue == "red");

            dic = parser.ParsePairs("back:red;fore:green")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 2);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");

            dic = parser.ParsePairs("back:red;")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 1);
            Assert.IsTrue(dic[0].RawValue == "red");

            dic = parser.ParsePairs("back:red;fore:green;")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 2);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");

            dic = parser.ParsePairs("back:red fore:green;")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 1);
            Assert.IsTrue(dic[0].RawValue == "red fore:green");

            dic = parser.ParsePairs("back:red; fore:green; trans:trans[double:opacity 1 1 linear;double:opacity]")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 3);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");
            Assert.IsTrue(dic[2].RawValue == "trans[double:opacity 1 1 linear;double:opacity]");

            dic = parser.ParsePairs("back:red; fore:green; trans:trans[double:opacity 1 1 linear;double:opacity;]")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 3);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");
            Assert.IsTrue(dic[2].RawValue == "trans[double:opacity 1 1 linear;double:opacity;]");

            dic = parser.ParsePairs("back:red; fore:green; trans:trans[double:opacity 1 1 linear;double:opacity;] border:1")
                .Select(p => new CssSetter(p.Item1, p.Item2)).ToList();
            Assert.IsTrue(dic.Count == 4);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");
            Assert.IsTrue(dic[2].RawValue == "trans[double:opacity 1 1 linear;double:opacity;]");
            Assert.IsTrue(dic[3].RawValue == "1");
        }
    }
}