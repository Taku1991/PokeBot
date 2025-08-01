using PKHeX.Core;
using PKHeX.Core.AutoMod;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SysBot.Pokemon;

public static class AutoLegalityWrapper
{
    private static bool Initialized;
    private static TradeSettings? TradeConfig;

    public static void EnsureInitialized(LegalitySettings cfg)
    {
        if (Initialized)
            return;
        Initialized = true;
        InitializeAutoLegality(cfg);
    }

    public static void SetTradeSettings(TradeSettings settings)
    {
        TradeConfig = settings;
    }

    private static void InitializeAutoLegality(LegalitySettings cfg)
    {
        InitializeCoreStrings();
        EncounterEvent.RefreshMGDB([cfg.MGDBPath]);
        InitializeTrainerDatabase(cfg);
        InitializeSettings(cfg);
    }

    // The list of encounter types in the priority we prefer if no order is specified.
    private static readonly EncounterTypeGroup[] EncounterPriority = [EncounterTypeGroup.Egg, EncounterTypeGroup.Slot, EncounterTypeGroup.Static, EncounterTypeGroup.Mystery, EncounterTypeGroup.Trade];

    private static void InitializeSettings(LegalitySettings cfg)
    {
        APILegality.SetAllLegalRibbons = cfg.SetAllLegalRibbons;
        APILegality.SetMatchingBalls = cfg.SetMatchingBalls;
        APILegality.ForceSpecifiedBall = cfg.ForceSpecifiedBall;
        APILegality.ForceLevel100for50 = cfg.ForceLevel100for50;
        Legalizer.EnableEasterEggs = cfg.EnableEasterEggs;
        APILegality.AllowTrainerOverride = cfg.AllowTrainerDataOverride;
        APILegality.AllowBatchCommands = cfg.AllowBatchCommands;
        APILegality.PrioritizeGame = cfg.PrioritizeGame;
        // Prevent missing entries in case of deletion
        GameVersion[] validVersions = [.. Enum.GetValues<GameVersion>().Where(ver => ver <= (GameVersion)51 && ver > GameVersion.Any)];
        foreach (var ver in validVersions)
        {
            if (!cfg.PriorityOrder.Contains(ver))
                cfg.PriorityOrder.Add(ver);
        }
        APILegality.PriorityOrder = cfg.PriorityOrder;
        APILegality.SetBattleVersion = cfg.SetBattleVersion;
        APILegality.Timeout = cfg.Timeout;
        var settings = ParseSettings.Settings;
        settings.Handler.CheckActiveHandler = false;
        var validRestriction = new NicknameRestriction { NicknamedTrade = Severity.Fishy, NicknamedMysteryGift = Severity.Fishy };
        settings.Nickname.SetAllTo(validRestriction);

        // As of February 2024, the default setting in PKHeX is Invalid for missing HOME trackers.
        // If the host wants to allow missing HOME trackers, we need to disable the default setting.
        bool allowMissingHOME = !cfg.EnableHOMETrackerCheck;
        if (allowMissingHOME)
            settings.HOMETransfer.Disable();

        // We need all the encounter types present, so add the missing ones at the end.
        var missing = EncounterPriority.Except(cfg.PrioritizeEncounters);
        cfg.PrioritizeEncounters.AddRange(missing);
        cfg.PrioritizeEncounters = [.. cfg.PrioritizeEncounters.Distinct()]; // Don't allow duplicates.
        EncounterMovesetGenerator.PriorityList = cfg.PrioritizeEncounters;
    }

    private static void InitializeTrainerDatabase(LegalitySettings cfg)
    {
        var externalSource = cfg.GeneratePathTrainerInfo;
        if (Directory.Exists(externalSource))
            TrainerSettings.LoadTrainerDatabaseFromPath(externalSource);

        // Seed the Trainer Database with enough fake save files so that we return a generation sensitive format when needed.
        var fallback = GetDefaultTrainer(cfg);
        for (byte generation = 1; generation <= GameUtil.GetGeneration(GameVersion.Gen9); generation++)
        {
            var versions = GameUtil.GetVersionsInGeneration(generation, GameVersion.Any);
            foreach (var version in versions)
                RegisterIfNoneExist(fallback, generation, version);
        }
        // Manually register for LGP/E since Gen7 above will only register the 3DS versions.
        RegisterIfNoneExist(fallback, 7, GameVersion.GP);
        RegisterIfNoneExist(fallback, 7, GameVersion.GE);
    }

    private static SimpleTrainerInfo GetDefaultTrainer(LegalitySettings cfg)
    {
        var OT = cfg.GenerateOT;
        if (OT.Length == 0)
            OT = "Blank"; // Will fail if actually left blank.
        var fallback = new SimpleTrainerInfo(GameVersion.Any)
        {
            Language = (byte)cfg.GenerateLanguage,
            TID16 = cfg.GenerateTID16,
            SID16 = cfg.GenerateSID16,
            OT = OT,
            Generation = 0,
        };
        return fallback;
    }

    private static void RegisterIfNoneExist(SimpleTrainerInfo fallback, byte generation, GameVersion version)
    {
        fallback = new SimpleTrainerInfo(version)
        {
            Language = fallback.Language,
            TID16 = fallback.TID16,
            SID16 = fallback.SID16,
            OT = fallback.OT,
            Generation = generation,
        };
        var exist = TrainerSettings.GetSavedTrainerData(version, generation, fallback);
        if (exist is SimpleTrainerInfo) // not anything from files; this assumes ALM returns SimpleTrainerInfo for non-user-provided fake templates.
            TrainerSettings.Register(fallback);
    }
    private static void InitializeCoreStrings()
    {
        var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName[..2];
        LocalizationUtil.SetLocalization(typeof(LegalityCheckStrings), lang);
        LocalizationUtil.SetLocalization(typeof(MessageStrings), lang);
        RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
        ParseSettings.ChangeLocalizationStrings(GameInfo.Strings.movelist, GameInfo.Strings.specieslist);
    }

    public static bool CanBeTraded(this PKM pkm)
    {
        if (TradeConfig?.TradeConfiguration.EnableSpamCheck ?? false)
        {
            if (pkm.IsNicknamed && StringsUtil.IsSpammyString(pkm.Nickname))
                return false;
            if (StringsUtil.IsSpammyString(pkm.OriginalTrainerName) && !IsFixedOT(new LegalityAnalysis(pkm).EncounterOriginal, pkm))
                return false;
        }

        return !FormInfo.IsFusedForm(pkm.Species, pkm.Form, pkm.Format);
    }

    public static bool IsFixedOT(IEncounterTemplate t, PKM pkm) => t switch
    {
        IFixedTrainer { IsFixedTrainer: true } => true,
        MysteryGift g => !g.IsEgg && g switch
        {
            WC9 wc9 => wc9.GetHasOT(pkm.Language),
            WA8 wa8 => wa8.GetHasOT(pkm.Language),
            WB8 wb8 => wb8.GetHasOT(pkm.Language),
            WC8 wc8 => wc8.GetHasOT(pkm.Language),
            WB7 wb7 => wb7.GetHasOT(pkm.Language),
            { Generation: >= 5 } gift => gift.OriginalTrainerName.Length > 0,
            _ => true,
        },
        _ => false,
    };

    public static ITrainerInfo GetTrainerInfo<T>() where T : PKM, new()
    {
        if (typeof(T) == typeof(PK8))
            return TrainerSettings.GetSavedTrainerData(GameVersion.SWSH, 8);
        if (typeof(T) == typeof(PB8))
            return TrainerSettings.GetSavedTrainerData(GameVersion.BDSP, 8);
        if (typeof(T) == typeof(PA8))
            return TrainerSettings.GetSavedTrainerData(GameVersion.PLA, 8);
        if (typeof(T) == typeof(PK9))
            return TrainerSettings.GetSavedTrainerData(GameVersion.SV, 9);
        if (typeof(T) == typeof(PB7))
            return TrainerSettings.GetSavedTrainerData(GameVersion.GE, 7);

        throw new ArgumentException("Type does not have a recognized trainer fetch.", typeof(T).Name);
    }

    public static ITrainerInfo GetTrainerInfo(byte gen) => TrainerSettings.GetSavedTrainerData(gen);

    public static PKM GetLegal(this ITrainerInfo sav, IBattleTemplate set, out string res)
    {
        var result = sav.GetLegalFromSet(set);
        res = result.Status switch
        {
            LegalizationResult.Regenerated => "Regenerated",
            LegalizationResult.Failed => "Failed",
            LegalizationResult.Timeout => "Timeout",
            LegalizationResult.VersionMismatch => "VersionMismatch",
            _ => "",
        };
        return result.Created;
    }

    public static string GetLegalizationHint(IBattleTemplate set, ITrainerInfo sav, PKM pk) => set.SetAnalysis(sav, pk);
    public static PKM LegalizePokemon(this PKM pk) => pk.Legalize();
    public static IBattleTemplate GetTemplate(ShowdownSet set) => new RegenTemplate(set);
}
