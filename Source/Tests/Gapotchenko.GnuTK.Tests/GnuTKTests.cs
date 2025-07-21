namespace Gapotchenko.GnuTK.Tests;

[TestClass]
public class GnuTKTests
{
    [TestMethod]
    [DynamicData(nameof(GnuTK_TestData_Toolkits))]
    public void GnuTK_Toolkit_Availability(string toolkit)
    {
        Console.WriteLine("Tool path: {0}", TestServices.ToolPath);
    }

    static IEnumerable<ValueTuple<string>> GnuTK_TestData_Toolkits =>
        TestServices.EnumerateToolkits()
        .Append("auto")
        .Select(x => ValueTuple.Create(x));
}
