using Discord;
using PKHeX.Core;
using SysBot.Pokemon.Discord.Helpers;
using SysBot.Pokemon.Helpers;
using System;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord;

public static class EmbedHelper
{
    public static async Task SendNotificationEmbedAsync(IUser user, string message, string language = "en")
    {
        var embed = new EmbedBuilder()
            .WithTitle(LocalizationHelper.GetStringForLanguage("Embed_Notice_Title", language))
            .WithDescription(message)
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl("https://raw.githubusercontent.com/hexbyt3/sprites/main/exclamation.gif")
            .WithColor(Color.Red)
            .Build();

        await user.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }

    public static async Task SendTradeCanceledEmbedAsync(IUser user, string reason, string language = "en")
    {
        var embed = new EmbedBuilder()
            .WithTitle(LocalizationHelper.GetStringForLanguage("Embed_TradeCanceled_Title", language))
            .WithDescription(LocalizationHelper.GetStringForLanguage("Embed_TradeCanceled_Description", language, reason))
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl("https://raw.githubusercontent.com/hexbyt3/sprites/main/dmerror.gif")
            .WithColor(Color.Red)
            .Build();

        await user.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }

    public static async Task SendTradeCodeEmbedAsync(IUser user, int code, string language = "en")
    {
        var embed = new EmbedBuilder()
            .WithTitle(LocalizationHelper.GetStringForLanguage("Embed_TradeCode_Title", language))
            .WithDescription($"# {code:0000 0000}")
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl("https://raw.githubusercontent.com/hexbyt3/sprites/main/tradecode.gif")
            .WithColor(Color.Blue)
            .Build();

        await user.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }

    public static async Task SendTradeFinishedEmbedAsync<T>(IUser user, string message, T pk, bool isMysteryEgg, string language = "en")
        where T : PKM, new()
    {
        string thumbnailUrl;

        if (isMysteryEgg)
        {
            thumbnailUrl = "https://raw.githubusercontent.com/hexbyt3/sprites/main/mysteryegg3.png";
        }
        else
        {
            thumbnailUrl = TradeExtensions<T>.PokeImg(pk, false, true, null);
        }

        var embed = new EmbedBuilder()
            .WithTitle(LocalizationHelper.GetStringForLanguage("Embed_TradeCompleted_Title", language))
            .WithDescription(message)
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl(thumbnailUrl)
            .WithColor(Color.Teal)
            .Build();

        await user.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }

    public static async Task SendTradeInitializingEmbedAsync(IUser user, string speciesName, int code, bool isMysteryEgg, string language = "en", string? message = null)
    {
        if (isMysteryEgg)
        {
            speciesName = LocalizationHelper.GetStringForLanguage("Embed_MysteryEgg", language);
        }

        var description = LocalizationHelper.GetStringForLanguage("Embed_TradeInitializing_Description", language, speciesName, code);

        var embed = new EmbedBuilder()
            .WithTitle(LocalizationHelper.GetStringForLanguage("Embed_TradeInitializing_Title", language))
            .WithDescription(description)
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl("https://raw.githubusercontent.com/hexbyt3/sprites/main/initializing.gif")
            .WithColor(Color.Orange);

        if (!string.IsNullOrEmpty(message))
        {
            embed.WithDescription($"{description}\n\n{message}");
        }

        var builtEmbed = embed.Build();
        await user.SendMessageAsync(embed: builtEmbed).ConfigureAwait(false);
    }

    public static async Task SendTradeSearchingEmbedAsync(IUser user, string trainerName, string inGameName, string language = "en", string? message = null)
    {
        var description = LocalizationHelper.GetStringForLanguage("Embed_TradeSearching_Description", language, trainerName, inGameName);

        var embed = new EmbedBuilder()
            .WithTitle(LocalizationHelper.GetStringForLanguage("Embed_TradeSearching_Title", language, trainerName))
            .WithDescription(description)
            .WithTimestamp(DateTimeOffset.Now)
            .WithThumbnailUrl("https://raw.githubusercontent.com/hexbyt3/sprites/main/searching.gif")
            .WithColor(Color.Green);

        if (!string.IsNullOrEmpty(message))
        {
            embed.WithDescription($"{description}\n\n{message}");
        }

        var builtEmbed = embed.Build();
        await user.SendMessageAsync(embed: builtEmbed).ConfigureAwait(false);
    }
}
