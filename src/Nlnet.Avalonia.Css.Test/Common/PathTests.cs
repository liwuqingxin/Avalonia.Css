namespace Nlnet.Avalonia.Css.Test;

[TestClass]
public class PathTests
{
    [TestMethod]
    public void StandardPathTest()
    {
        var p1 = ".././A/B/c.exe";
        var s1 = p1.GetStandardPath();
        
        var p2 = "..\\.\\\\A/B\\\\c.exe";
        var s2 = p2.GetStandardPath();
        
        Assert.IsTrue(string.Equals(s1, s2));
    }
}