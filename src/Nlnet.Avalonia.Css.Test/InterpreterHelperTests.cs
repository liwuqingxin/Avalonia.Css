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

            dic = InterpreterHelper.ParseSetters("back:red").ToList();
            Assert.IsTrue(dic.Count == 1);
            Assert.IsTrue(dic[0].RawValue == "red");

            dic = InterpreterHelper.ParseSetters("back:red;fore:green").ToList();
            Assert.IsTrue(dic.Count == 2);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");

            dic = InterpreterHelper.ParseSetters("back:red;").ToList();
            Assert.IsTrue(dic.Count == 1);
            Assert.IsTrue(dic[0].RawValue == "red");

            dic = InterpreterHelper.ParseSetters("back:red;fore:green;").ToList();
            Assert.IsTrue(dic.Count == 2);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");

            dic = InterpreterHelper.ParseSetters("back:red fore:green;").ToList();
            Assert.IsTrue(dic.Count == 1);
            Assert.IsTrue(dic[0].RawValue == "red fore:green");

            dic = InterpreterHelper.ParseSetters("back:red; fore:green; trans:trans[double:opacity 1 1 linear;double:opacity]").ToList();
            Assert.IsTrue(dic.Count == 3);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");
            Assert.IsTrue(dic[2].RawValue == "trans[double:opacity 1 1 linear;double:opacity]");

            dic = InterpreterHelper.ParseSetters("back:red; fore:green; trans:trans[double:opacity 1 1 linear;double:opacity;]").ToList();
            Assert.IsTrue(dic.Count == 3);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");
            Assert.IsTrue(dic[2].RawValue == "trans[double:opacity 1 1 linear;double:opacity;]");

            dic = InterpreterHelper.ParseSetters("back:red; fore:green; trans:trans[double:opacity 1 1 linear;double:opacity;] border:1").ToList();
            Assert.IsTrue(dic.Count == 4);
            Assert.IsTrue(dic[0].RawValue == "red");
            Assert.IsTrue(dic[1].RawValue == "green");
            Assert.IsTrue(dic[2].RawValue == "trans[double:opacity 1 1 linear;double:opacity;]");
            Assert.IsTrue(dic[3].RawValue == "1");
        }
    }
}