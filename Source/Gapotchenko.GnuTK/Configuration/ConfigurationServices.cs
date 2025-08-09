// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.GnuTK.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.FileProviders;

namespace Gapotchenko.GnuTK.Configuration;

static class ConfigurationServices
{
    public static string? TryGetSetting(string key) =>
        TryGetSetting(key, out string? value) ? value : null;

    public static bool TryGetSetting(string key, out string? value)
    {
        if (m_ConfigurationProvider is { } configurationProvider)
        {
            return configurationProvider.TryGet(key, out value);
        }
        else
        {
            value = default;
            return false;
        }
    }

    static readonly ConfigurationProvider? m_ConfigurationProvider = CreateConfigurationProvider();

    static ConfigurationProvider? CreateConfigurationProvider()
    {
        string? modulePath = GetModulePath();
        if (modulePath is null)
            return null;

        if (modulePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
            modulePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            modulePath = Path.Combine(
                Path.GetDirectoryName(modulePath) ?? "",
                Path.GetFileNameWithoutExtension(modulePath));
        }

        string configurationFilePath = modulePath + ".ini";
        if (!File.Exists(configurationFilePath))
            return null;

        string? baseDirectory = Path.GetDirectoryName(configurationFilePath);
        if (string.IsNullOrEmpty(baseDirectory))
            return null;

        var source = new IniConfigurationSource
        {
            Path = Path.GetFileName(configurationFilePath),
            FileProvider = new PhysicalFileProvider(baseDirectory)
        };

        var provider = new IniConfigurationProvider(source);

        try
        {
            provider.Load();
        }
        catch (Exception e)
        {
            throw new DiagnosticException(e, DiagnosticCode.ConfigurationFileLoadError);
        }

        return provider;
    }

    public static string BaseDirectory { get; } = GetBaseDirectory();

    static string GetBaseDirectory()
    {
        string? baseDirectory = Path.GetDirectoryName(GetModulePath());
        if (string.IsNullOrEmpty(baseDirectory))
            throw new InvalidOperationException("Cannot determine configuration base directory.");
        return baseDirectory;
    }

    [UnconditionalSuppressMessage("SingleFile", "IL3000", Justification = "Returns an empty string for assemblies embedded in a single-file app.")]
    static string? GetModulePath() =>
        Empty.Nullify(typeof(ConfigurationServices).Assembly.Location) ??
        Environment.ProcessPath;
}
