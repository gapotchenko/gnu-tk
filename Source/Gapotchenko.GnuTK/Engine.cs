// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Console;
using Gapotchenko.FX.Linq;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Hosting;
using Gapotchenko.GnuTK.Toolkits;

namespace Gapotchenko.GnuTK;

/// <summary>
/// The GNU-TK engine.
/// </summary>
public sealed class Engine
{
    /// <summary>
    /// Gets or initializes a name of a toolkit to use.
    /// </summary>
    /// <value>
    /// The name of a toolkit to use,
    /// or <see langword="null"/> to select the toolkit automatically.
    /// </value>
    public string? ToolkitName { get; init; }

    /// <summary>
    /// Gets or initializes a list of portables toolkit paths.
    /// </summary>
    public IReadOnlyList<string> ToolkitPaths { get; init; } = [];

    /// <summary>
    /// Gets or initializes a value indicating whether to suppress any auxilary messages.
    /// </summary>
    public bool Quiet { get; init; }

    IToolkit GetToolkit() =>
        ToolkitServices.TrySelectToolkit(EnumerateToolkits(), ToolkitName)
        ?? throw new GnuTKDiagnosticException("No suitable GNU toolkit is found.") { ErrorCode = DiagnosticErrorCodes.NoSuitableToolkitIsFound };

    /// <summary>
    /// Lists all available toolkits.
    /// </summary>
    public void ListToolkits()
    {
        bool quiet = Quiet;
        bool useColor = !quiet && ConsoleTraits.IsColorEnabled;

        bool hasToolkits = false;

        foreach (var toolkit in EnumerateToolkits())
        {
            const int delimeterWidth = 2;
            const int toolkitColumnWidth = 14 + delimeterWidth;
            const int descriptionColumnWidth = 19 + delimeterWidth;

            if (!hasToolkits)
            {
                if (!quiet)
                {
                    if (useColor)
                        Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Available GNU Toolkits");
                    if (useColor)
                        Console.ResetColor();
                    Console.WriteLine();

                    Console.WriteLine();
                }

                Console.Write("Toolkit".PadRight(toolkitColumnWidth));
                Console.Write("Description".PadRight(descriptionColumnWidth));
                Console.WriteLine("Location");

                Console.WriteLine("------------------------------------------------------------------------------");

                hasToolkits = true;
            }

            Console.Write(toolkit.Name.PadRight(toolkitColumnWidth));
            Console.Write(toolkit.Description.PadRight(descriptionColumnWidth));
            Console.Write(toolkit.InstallationPath);
            Console.WriteLine();
        }

        if (!hasToolkits)
            Console.WriteLine("No available GNU toolkits are found.");

        if (quiet)
            return;

        Console.WriteLine();

        if (useColor)
            Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Tips:");
        if (useColor)
            Console.ResetColor();
        Console.WriteLine();

        var families = ToolkitServices.ToolkitFamilies;
        if (families is [])
        {
            Console.WriteLine(
                """
                  - GNU-TK is not aware of any GNU toolkits that can be used on the current
                    operating system
                """);
        }
        else
        {
            var traits = families.Select(x => x.Traits).Aggregate((a, b) => a | b);

            if (traits.HasFlag(ToolkitFamilyTraits.Installable))
            {
                Console.WriteLine(
                    """
                      - To add a GNU toolkit to the list, you can install it:
                        https://gapt.ch/help/gnu-tk/install-toolkits
                    """);
            }

            if (traits.HasFlag(ToolkitFamilyTraits.Portable))
            {
                char ps = Path.PathSeparator;
                Console.WriteLine(
                    $"""
                      - You can use 'GNU_TK_TOOLKIT_PATH' environment variable to specify the
                        directory paths of portable GNU toolkits, separated by a '{ps}' character
                    """);
            }

            if ((traits & ToolkitFamilyTraits.DeployableMask) != 0)
            {
                string os = HostServices.OSName;
                var deployableFamilies = families.Where(x => (x.Traits & ToolkitFamilyTraits.DeployableMask) != 0);
                string supportedToolkits = string.Join(", ", deployableFamilies.Select(x => x.Name).Order());
                Console.WriteLine(
                    $"""
                      - GNU toolkits supported on {os}: {supportedToolkits}
                    """);
            }
            else
            {
                Console.WriteLine(
                    """
                      - GNU-TK is not aware of any GNU toolkits that can be used on the current
                        operating system except the built-in one(s)
                    """);
            }
        }
    }

    /// <summary>
    /// Checks the toolkit.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the toolkit check passes;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CheckToolkit()
    {
        bool quiet = Quiet;

        var toolkits = EnumerateToolkits().Memoize();
        var toolkit = ToolkitServices.TrySelectToolkit(toolkits, ToolkitName);

        if (toolkit is null)
        {
            if (toolkits.Any())
            {
                Console.WriteLine("No suitable GNU toolkit is found.");

                if (!quiet)
                {
                    Console.WriteLine();
                    Console.WriteLine("To list all available toolkits, use 'gnu-tk list' command.");
                }
            }
            else
            {
                Console.WriteLine("No available GNU toolkits are found.");

                if (!quiet)
                {
                    Console.WriteLine();
                    Console.WriteLine("Learn more: https://gapt.ch/help/gnu-tk/install-toolkits");
                }
            }
            return false;
        }

        bool useColor = !quiet && ConsoleTraits.IsColorEnabled;

        if (!quiet)
        {
            if (useColor)
                Console.ForegroundColor = ConsoleColor.White;
            Console.Write("GNU Toolkit Check");
            if (useColor)
                Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }

        Console.WriteLine("Toolkit: {0}", toolkit.Name);
        Console.WriteLine("Description: {0}", toolkit.Description);
        Console.WriteLine("Location: {0}", toolkit.InstallationPath);

        if (!quiet)
            Console.WriteLine();
        Console.Write("Check status: ");
        if (useColor)
            Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("PASS");
        if (useColor)
            Console.ResetColor();
        Console.WriteLine();

        if (!quiet)
        {
            Console.WriteLine();

            if (useColor)
                Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Tips:");
            if (useColor)
                Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine(
                """
                  - To list all available toolkits, use 'gnu-tk list' command
                  - To change the selected toolkit, use '--toolkit' command-line option or its
                    shorthand '-t'
                  - Alternatively, you can set 'GNU_TK_TOOLKIT' environment variable to the
                    name of a GNU toolkit to use by default
                """);
        }

        return true;
    }

    IEnumerable<IToolkit> EnumerateToolkits() => ToolkitServices.EnumerateToolkits(ToolkitPaths);
}
