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
        string directoryPath = Path.Combine(TestServices.BasePath, "Tests", "Shell");

        // By using a full script path, we also test the file mapping between the host and GNU environments.
        string scriptPath = Path.Combine(directoryPath, "run-tests.sh");

        // File asset path argument is passed to test the file mapping between the host and GNU environments.
        string fileAssetPath = Path.Combine(directoryPath, "assets", "file.txt");

        var environment = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["GNU_TK"] = TestServices.ToolPath
        };

        Assert.AreEqual(
            0,
            ShellServices.ExecuteProcess(
                TestServices.ToolPath,
                ["-t", toolkit, "-f", scriptPath, fileAssetPath],
                environment: environment));
    }

    static IEnumerable<string> GnuTK_TestData_Toolkits => TestServices.EnumerateToolkits();
}
