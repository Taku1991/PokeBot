using System.Globalization;
using System.Resources;

namespace SysBot.Pokemon.Discord.Helpers;

/// <summary>
/// Helper class for handling localized strings
/// </summary>
public static class LocalizationHelper
{
    private static readonly ResourceManager ResourceManager = new("SysBot.Pokemon.Discord.Resources.Localization", typeof(LocalizationHelper).Assembly);
    
    // Default to English, can be changed per server in future
    private static CultureInfo _currentCulture = CultureInfo.GetCultureInfo("en");
    
    /// <summary>
    /// Sets the current culture for localization
    /// </summary>
    /// <param name="cultureName">Culture name (e.g., "en", "de")</param>
    public static void SetCulture(string cultureName)
    {
        _currentCulture = CultureInfo.GetCultureInfo(cultureName);
    }
    
    /// <summary>
    /// Gets a localized string by key
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <returns>Localized string</returns>
    public static string GetString(string key)
    {
        return ResourceManager.GetString(key, _currentCulture) ?? key;
    }
    
    /// <summary>
    /// Gets a localized string with format arguments
    /// </summary>
    /// <param name="key">Resource key</param>
    /// <param name="args">Format arguments</param>
    /// <returns>Formatted localized string</returns>
    public static string GetString(string key, params object[] args)
    {
        var format = ResourceManager.GetString(key, _currentCulture) ?? key;
        return string.Format(format, args);
    }
}
