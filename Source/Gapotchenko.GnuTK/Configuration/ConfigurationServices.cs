// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

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
        string? processPath = Environment.ProcessPath;
        if (processPath is null)
            return null;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            processPath = Path.Combine(
                Path.GetDirectoryName(processPath) ?? "",
                Path.GetFileNameWithoutExtension(processPath));
        }

        string configurationFilePath = processPath + ".ini";
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
        string? baseDirectory = Path.GetDirectoryName(Environment.ProcessPath);
        if (string.IsNullOrEmpty(baseDirectory))
            throw new InvalidOperationException("Cannot determine configuration base directory.");
        return baseDirectory;
    }
}
