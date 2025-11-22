using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SysBot.Pokemon.Helpers;

/// <summary>
/// Translates German Showdown format sets to English for parsing
/// </summary>
public static class GermanShowdownTranslator
{
    private static readonly Dictionary<string, string> GermanToEnglishKeywords = new()
    {
        // Stats
        { "KP", "HP" },
        { "Angr", "Atk" },
        { "Vert", "Def" },
        { "SpAng", "SpA" },
        { "SpVert", "SpD" },
        { "Init", "Spe" },
        
        // Keywords
        { "Level", "Level" },
        { "Shiny", "Shiny" },
        { "Fähigkeit", "Ability" },
        { "Wesen", "Nature" },
        { "EVs", "EVs" },
        { "IVs", "IVs" },
        { "Ball", "Ball" },
        { "Glückspunkte", "Happiness" },
        { "Freundschaft", "Friendship" },
        { "Tera-Typ", "Tera Type" },
        { "Teratyp", "Tera Type" },
        { "Alpha", "Alpha" },
        { "Gigadynamax", "Gigantamax" },
        { "Sprache", "Language" },
        
        // Boolean values
        { "Ja", "Yes" },
        { "Nein", "No" },
        
        // Gender
        { "Männlich", "M" },
        { "Weiblich", "F" },
        
        // Languages
        { "Deutsch", "German" },
        { "Englisch", "English" },
        { "Französisch", "French" },
        { "Italienisch", "Italian" },
        { "Spanisch", "Spanish" },
        { "Japanisch", "Japanese" },
        { "Koreanisch", "Korean" },
    };

    /// <summary>
    /// Checks if content contains German keywords
    /// </summary>
    public static bool IsGermanShowdown(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        // Check for common German keywords
        return content.Contains("Fähigkeit") ||
               content.Contains("Wesen") ||
               content.Contains("Freundschaft") ||
               content.Contains("Tera-Typ") ||
               content.Contains("Teratyp") ||
               content.Contains("SpAng") ||
               content.Contains("SpVert") ||
               content.Contains("Init") ||
               content.Contains("Angr") ||
               content.Contains("Vert");
    }

    /// <summary>
    /// Translates a German Showdown set to English
    /// </summary>
    public static string TranslateToEnglish(string germanSet)
    {
        if (string.IsNullOrWhiteSpace(germanSet))
            return germanSet;

        var lines = germanSet.Split('\n');
        var translatedLines = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                translatedLines.Add(line);
                continue;
            }

            var translatedLine = line;

            // Handle first line (Species @ Item)
            if (!line.StartsWith("-") && !line.Contains(":") && !line.StartsWith("."))
            {
                translatedLine = TranslateSpeciesLine(line);
            }
            // Handle move lines (starting with -)
            else if (line.TrimStart().StartsWith("-"))
            {
                translatedLine = TranslateMoveLine(line);
            }
            // Handle property lines (Key: Value)
            else if (line.Contains(":"))
            {
                translatedLine = TranslatePropertyLine(line);
            }
            // Handle batch commands (starting with .)
            else if (line.TrimStart().StartsWith("."))
            {
                // Batch commands stay as-is, they use numeric codes
                translatedLine = line;
            }

            translatedLines.Add(translatedLine);
        }

        return string.Join("\n", translatedLines);
    }

    private static string TranslateSpeciesLine(string line)
    {
        // Format: Species (Gender) @ Item
        // or: Species @ Item
        // or just: Species
        
        var result = line;

        // Translate item name if present
        if (result.Contains("@"))
        {
            var parts = result.Split('@');
            if (parts.Length == 2)
            {
                var itemPart = parts[1].Trim();
                var translatedItem = TranslateItem(itemPart);
                result = $"{parts[0].Trim()} @ {translatedItem}";
            }
        }

        // Translate species name
        result = TranslateSpeciesName(result);

        // Translate gender indicators (with optional spaces, case-insensitive)
        result = Regex.Replace(result, @"\(\s*Männlich\s*\)", "(M)", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\(\s*Weiblich\s*\)", "(F)", RegexOptions.IgnoreCase);

        return result;
    }

    private static string TranslateMoveLine(string line)
    {
        // Format: - MoveName
        var trimmed = line.TrimStart();
        if (!trimmed.StartsWith("-"))
            return line;

        var moveName = trimmed.Substring(1).Trim();
        var translatedMove = TranslateMove(moveName);

        return line.Replace(moveName, translatedMove);
    }

    private static string TranslatePropertyLine(string line)
    {
        var colonIndex = line.IndexOf(':');
        if (colonIndex < 0)
            return line;

        var key = line.Substring(0, colonIndex).Trim();
        var value = line.Substring(colonIndex + 1).Trim();

        // Translate the key
        var translatedKey = GermanToEnglishKeywords.TryGetValue(key, out var engKey) ? engKey : key;

        // Translate the value based on the key type
        var translatedValue = value;

        // Special handling for EVs and IVs
        if (key == "EVs" || key == "IVs")
        {
            translatedValue = TranslateStats(value);
        }
        // Translate nature
        else if (key == "Wesen")
        {
            translatedValue = TranslateNature(value);
        }
        // Translate ability
        else if (key == "Fähigkeit")
        {
            translatedValue = TranslateAbility(value);
        }
        // Translate ball
        else if (key == "Ball")
        {
            translatedValue = TranslateBall(value);
        }
        // Translate boolean values
        else if (value == "Ja" || value == "Nein")
        {
            translatedValue = GermanToEnglishKeywords[value];
        }
        // Translate language values
        else if (key == "Sprache")
        {
            translatedValue = GermanToEnglishKeywords.TryGetValue(value, out var lang) ? lang : value;
        }

        return $"{translatedKey}: {translatedValue}";
    }

    private static string TranslateStats(string stats)
    {
        // Format: "252 KP / 252 SpAng / 6 Init"
        var result = stats;
        foreach (var kvp in GermanToEnglishKeywords)
        {
            // Use word boundaries to avoid partial replacements, case-insensitive
            result = Regex.Replace(result, $@"\b{Regex.Escape(kvp.Key)}\b", kvp.Value, RegexOptions.IgnoreCase);
        }
        return result;
    }

    private static string TranslateSpeciesName(string speciesText)
    {
        // Get German strings from PKHeX
        var germanStrings = GameInfo.GetStrings("de");
        var englishStrings = GameInfo.GetStrings("en");

        // Extract just the species name (before @ or parentheses)
        var speciesMatch = Regex.Match(speciesText, @"^([^\(@]+)");
        if (!speciesMatch.Success)
            return speciesText;

        var speciesName = speciesMatch.Groups[1].Value.Trim();

        // Find the species index in German strings
        for (int i = 0; i < germanStrings.specieslist.Length && i < englishStrings.specieslist.Length; i++)
        {
            if (germanStrings.specieslist[i].Equals(speciesName, StringComparison.OrdinalIgnoreCase))
            {
                // Replace with English name
                return speciesText.Replace(speciesName, englishStrings.specieslist[i]);
            }
        }

        return speciesText; // Return original if not found
    }

    private static string TranslateMove(string moveName)
    {
        var germanStrings = GameInfo.GetStrings("de");
        var englishStrings = GameInfo.GetStrings("en");

        // Find the move index in German strings
        for (int i = 0; i < germanStrings.movelist.Length && i < englishStrings.movelist.Length; i++)
        {
            if (germanStrings.movelist[i].Equals(moveName, StringComparison.OrdinalIgnoreCase))
            {
                return englishStrings.movelist[i];
            }
        }

        return moveName; // Return original if not found
    }

    private static string TranslateNature(string natureName)
    {
        var germanStrings = GameInfo.GetStrings("de");
        var englishStrings = GameInfo.GetStrings("en");

        // Find the nature index in German strings
        for (int i = 0; i < germanStrings.natures.Length && i < englishStrings.natures.Length; i++)
        {
            if (germanStrings.natures[i].Equals(natureName, StringComparison.OrdinalIgnoreCase))
            {
                return englishStrings.natures[i];
            }
        }

        return natureName; // Return original if not found
    }

    private static string TranslateAbility(string abilityName)
    {
        var germanStrings = GameInfo.GetStrings("de");
        var englishStrings = GameInfo.GetStrings("en");

        // Find the ability index in German strings
        for (int i = 0; i < germanStrings.abilitylist.Length && i < englishStrings.abilitylist.Length; i++)
        {
            if (germanStrings.abilitylist[i].Equals(abilityName, StringComparison.OrdinalIgnoreCase))
            {
                return englishStrings.abilitylist[i];
            }
        }

        return abilityName; // Return original if not found
    }

    private static string TranslateBall(string ballName)
    {
        var germanStrings = GameInfo.GetStrings("de");
        var englishStrings = GameInfo.GetStrings("en");

        // Find the ball index in German strings
        for (int i = 0; i < germanStrings.balllist.Length && i < englishStrings.balllist.Length; i++)
        {
            if (germanStrings.balllist[i].Equals(ballName, StringComparison.OrdinalIgnoreCase))
            {
                return englishStrings.balllist[i];
            }
        }

        return ballName; // Return original if not found
    }

    private static string TranslateItem(string itemName)
    {
        var germanStrings = GameInfo.GetStrings("de");
        var englishStrings = GameInfo.GetStrings("en");

        // Find the item index in German strings
        for (int i = 0; i < germanStrings.itemlist.Length && i < englishStrings.itemlist.Length; i++)
        {
            if (germanStrings.itemlist[i].Equals(itemName, StringComparison.OrdinalIgnoreCase))
            {
                return englishStrings.itemlist[i];
            }
        }

        return itemName; // Return original if not found
    }
}
