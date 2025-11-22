using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using SysBot.Base;
using SysBot.Pokemon.Discord.Helpers;
using SysBot.Pokemon.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord;

[Summary("Queues new Link Code trades")]
public class TradeModule<T> : ModuleBase<SocketCommandContext> where T : PKM, new()
{
    private static TradeQueueInfo<T> Info => SysCord<T>.Runner.Hub.Queues.Info;

    #region Medal Achievement Command

    [Command("medals")]
    [Alias("ml", "medaillen", "md")]
    [Summary("Shows your current trade count and medal status")]
    public async Task ShowMedalsCommand()
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        var tradeCodeStorage = new TradeCodeStorage();
        int totalTrades = tradeCodeStorage.GetTradeCount(Context.User.Id);

        if (totalTrades == 0)
        {
            await ReplyAsync(LocalizationHelper.GetString("Trade_NoTradesYet", Context.User.Username));
            return;
        }

        int currentMilestone = MedalHelpers.GetCurrentMilestone(totalTrades);
        var embed = MedalHelpers.CreateMedalsEmbed(Context.User, currentMilestone, totalTrades);
        await Context.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
    }

    #endregion

    #region Trade Commands

    [Command("trade")]
    [Alias("t", "tausch", "tauschen", "tau")]
    [Summary("Makes the bot trade you a Pokémon converted from the provided Showdown Set.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task TradeAsync([Summary("Showdown Set")][Remainder] string content)
    {
        var userID = Context.User.Id;
        var code = Info.GetRandomTradeCode(userID);
        return ProcessTradeAsync(code, content);
    }

    [Command("trade")]
    [Alias("t")]
    [Summary("Makes the bot trade you a Pokémon converted from the provided Showdown Set.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task TradeAsync([Summary("Trade Code")] int code, [Summary("Showdown Set")][Remainder] string content)
        => ProcessTradeAsync(code, content);

    [Command("trade")]
    [Alias("t")]
    [Summary("Makes the bot trade you the provided Pokémon file.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task TradeAsyncAttach([Summary("Trade Code")] int code, [Summary("Ignore AutoOT")] bool ignoreAutoOT = false)
    {
        var sig = Context.User.GetFavor();
        return ProcessTradeAttachmentAsync(code, sig, Context.User, ignoreAutoOT: ignoreAutoOT);
    }

    [Command("trade")]
    [Alias("t")]
    [Summary("Makes the bot trade you the attached file.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public async Task TradeAsyncAttach([Summary("Ignore AutoOT")] bool ignoreAutoOT = false)
    {
        var userID = Context.User.Id;
        var code = Info.GetRandomTradeCode(userID);
        var sig = Context.User.GetFavor();

        await Task.Run(async () =>
        {
            await ProcessTradeAttachmentAsync(code, sig, Context.User, ignoreAutoOT: ignoreAutoOT).ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    [Command("hidetrade")]
    [Alias("ht", "verstecktertausch", "vt")]
    [Summary("Makes the bot trade you a Pokémon without showing trade embed details.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task HideTradeAsync([Summary("Showdown Set")][Remainder] string content)
    {
        var userID = Context.User.Id;
        var code = Info.GetRandomTradeCode(userID);
        return ProcessTradeAsync(code, content, isHiddenTrade: true);
    }

    [Command("hidetrade")]
    [Alias("ht")]
    [Summary("Makes the bot trade you a Pokémon without showing trade embed details.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task HideTradeAsync([Summary("Trade Code")] int code, [Summary("Showdown Set")][Remainder] string content)
        => ProcessTradeAsync(code, content, isHiddenTrade: true);

    [Command("hidetrade")]
    [Alias("ht")]
    [Summary("Makes the bot trade you the provided file without showing trade embed details.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task HideTradeAsyncAttach([Summary("Trade Code")] int code, [Summary("Ignore AutoOT")] bool ignoreAutoOT = false)
    {
        var sig = Context.User.GetFavor();
        return ProcessTradeAttachmentAsync(code, sig, Context.User, isHiddenTrade: true, ignoreAutoOT: ignoreAutoOT);
    }

    [Command("hidetrade")]
    [Alias("ht")]
    [Summary("Makes the bot trade you the attached file without showing trade embed details.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public async Task HideTradeAsyncAttach([Summary("Ignore AutoOT")] bool ignoreAutoOT = false)
    {
        var userID = Context.User.Id;
        var code = Info.GetRandomTradeCode(userID);
        var sig = Context.User.GetFavor();

        await ProcessTradeAttachmentAsync(code, sig, Context.User, isHiddenTrade: true, ignoreAutoOT: ignoreAutoOT).ConfigureAwait(false);

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    [Command("tradeUser")]
    [Alias("tu", "tradeOther", "tauschBenutzer", "tb")]
    [Summary("Makes the bot trade the mentioned user the attached file.")]
    [RequireSudo]
    public async Task TradeAsyncAttachUser([Summary("Trade Code")] int code, [Remainder] string _)
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        if (Context.Message.MentionedUsers.Count > 1)
        {
            await ReplyAsync(LocalizationHelper.GetString("Trade_TooManyMentions")).ConfigureAwait(false);
            return;
        }

        if (Context.Message.MentionedUsers.Count == 0)
        {
            await ReplyAsync(LocalizationHelper.GetString("Trade_MustMentionUser")).ConfigureAwait(false);
            return;
        }

        var usr = Context.Message.MentionedUsers.ElementAt(0);
        var sig = usr.GetFavor();
        await ProcessTradeAttachmentAsync(code, sig, usr).ConfigureAwait(false);
    }

    [Command("tradeUser")]
    [Alias("tu", "tradeOther", "tauschBenutzer", "tb")]
    [Summary("Makes the bot trade the mentioned user the attached file.")]
    [RequireSudo]
    public Task TradeAsyncAttachUser([Remainder] string _)
    {
        var userID = Context.User.Id;
        var code = Info.GetRandomTradeCode(userID);
        return TradeAsyncAttachUser(code, _);
    }

    #endregion

    #region Special Trade Commands

    [Command("egg")]
    [Alias("Egg", "ei", "Ei")]
    [Summary("Trades an egg generated from the provided Pokémon name.")]
    [RequireQueueRole(nameof(DiscordManager.RolesEgg))]
    public async Task TradeEgg([Remainder] string egg)
    {
        var userID = Context.User.Id;
        var code = Info.GetRandomTradeCode(userID);
        await TradeEggAsync(code, egg).ConfigureAwait(false);
    }

    [Command("egg")]
    [Alias("Egg", "ei", "Ei")]
    [Summary("Trades an egg generated from the provided Pokémon name.")]
    [RequireQueueRole(nameof(DiscordManager.RolesEgg))]
    public async Task TradeEggAsync([Summary("Trade Code")] int code, [Summary("Showdown Set")][Remainder] string content)
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                LocalizationHelper.GetString("Trade_AlreadyInQueue"), 2);
            return;
        }

        content = ReusableActions.StripCodeBlock(content);

        // Translate German Showdown sets to English
        if (GermanShowdownTranslator.IsGermanShowdown(content))
        {
            content = GermanShowdownTranslator.TranslateToEnglish(content);
        }

        var set = new ShowdownSet(content);
        var template = AutoLegalityWrapper.GetTemplate(set);

        _ = Task.Run(async () =>
        {
            try
            {
                var sav = AutoLegalityWrapper.GetTrainerInfo<T>();

                // Generate the egg using ALM's GenerateEgg method
                var pkm = sav.GenerateEgg(template, out var result);

                if (result != LegalizationResult.Regenerated)
                {
                    var reason = result == LegalizationResult.Timeout
                        ? LocalizationHelper.GetString("Trade_EggGenerationTimeout")
                        : LocalizationHelper.GetString("Trade_EggGenerationFailed");
                    await Helpers<T>.ReplyAndDeleteAsync(Context, reason, 2);
                    return;
                }

                pkm = EntityConverter.ConvertToType(pkm, typeof(T), out _) ?? pkm;
                if (pkm is not T pk)
                {
                    await Helpers<T>.ReplyAndDeleteAsync(Context, LocalizationHelper.GetString("Trade_EggConversionFailed"), 2);
                    return;
                }

                var sig = Context.User.GetFavor();
                await Helpers<T>.AddTradeToQueueAsync(Context, code, Context.User.Username, pk, sig, Context.User).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogUtil.LogSafe(ex, nameof(TradeModule<T>));
                await Helpers<T>.ReplyAndDeleteAsync(Context, "An error occurred while processing the request.", 2);
            }
        });

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    [Command("fixOT")]
    [Alias("fix", "f", "reparieren", "rep")]
    [Summary("Fixes OT and Nickname of a Pokémon you show via Link Trade if an advert is detected.")]
    [RequireQueueRole(nameof(DiscordManager.RolesFixOT))]
    public async Task FixAdOT()
    {
        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "You already have an existing trade in the queue that cannot be cleared. Please wait until it is processed.", 2);
            return;
        }

        var code = Info.GetRandomTradeCode(userID);
        await ProcessFixOTAsync(code);
    }

    [Command("fixOT")]
    [Alias("fix", "f", "reparieren", "rep")]
    [Summary("Fixes OT and Nickname of a Pokémon you show via Link Trade if an advert is detected.")]
    [RequireQueueRole(nameof(DiscordManager.RolesFixOT))]
    public async Task FixAdOT([Summary("Trade Code")] int code)
    {
        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "You already have an existing trade in the queue that cannot be cleared. Please wait until it is processed.", 2);
            return;
        }

        await ProcessFixOTAsync(code);
    }

    private async Task ProcessFixOTAsync(int code)
    {
        var trainerName = Context.User.Username;
        var sig = Context.User.GetFavor();
        var lgcode = Info.GetRandomLGTradeCode();

        await QueueHelper<T>.AddToQueueAsync(Context, code, trainerName, sig, new T(),
            PokeRoutineType.FixOT, PokeTradeType.FixOT, Context.User, false, 1, 1, false, false, lgcode: lgcode).ConfigureAwait(false);

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    [Command("dittoTrade")]
    [Alias("dt", "ditto")]
    [Summary("Makes the bot trade you a Ditto with a requested stat spread and language.")]
    public async Task DittoTrade([Summary("A combination of \"ATK/SPA/SPE\" or \"6IV\"")] string keyword,
        [Summary("Language")] string language, [Summary("Nature")] string nature)
    {
        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "You already have an existing trade in the queue that cannot be cleared. Please wait until it is processed.", 2);
            return;
        }

        var code = Info.GetRandomTradeCode(userID);
        await ProcessDittoTradeAsync(code, keyword, language, nature);
    }

    [Command("dittoTrade")]
    [Alias("dt", "ditto")]
    [Summary("Makes the bot trade you a Ditto with a requested stat spread and language.")]
    public async Task DittoTrade([Summary("Trade Code")] int code,
        [Summary("A combination of \"ATK/SPA/SPE\" or \"6IV\"")] string keyword,
        [Summary("Language")] string language, [Summary("Nature")] string nature)
    {
        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "You already have an existing trade in the queue that cannot be cleared. Please wait until it is processed.", 2);
            return;
        }

        await ProcessDittoTradeAsync(code, keyword, language, nature);
    }

    private async Task ProcessDittoTradeAsync(int code, string keyword, string language, string nature)
    {
        keyword = keyword.ToLower().Trim();

        if (!Enum.TryParse(language, true, out LanguageID lang))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context, $"Couldn't recognize language: {language}.", 2);
            return;
        }

        nature = nature.Trim()[..1].ToUpper() + nature.Trim()[1..].ToLower();
        var set = new ShowdownSet($"{keyword}(Ditto)\nLanguage: {lang}\nNature: {nature}");
        var template = AutoLegalityWrapper.GetTemplate(set);
        var sav = AutoLegalityWrapper.GetTrainerInfo<T>();
        var pkm = sav.GetLegal(template, out var result);

        if (pkm == null)
        {
            await ReplyAsync("Set took too long to legalize.");
            return;
        }

        TradeExtensions<T>.DittoTrade((T)pkm);
        var la = new LegalityAnalysis(pkm);

        if (pkm is not T pk || !la.Valid)
        {
            var reason = result == "Timeout" ? "That set took too long to generate." : "I wasn't able to create something from that.";
            var imsg = $"Oops! {reason} Here's my best attempt for that Ditto!";
            await Context.Channel.SendPKMAsync(pkm, imsg).ConfigureAwait(false);
            return;
        }

        // Heal without setting markings
        pk.HealPP();
        pk.SetSuggestedHyperTrainingData();

        // Ad Name Check
        if (Info.Hub.Config.Trade.TradeConfiguration.EnableSpamCheck)
        {
            if (TradeExtensions<T>.HasAdName(pk, out string ad))
            {
                await Helpers<T>.ReplyAndDeleteAsync(Context, "Detected Adname in the Pokémon's name or trainer name, which is not allowed.", 5);
                return;
            }
        }

        var sig = Context.User.GetFavor();
        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, pk,
            PokeRoutineType.LinkTrade, PokeTradeType.Specific).ConfigureAwait(false);

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    [Command("itemTrade")]
    [Alias("it", "item")]
    [Summary("Makes the bot trade you a Pokémon holding the requested item.")]
    public async Task ItemTrade([Remainder] string item)
    {
        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "You already have an existing trade in the queue that cannot be cleared. Please wait until it is processed.", 2);
            return;
        }

        var code = Info.GetRandomTradeCode(userID);
        await ProcessItemTradeAsync(code, item);
    }

    [Command("itemTrade")]
    [Alias("it", "item")]
    [Summary("Makes the bot trade you a Pokémon holding the requested item.")]
    public async Task ItemTrade([Summary("Trade Code")] int code, [Remainder] string item)
    {
        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "You already have an existing trade in the queue that cannot be cleared. Please wait until it is processed.", 2);
            return;
        }

        await ProcessItemTradeAsync(code, item);
    }

    private async Task ProcessItemTradeAsync(int code, string item)
    {
        Species species = Info.Hub.Config.Trade.TradeConfiguration.ItemTradeSpecies == Species.None
            ? Species.Diglett
            : Info.Hub.Config.Trade.TradeConfiguration.ItemTradeSpecies;

        var set = new ShowdownSet($"{SpeciesName.GetSpeciesNameGeneration((ushort)species, 2, 8)} @ {item.Trim()}");
        var template = AutoLegalityWrapper.GetTemplate(set);
        var sav = AutoLegalityWrapper.GetTrainerInfo<T>();
        var pkm = sav.GetLegal(template, out var result);

        if (pkm == null)
        {
            await ReplyAsync("Set took too long to legalize.");
            return;
        }

        pkm = EntityConverter.ConvertToType(pkm, typeof(T), out _) ?? pkm;

        if (pkm.HeldItem == 0)
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context, $"{Context.User.Username}, the item you entered wasn't recognized.", 2);
            return;
        }

        var la = new LegalityAnalysis(pkm);
        if (pkm is not T pk || !la.Valid)
        {
            var reason = result == "Timeout" ? "That set took too long to generate." : "I wasn't able to create something from that.";
            var imsg = $"Oops! {reason} Here's my best attempt for that {species}!";
            await Context.Channel.SendPKMAsync(pkm, imsg).ConfigureAwait(false);
            return;
        }

        // Heal without setting markings
        pk.HealPP();
        pk.SetSuggestedHyperTrainingData();
        var sig = Context.User.GetFavor();
        await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, pk,
            PokeRoutineType.LinkTrade, PokeTradeType.Specific).ConfigureAwait(false);

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    #endregion

    #region List Commands

    [Command("tradeList")]
    [Alias("tl")]
    [Summary("Prints the users in the trade queues.")]
    [RequireSudo]
    public async Task GetTradeListAsync()
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        string msg = Info.GetTradeList(PokeRoutineType.LinkTrade);
        var embed = new EmbedBuilder();
        embed.AddField(x =>
        {
            x.Name = "Pending Trades";
            x.Value = msg;
            x.IsInline = false;
        });
        await ReplyAsync(LocalizationHelper.GetString("Trade_QueueList"), embed: embed.Build()).ConfigureAwait(false);
    }

    [Command("fixOTList")]
    [Alias("fl", "fq")]
    [Summary("Prints the users in the FixOT queue.")]
    [RequireSudo]
    public async Task GetFixListAsync()
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        string msg = Info.GetTradeList(PokeRoutineType.FixOT);
        var embed = new EmbedBuilder();
        embed.AddField(x =>
        {
            x.Name = "Pending Trades";
            x.Value = msg;
            x.IsInline = false;
        });
        await ReplyAsync(LocalizationHelper.GetString("Trade_QueueList"), embed: embed.Build()).ConfigureAwait(false);
    }

    [Command("listevents")]
    [Alias("le")]
    [Summary("Lists available event files, filtered by a specific letter or substring, and sends the list via DM.")]
    public Task ListEventsAsync([Remainder] string args = "")
        => ListHelpers<T>.HandleListCommandAsync(
            Context,
            SysCord<T>.Runner.Config.Trade.RequestFolderSettings.EventsFolder,
            "events",
            "er",
            args
        );

    [Command("battlereadylist")]
    [Alias("brl")]
    [Summary("Lists available battle-ready files, filtered by a specific letter or substring, and sends the list via DM.")]
    public Task BattleReadyListAsync([Remainder] string args = "")
        => ListHelpers<T>.HandleListCommandAsync(
            Context,
            SysCord<T>.Runner.Config.Trade.RequestFolderSettings.BattleReadyPKMFolder,
            "battle-ready files",
            "brr",
            args
        );

    #endregion

    #region Request Commands

    [Command("eventrequest")]
    [Alias("er")]
    [Summary("Downloads event attachments from the specified EventsFolder and adds to trade queue.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task EventRequestAsync(int index)
        => ListHelpers<T>.HandleRequestCommandAsync(
            Context,
            SysCord<T>.Runner.Config.Trade.RequestFolderSettings.EventsFolder,
            index,
            "event",
            "le"
        );

    [Command("battlereadyrequest")]
    [Alias("brr", "br")]
    [Summary("Downloads battle-ready attachments from the specified folder and adds to trade queue.")]
    [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
    public Task BattleReadyRequestAsync(int index)
        => ListHelpers<T>.HandleRequestCommandAsync(
            Context,
            SysCord<T>.Runner.Config.Trade.RequestFolderSettings.BattleReadyPKMFolder,
            index,
            "battle-ready file",
            "brl"
        );

    #endregion

    #region Batch Trades

    [Command("batchTrade")]
    [Alias("bt")]
    [Summary("Makes the bot trade multiple Pokémon from the provided list, up to a maximum of 4 trades.")]
    [RequireQueueRole(nameof(DiscordManager.RolesBatchTrade))]
    public async Task BatchTradeAsync([Summary("List of Showdown Sets separated by '---'")][Remainder] string content)
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        var tradeConfig = SysCord<T>.Runner.Config.Trade.TradeConfiguration;

        // Check if batch trades are allowed
        if (!tradeConfig.AllowBatchTrades)
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                "Batch trades are currently disabled by the bot administrator.", 2);
            return;
        }

        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                LocalizationHelper.GetString("Trade_AlreadyInQueue"), 2);
            return;
        }

        // Check if user has permission to use AutoOT
        bool hasAutoOTPermission = true;
        if (SysCordSettings.Manager != null)
        {
            if (Context.User is SocketGuildUser gUser)
            {
                var roles = gUser.Roles.Select(z => z.Name);
                hasAutoOTPermission = SysCordSettings.Manager.GetHasRoleAccess(nameof(DiscordManager.RolesAutoOT), roles);
            }
        }

        content = ReusableActions.StripCodeBlock(content);
        var trades = BatchHelpers<T>.ParseBatchTradeContent(content);

        // Use configured max trades per batch, default to 4 if less than 1
        int maxTradesAllowed = tradeConfig.MaxPkmsPerTrade > 0 ? tradeConfig.MaxPkmsPerTrade : 4;

        if (trades.Count > maxTradesAllowed)
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                $"You can only process up to {maxTradesAllowed} trades at a time. Please reduce the number of trades in your batch.", 5);
            return;
        }

        var processingMessage = await Context.Channel.SendMessageAsync(LocalizationHelper.GetString("Trade_BatchProcessing", Context.User.Mention, trades.Count));

        _ = Task.Run(async () =>
        {
            try
            {
                var batchPokemonList = new List<T>();
                var errors = new List<BatchTradeError>();
                for (int i = 0; i < trades.Count; i++)
                {
                    var userRoles = Context.User is SocketGuildUser batchGUser ? batchGUser.Roles.Select(r => r.Name) : null;
                    var (pk, error, set, legalizationHint) = await BatchHelpers<T>.ProcessSingleTradeForBatch(trades[i], userRoles);
                    if (pk != null)
                    {
                        // If user doesn't have AutoOT permission, force ignoreAutoOT for all Pokémon in batch
                        if (!hasAutoOTPermission)
                        {
                            // This will be handled by the trade bot when processing the batch
                            // We can't modify the Pokémon here, but the trade bot will check CanUseAutoOT
                        }
                        batchPokemonList.Add(pk);
                    }
                    else
                    {
                        var speciesName = set != null && set.Species > 0
                            ? GameInfo.Strings.Species[set.Species]
                            : "Unknown";
                        errors.Add(new BatchTradeError
                        {
                            TradeNumber = i + 1,
                            SpeciesName = speciesName,
                            ErrorMessage = error ?? "Unknown error",
                            LegalizationHint = legalizationHint,
                            ShowdownSet = set != null ? string.Join("\n", set.GetSetLines()) : trades[i]
                        });
                    }
                }

                await processingMessage.DeleteAsync();

                if (errors.Count > 0)
                {
                    await BatchHelpers<T>.SendBatchErrorEmbedAsync(Context, errors, trades.Count);
                    return;
                }
                if (batchPokemonList.Count > 0)
                {
                    var batchTradeCode = Info.GetRandomTradeCode(userID);
                    await BatchHelpers<T>.ProcessBatchContainer(Context, batchPokemonList, batchTradeCode, trades.Count);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await processingMessage.DeleteAsync();
                }
                catch { }

                await Context.Channel.SendMessageAsync(LocalizationHelper.GetString("Trade_BatchError", Context.User.Mention));
                Base.LogUtil.LogError($"Batch trade processing error: {ex.Message}", nameof(BatchTradeAsync));
            }
        });

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, 2);
    }

    #endregion

    #region Private Helper Methods

    private async Task ProcessTradeAsync(int code, string content, bool isHiddenTrade = false)
    {
        LocalizationHelper.DetectAndSetCulture(Context.Message.Content);

        var userID = Context.User.Id;
        if (!await Helpers<T>.EnsureUserNotInQueueAsync(userID))
        {
            await Helpers<T>.ReplyAndDeleteAsync(Context,
                LocalizationHelper.GetString("Trade_AlreadyInQueue"), 2);
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                // Translate German Showdown sets to English
                if (GermanShowdownTranslator.IsGermanShowdown(content))
                {
                    content = GermanShowdownTranslator.TranslateToEnglish(content);
                }

                var userRoles = Context.User is SocketGuildUser tradeGUser ? tradeGUser.Roles.Select(r => r.Name) : null;
                var result = await Helpers<T>.ProcessShowdownSetAsync(content, userRoles: userRoles);

                if (result.Pokemon == null)
                {
                    await Helpers<T>.SendTradeErrorEmbedAsync(Context, result);
                    return;
                }

                var sig = Context.User.GetFavor();

                // Check if user has permission to use AutoOT
                bool ignoreAutoOT = false;
                if (SysCordSettings.Manager != null)
                {
                    if (Context.User is SocketGuildUser gUser)
                    {
                        var roles = gUser.Roles.Select(z => z.Name);
                        if (!SysCordSettings.Manager.GetHasRoleAccess(nameof(DiscordManager.RolesAutoOT), roles))
                        {
                            // User doesn't have AutoOT permission, force ignoreAutoOT to true
                            ignoreAutoOT = true;
                        }
                    }
                }

                await Helpers<T>.AddTradeToQueueAsync(
                    Context, code, Context.User.Username, result.Pokemon, sig, Context.User,
                    isHiddenTrade: isHiddenTrade,
                    lgcode: result.LgCode,
                    ignoreAutoOT: ignoreAutoOT,
                    isNonNative: result.IsNonNative
                );
            }
            catch (Exception ex)
            {
                LogUtil.LogSafe(ex, nameof(TradeModule<T>));
                var msg = "Oops! An unexpected problem happened with this Showdown Set.";
                await Helpers<T>.ReplyAndDeleteAsync(Context, msg, 2);
            }
        });

        if (Context.Message is IUserMessage userMessage)
            _ = Helpers<T>.DeleteMessagesAfterDelayAsync(userMessage, null, isHiddenTrade ? 0 : 2);
    }

    private async Task ProcessTradeAttachmentAsync(int code, RequestSignificance sig, SocketUser user, bool isHiddenTrade = false, bool ignoreAutoOT = false)
    {
        var pk = await Helpers<T>.ProcessTradeAttachmentAsync(Context);
        if (pk == null)
            return;

        // Check if user has permission to use AutoOT and override trainer data
        if (SysCordSettings.Manager != null && Context.User is SocketGuildUser gUser)
        {
            var roles = gUser.Roles.Select(z => z.Name);

            // Check AutoOT permission
            if (!ignoreAutoOT && !SysCordSettings.Manager.GetHasRoleAccess(nameof(DiscordManager.RolesAutoOT), roles))
            {
                // User doesn't have AutoOT permission, force ignoreAutoOT to true
                ignoreAutoOT = true;
            }

            // Check Trainer Data Override permission
            if (!SysCordSettings.Manager.GetHasRoleAccess(nameof(DiscordManager.RolesTrainerDataOverride), roles))
            {
                // User doesn't have permission to use custom trainer data
                // Legalize the Pokemon to set bot's default OT/TID/SID
                try
                {
                    var legalized = pk.LegalizePokemon();
                    if (legalized != null && new LegalityAnalysis(legalized).Valid)
                    {
                        pk = (T)legalized;
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogSafe(ex, nameof(TradeModule<T>));
                    // If legalization fails, continue with original Pokemon
                    // The bot will handle it during trade processing
                }
            }
        }

        await Helpers<T>.AddTradeToQueueAsync(Context, code, user.Username, pk, sig, user,
            isHiddenTrade: isHiddenTrade, ignoreAutoOT: ignoreAutoOT);
    }

    #endregion
}
