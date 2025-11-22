using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace SysBot.Pokemon.Discord.Helpers;

/// <summary>
/// Helper class for handling localized strings
/// </summary>
public static class LocalizationHelper
{
    private static readonly ResourceManager ResourceManager = new("SysBot.Pokemon.Discord.Resources.Localization", typeof(LocalizationHelper).Assembly);

    // Thread-local culture for per-request language detection
    private static readonly AsyncLocal<CultureInfo?> _currentCulture = new();

    // German command aliases for auto-detection
    private static readonly HashSet<string> GermanAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        "tausch", "tauschen", "tau",
        "verstecktertausch", "vt",
        "ei",
        "reparieren", "rep",
        "medaillen", "md",
        "warteschlangestatus", "ws", "position", "pos",
        "warteschlangeraus", "wr",
        "warteschlangeliste", "wl", "liste",
        "warteschlangealles", "wal",
        "tauschcodelöschen", "tcl",
        "tauschbenutzer", "tb",
        "warteschlangemodus", "wm",
        "hallo", "moin",
        "infos", "über",
        "statistik", "stat"
    };

    /// <summary>
    /// Detects and sets the culture based on the command message
    /// </summary>
    /// <param name="messageContent">The full message content from Discord</param>
    public static void DetectAndSetCulture(string messageContent)
    {
        if (string.IsNullOrWhiteSpace(messageContent))
        {
            _currentCulture.Value = CultureInfo.GetCultureInfo("en");
            return;
        }

        // Extract the command (first word after prefix, e.g., "!tausch" -> "tausch")
        var parts = messageContent.TrimStart().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            _currentCulture.Value = CultureInfo.GetCultureInfo("en");
            return;
        }

        var command = parts[0].TrimStart('!', '.', '/'); // Remove common prefixes

        // Check if it's a German alias
        if (GermanAliases.Contains(command))
        {
            _currentCulture.Value = CultureInfo.GetCultureInfo("de");
        }
        else
        {
            _currentCulture.Value = CultureInfo.GetCultureInfo("en");
        }
    }

    /// <summary>
    /// Sets the current culture for localization
    /// </summary>
    /// <param name="cultureName">Culture name (e.g., "en", "de")</param>
    public static void SetCulture(string cultureName)
    {
        _currentCulture.Value = CultureInfo.GetCultureInfo(cultureName);
    }

    /// <summary>
    /// Gets the currently detected language code
    /// </summary>
    /// <returns>Language code (e.g., "en", "de")</returns>
    public static string GetCurrentLanguageCode()
    {
        var culture = _currentCulture.Value ?? CultureInfo.GetCultureInfo("en");
        return culture.TwoLetterISOLanguageName;
    }

    /// <summary>
    /// Gets a localized string by key
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <returns>Localized string</returns>
    public static string GetString(string key)
    {
        var culture = _currentCulture.Value ?? CultureInfo.GetCultureInfo("en");
        return ResourceManager.GetString(key, culture) ?? key;
    }

    /// <summary>
    /// Gets a localized string with format arguments
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <param name="args">Format arguments</param>
    /// <returns>Formatted localized string</returns>
    public static string GetString(string key, params object[] args)
    {
        var culture = _currentCulture.Value ?? CultureInfo.GetCultureInfo("en");
        var format = ResourceManager.GetString(key, culture) ?? key;
        return string.Format(format, args);
    }

    /// <summary>
    /// Gets a localized string by key with a specific language
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <param name="languageCode">Language code (e.g., "en", "de")</param>
    /// <returns>Localized string</returns>
    public static string GetStringForLanguage(string key, string languageCode)
    {
        var culture = CultureInfo.GetCultureInfo(languageCode);
        return ResourceManager.GetString(key, culture) ?? key;
    }

    /// <summary>
    /// Gets a localized string with format arguments for a specific language
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <param name="languageCode">Language code (e.g., "en", "de")</param>
    /// <param name="args">Format arguments</param>
    /// <returns>Formatted localized string</returns>
    public static string GetStringForLanguage(string key, string languageCode, params object[] args)
    {
        var culture = CultureInfo.GetCultureInfo(languageCode);
        var format = ResourceManager.GetString(key, culture) ?? key;
        return string.Format(format, args);
    }
}
