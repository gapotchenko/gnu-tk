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
    /// Gets or initializes the names of toolkits to use.
    /// </summary>
    /// <value>
    /// The names of toolkits to use,
    /// or <see langword="null"/> to select the toolkit automatically.
    /// </value>
    public IReadOnlyList<string>? ToolkitNames { get; init; }

    /// <summary>
    /// Gets or initializes a list of portable toolkit paths.
    /// </summary>
    public IReadOnlyList<string> ToolkitPaths { get; init; } = [];

    /// <summary>
    /// Gets or initializes a value indicating whether to
    /// use a toolkit with strict GNU semantics.
    /// </summary>
    public bool Strict { get; init; }

    /// <summary>
    /// Gets or initializes a value indicating whether to
    /// suppress auxiliary messages.
    /// </summary>
    public bool Quiet { get; init; }

    /// <summary>
    /// Executes the specified command.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <returns>The exit code.</returns>
    public int ExecuteCommand(string command, IReadOnlyList<string> arguments)
    {
        var toolkit = GetToolkit();
        return toolkit.ExecuteCommand(command, arguments);
    }

    /// <summary>
    /// Executes the specified file.
    /// </summary>
    /// <param name="path">The path of file to execute.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The exit code.</returns>
    public int ExecuteFile(string path, IReadOnlyList<string> arguments)
    {
        var toolkit = GetToolkit();
        if (path == "-")
            path = "/dev/stdin";
        return toolkit.ExecuteFile(path, arguments);
    }

    IToolkit GetToolkit()
    {
        var names = ToolkitNames;
        return
            ToolkitServices.TrySelectToolkit(EnumerateToolkits(), names) ??
            throw new DiagnosticException(
                DiagnosticMessages.SuitableToolkitNotFound(names),
                DiagnosticCode.SuitableToolkitNotFound);
    }

    /// <summary>
    /// Lists all available toolkits.
    /// </summary>
    public void ListToolkits()
    {
        bool quiet = Quiet;
        bool useColor = !quiet && ConsoleTraits.IsColorEnabled;

        bool hasToolkits = false;

        foreach (var toolkit in EnumerateToolkits().OrderBy(x => x.Name))
        {
            const int delimiterWidth = 2;
            const int nameColumnWidth = 17 + delimiterWidth;
            const int descriptionColumnWidth = 24 + delimiterWidth;

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

                Console.Write("Name".PadRight(nameColumnWidth));
                Console.Write("Description".PadRight(descriptionColumnWidth));
                Console.WriteLine("Location");

                Console.WriteLine("------------------------------------------------------------------------------");

                hasToolkits = true;
            }

            Console.Write(toolkit.Name.PadRight(nameColumnWidth));
            Console.Write(toolkit.Description.PadRight(descriptionColumnWidth));
            Console.Write(toolkit.InstallationPath);
            Console.WriteLine();
        }

        if (!hasToolkits)
            Console.WriteLine("No available GNU toolkits are found.");

        if (quiet)
            return;

        // Be kind and helpful by providing automatically generated tips.

        Console.WriteLine();

        if (useColor)
            Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Tips:");
        if (useColor)
            Console.ResetColor();
        Console.WriteLine();

        var families = EnumerateToolkitFamilies().Memoize();
        if (!families.Any())
        {
            if (Strict && ToolkitServices.SupportedToolkitFamilies.Any())
            {
                Console.WriteLine(
                    """
                      - GNU-TK is not aware of any toolkits with strict GNU semantics that can be
                        used on this operating system
                    """);
            }
            else
            {
                Console.WriteLine(
                    """
                      - GNU-TK is not aware of any GNU toolkits that can be used on this operating
                        system
                    """);
            }
        }
        else
        {
            var traits = families.Select(x => x.Traits).Aggregate((a, b) => a | b);

            if (traits.HasFlag(ToolkitFamilyTraits.Installable))
            {
                Console.WriteLine(
                    """
                      - To add a GNU toolkit to the list, you can install it on your system:
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

            if ((traits & ToolkitFamilyTraits.DeploymentMask) != 0)
            {
                string os = HostEnvironment.OSName;
                var deployableFamilies = families.Where(x => (x.Traits & ToolkitFamilyTraits.DeploymentMask) != 0);
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
                      - GNU-TK is not aware of any GNU toolkits that can be used on this operating
                        system alongside the default one(s)
                    """);
            }
        }
    }

    /// <summary>
    /// Checks the toolkit.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the toolkit passes all checks;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool CheckToolkit()
    {
        bool quiet = Quiet;

        var toolkits = EnumerateToolkits().Memoize();
        var names = ToolkitNames;
        var toolkit = ToolkitServices.TrySelectToolkit(toolkits, names);

        if (toolkit is null)
        {
            if (toolkits.Any())
            {
                Console.WriteLine(DiagnosticMessages.SuitableToolkitNotFound(names));

                if (!quiet)
                {
                    Console.WriteLine();
                    Console.WriteLine("To list all available toolkits, use 'gnu-tk list' command.");
                }
            }
            else
            {
                if (Strict)
                    Console.WriteLine("No available toolkits with strict GNU semantics are found.");
                else
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

        Console.WriteLine("Name: {0}", toolkit.Name);
        Console.WriteLine("Description: {0}", toolkit.Description);
        Console.WriteLine("Location: {0}", toolkit.InstallationPath);
        Console.WriteLine("Semantics: {0}", toolkit.Family.Traits.HasFlag(ToolkitFamilyTraits.Alike) ? "GNU-like" : "GNU");

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
                """);

            if (toolkits.CountIsAtLeast(2))
            {
                Console.WriteLine(
                    """
                      - To change the selected toolkit, specify '--toolkit' command-line option or
                        its shorthand '-t'
                      - Alternatively, you can set 'GNU_TK_TOOLKIT' environment variable to the
                        name of a GNU toolkit to use by default
                    """);
            }
        }

        return true;
    }

    IEnumerable<IToolkit> EnumerateToolkits() => ToolkitServices.EnumerateToolkits(
        EnumerateToolkitFamilies(),
        ToolkitPaths);

    IEnumerable<IToolkitFamily> EnumerateToolkitFamilies()
    {
        var families = ToolkitServices.SupportedToolkitFamilies.AsEnumerable();
        if (Strict)
            families = families.Where(family => !family.Traits.HasFlag(ToolkitFamilyTraits.Alike));
        return families;
    }
}
