// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Console;
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

    /// <summary>
    /// Lists all available toolkits.
    /// </summary>
    public void ListToolkits()
    {
        bool useColor = ConsoleTraits.IsColorEnabled;
        bool hasToolkits = false;

        foreach (var toolkit in EnumerateToolkits())
        {
            const int delimeterWidth = 2;
            const int toolkitColumnWidth = 14 + delimeterWidth;
            const int descriptionColumnWidth = 19 + delimeterWidth;

            if (!hasToolkits)
            {
                if (!Quiet)
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

        if (Quiet)
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

    IEnumerable<IToolkit> EnumerateToolkits() => ToolkitServices.EnumerateToolkits(ToolkitPaths);
}
