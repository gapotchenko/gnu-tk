// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.Linq;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Hosting;
using Gapotchenko.GnuTK.Toolkits;
using Gapotchenko.GnuTK.UI;

namespace Gapotchenko.GnuTK;

/// <summary>
/// The GNU-TK engine.
/// </summary>
sealed class Engine
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
    /// Gets or initializes the toolkit isolation levels to use.
    /// </summary>
    /// <value>
    /// The toolkit isolation levels to use,
    /// or <see langword="null"/> to not impose restrictions on isolation levels.
    /// </value>
    public IReadOnlyList<ToolkitIsolation>? ToolkitIsolationLevels { get; init; }

    /// <summary>
    /// Gets or initializes a value indicating whether to
    /// use a toolkit with strict GNU semantics.
    /// </summary>
    public bool Strict { get; init; }

    /// <summary>
    /// Gets or initializes a value indicating whether to
    /// enable POSIX-compliant behavior.
    /// </summary>
    public bool Posix { get; init; }

    /// <summary>
    /// Gets or initializes a value indicating whether to
    /// suppress auxiliary messages.
    /// </summary>
    public bool Quiet { get; init; }

    /// <summary>
    /// Executes the specified command line.
    /// </summary>
    /// <param name="command">The command line to execute.</param>
    /// <returns>The exit code.</returns>
    public int ExecuteCommandLine(string commandLine)
    {
        var toolkit = GetToolkit();

        string command;
        if (toolkit.Family.Traits.HasFlag(ToolkitFamilyTraits.FilePathTranslation))
        {
            IReadOnlyList<string> parts = [.. CommandLine.Split(commandLine)];
            IReadOnlyList<string> newParts = [.. parts.Select(part => TranslateFilePath(toolkit, part))];
            if (newParts.SequenceEqual(parts, StringComparer.Ordinal))
            {
                // Minimize command line reconstruction which may introduce inaccuracies otherwise.

                // An example of reconstruction inaccuracy is command-line arguments containing '*' characters.
                // From the standpoint of escaping, such arguments should be quoted,
                // but this would change the semantics of a '*' character which signifies a file mask in Unix.
                // File masks are expanded by a command shell unless a string containing them is quoted.

                command = commandLine;
            }
            else
            {
                command = CommandLine.Build(newParts);
            }
        }
        else
        {
            command = commandLine;
        }

        return ExecuteCommandCore(toolkit, command, []);
    }

    /// <summary>
    /// Executes the specified command.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <returns>The exit code.</returns>
    public int ExecuteCommand(string command, IReadOnlyList<string> arguments)
    {
        var toolkit = GetToolkit();
        return ExecuteCommandCore(toolkit, command, PrepareCommandArguments(toolkit, arguments));
    }

    int ExecuteCommandCore(IScriptableToolkit toolkit, string command, IReadOnlyList<string> arguments)
    {
        return toolkit.ExecuteCommand(
            command,
            arguments,
            GetToolkitExecutionEnvironment(),
            GetToolkitExecutionOptions());
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
        return toolkit.ExecuteFile(
            path,
            PrepareCommandArguments(toolkit, arguments),
            GetToolkitExecutionEnvironment(),
            GetToolkitExecutionOptions());
    }

    static IReadOnlyList<string> PrepareCommandArguments(IScriptableToolkit toolkit, IReadOnlyList<string> arguments)
    {
        if (toolkit.Family.Traits.HasFlag(ToolkitFamilyTraits.FilePathTranslation))
            return [.. arguments.Select(argument => TranslateFilePath(toolkit, argument))];
        else
            return arguments;
    }

    static string TranslateFilePath(IScriptableToolkit toolkit, string value)
    {
        return IsTranslatableFilePath(value)
            ? toolkit.TranslateFilePath(value)
            : value;

        static bool IsTranslatableFilePath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (path.Length >= 2 && path[1] == ':' && char.IsAsciiLetter(path[0]))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }

    IScriptableToolkit GetToolkit()
    {
        var names = ToolkitNames;
        return
            TryGetToolkit(EnumerateToolkits(), names) ??
            throw new DiagnosticException(
                DiagnosticMessages.SuitableToolkitNotFound(names),
                DiagnosticCode.SuitableToolkitNotFound);
    }

    IReadOnlyDictionary<string, string?> GetToolkitExecutionEnvironment()
    {
        var environment = ToolkitEnvironment.Create();

        if (Posix)
            environment[ToolkitEnvironment.PosixlyCorrect] = string.Empty;

        if (ToolkitNames is { } toolkitNames)
            environment["GNU_TK_TOOLKIT"] = string.Join(',', toolkitNames);
        if (Strict)
            environment["GNU_TK_STRICT"] = string.Empty;

        var version = AppInformation.Current.ProductVersion;
        environment["GNU_TK_VERSION"] = Invariant($"{version.Major:D4}{version.Minor:D2}{version.Build:D2}");

        return environment;
    }

    ToolkitExecutionOptions GetToolkitExecutionOptions()
    {
        var options = ToolkitExecutionOptions.None;
        if (Strict)
            options |= ToolkitExecutionOptions.Strict;
        return options;
    }

    /// <summary>
    /// Lists all available toolkits.
    /// </summary>
    public void ListToolkits()
    {
        bool quiet = Quiet;

        bool hasToolkits = false;

        foreach (var toolkit in EnumerateToolkits().OrderBy(x => x.Name))
        {
            const int delimiterWidth = 2;
            const int nameColumnWidth = 17 + delimiterWidth;
            const int descriptionColumnWidth = 25 + delimiterWidth;

            if (!hasToolkits)
            {
                if (!quiet)
                {
                    using (UIStyles.Scope.Title(Console.Out))
                        Console.Write("Available GNU Toolkits");
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
            Console.Write(GetToolkitLocation(toolkit));
            Console.WriteLine();
        }

        if (!hasToolkits)
            Console.WriteLine("No available GNU toolkits are found.");

        if (quiet)
            return;

        // Be kind and helpful by providing automatically generated tips.

        Console.WriteLine();

        using (UIStyles.Scope.Title(Console.Out))
            Console.Write("Tips:");
        Console.WriteLine();

        var families =
            EnumerateToolkitFamilies()
            .Where(family => (family.Traits & ToolkitFamilyTraits.Unofficial) == 0)
            .Memoize();

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
                      - You can install a GNU toolkit to add it to the list:
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
                string supportedToolkits = string.Join(", ", deployableFamilies.Select(x => x.InformativeName ?? x.Name).Order());
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
        var toolkit = TryGetToolkit(toolkits, names);

        if (toolkit is null)
        {
            if (toolkits.Any() ||
                Strict && EnumerateToolkits(ToolkitServices.SupportedToolkitFamilies).Any())
            {
                using (UIStyles.Scope.Error(Console.Out))
                {
                    if (Strict)
                        Console.Write(DiagnosticMessages.SuitableStrictToolkitNotFound(names));
                    else
                        Console.Write(DiagnosticMessages.SuitableToolkitNotFound(names));
                }
                Console.WriteLine();

                if (!quiet)
                {
                    Console.WriteLine();
                    Console.WriteLine("To list all available toolkits, use 'gnu-tk list' command.");
                }
            }
            else
            {
                using (UIStyles.Scope.Error(Console.Out))
                    Console.Write("No available GNU toolkits are found.");
                Console.WriteLine();

                if (!quiet)
                {
                    Console.WriteLine();
                    Console.WriteLine("Learn more: https://gapt.ch/help/gnu-tk/install-toolkits");
                }
            }
            return false;
        }

        if (!quiet)
        {
            using (UIStyles.Scope.Title(Console.Out))
                Console.Write("GNU Toolkit Check");
            Console.WriteLine();
            Console.WriteLine();
        }

        Console.WriteLine("Name: {0}", toolkit.Name);
        Console.WriteLine("Description: {0}", toolkit.Description);
        if (GetToolkitLocation(toolkit) is { } location)
            Console.WriteLine("Location: {0}", location);
        Console.WriteLine("Semantics: {0}", toolkit.Family.Traits.HasFlag(ToolkitFamilyTraits.Alike) ? "GNU-like" : "GNU");
        Console.WriteLine(
            "Isolation: {0}",
            toolkit.Isolation switch
            {
                ToolkitIsolation.None => "none",
                ToolkitIsolation.VirtualMachine => "VM",
                ToolkitIsolation.Container => "container"
            });

        if (!quiet)
            Console.WriteLine();
        Console.Write("Check status: ");
        using (UIStyles.Scope.Success(Console.Out))
        {
            if (!quiet && (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)))
                Console.Write("✔ ");
            Console.Write("PASS");
        }
        Console.WriteLine();

        if (!quiet)
        {
            Console.WriteLine();

            using (UIStyles.Scope.Title(Console.Out))
                Console.Write("Tips:");
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

    static IScriptableToolkit? TryGetToolkit(IEnumerable<IToolkit> toolkits, IEnumerable<string>? names) =>
        ToolkitServices.TryGetScriptableToolkit(ToolkitServices.SelectToolkits(toolkits, names));

    IEnumerable<IToolkit> EnumerateToolkits() => EnumerateToolkits(EnumerateToolkitFamilies());

    IEnumerable<IToolkit> EnumerateToolkits(IEnumerable<IToolkitFamily> families)
    {
        var toolkits = ToolkitServices.EnumerateToolkits(families, ToolkitPaths);

        if (ToolkitIsolationLevels is { } isolationLevels)
        {
            toolkits = toolkits.Where(
                toolkit =>
                {
                    if (toolkit is IScriptableToolkit scriptableToolkit)
                        return isolationLevels.Contains(scriptableToolkit.Isolation);
                    else
                        return true;
                });
        }

        return toolkits;
    }

    IEnumerable<IToolkitFamily> EnumerateToolkitFamilies()
    {
        var families = ToolkitServices.SupportedToolkitFamilies.AsEnumerable();
        if (Strict)
            families = families.Where(family => !family.Traits.HasFlag(ToolkitFamilyTraits.Alike));
        return families;
    }

    static string? GetToolkitLocation(IToolkit toolkit)
    {
        return (toolkit.Traits & ToolkitTraits.BuiltIn) != 0
            ? "(built-in)"
            : toolkit.InstallationPath;
    }
}
