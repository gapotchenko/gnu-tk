// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Tests;

[TestClass]
public class GnuTKTests
{
    [TestMethod]
    [DynamicData(nameof(GnuTK_TestData_Toolkits))]
    public void GnuTK_Toolkit_Availability(string toolkit)
    {
        Assert.AreEqual(
            0,
            ShellServices.ExecuteProcess(
                TestServices.ToolPath,
                ["-t", toolkit, "check", "-q"]));
    }

    [TestMethod]
    [DynamicData(nameof(GnuTK_TestData_Toolkits))]
    public void GnuTK_Toolkit_ShellTests(string toolkit)
    {
        string workingDirectory = Path.Combine(TestServices.BasePath, "Tests", "Shell");

        var environment = new Dictionary<string, string?>
        {
            ["GNU_TK"] = TestServices.ToolPath
        };

        Assert.AreEqual(
            0,
            ShellServices.ExecuteProcess(
                TestServices.ToolPath,
                ["-t", toolkit, "-f", "run-tests.sh"],
                workingDirectory,
                environment));
    }

    static IEnumerable<string> GnuTK_TestData_Toolkits => TestServices.EnumerateToolkits();
}
