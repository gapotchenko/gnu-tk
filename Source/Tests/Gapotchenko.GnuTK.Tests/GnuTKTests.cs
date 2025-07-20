namespace Gapotchenko.GnuTK.Tests;

[TestClass]
public class GnuTKTests
{
    [TestMethod]
    [DynamicData(nameof(GnuTK_TestData_Toolkits))]
    public void GnuTK_Toolkit_Availability(string toolkit)
    {

    }

    static IEnumerable<ValueTuple<string>> GnuTK_TestData_Toolkits => TestServices.EnumerateToolkits().Select(x => ValueTuple.Create(x));
}
