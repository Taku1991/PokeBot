using Discord;
using Discord.WebSocket;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using PKHeX.Drawing.PokeSprite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = Discord.Color;

namespace SysBot.Pokemon.Discord;

public class DiscordTradeNotifier<T> : IPokeTradeNotifier<T>
    where T : PKM, new()
{
    private T Data { get; }

    private PokeTradeTrainerInfo Info { get; }

    private int Code { get; }

    private List<Pictocodes> LGCode { get; }

    private SocketUser Trader { get; }

    private int BatchTradeNumber { get; }

    private int TotalBatchTrades { get; }

    private bool IsMysteryEgg { get; }

    public DiscordTradeNotifier(T data, PokeTradeTrainerInfo info, int code, SocketUser trader, int batchTradeNumber, int totalBatchTrades, bool isMysteryEgg, List<Pictocodes> lgcode)
    {
        Data = data;
        Info = info;
        Code = code;
        Trader = trader;
        BatchTradeNumber = batchTradeNumber;
        TotalBatchTrades = totalBatchTrades;
        IsMysteryEgg = isMysteryEgg;
        LGCode = lgcode;
    }

    public Action<PokeRoutineExecutor<T>>? OnFinish { private get; set; }

    public readonly PokeTradeHub<T> Hub = SysCord<T>.Runner.Hub;

    public void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        int language = 2;
        var speciesName = SpeciesName.GetSpeciesName(Data.Species, language);
        var batchInfo = TotalBatchTrades > 1 ? $" (Trade {BatchTradeNumber} of {TotalBatchTrades})" : "";
        var receive = (Data.Species == 0 ? string.Empty : $" ({Data.Nickname})");

        if (Data is PK9)
        {
            var message = $"Initializing trade{receive}{batchInfo}. Please be ready.";

            if (TotalBatchTrades > 1 && BatchTradeNumber == 1)
            {
                message += "\n**Please stay in the trade until all batch trades are completed.**";
            }

            EmbedHelper.SendTradeInitializingEmbedAsync(Trader, speciesName, Code, IsMysteryEgg, message).ConfigureAwait(false);
        }
        else if (Data is PB7)
        {
            var (thefile, lgcodeembed) = CreateLGLinkCodeSpriteEmbed(LGCode);
            Trader.SendFileAsync(thefile, $"Initializing trade{receive}. Please be ready. Your code is", embed: lgcodeembed).ConfigureAwait(false);
        }
        else
        {
            EmbedHelper.SendTradeInitializingEmbedAsync(Trader, speciesName, Code, IsMysteryEgg).ConfigureAwait(false);
        }
    }

    public void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
    {
        var batchInfo = TotalBatchTrades > 1 ? $" for batch trade (Trade {BatchTradeNumber} of {TotalBatchTrades})" : "";
        var name = Info.TrainerName;
        var trainer = string.IsNullOrEmpty(name) ? string.Empty : $" {name}";

        if (Data is PB7 && LGCode != null && LGCode.Count != 0)
        {
            var message = $"I'm waiting for you{trainer}{batchInfo}! My IGN is **{routine.InGameName}**.";
            Trader.SendMessageAsync(message).ConfigureAwait(false);
        }
        else
        {
            string? additionalMessage = null;
            if (TotalBatchTrades > 1 && BatchTradeNumber > 1)
            {
                var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
                additionalMessage = $"Now trading{receive} (Trade {BatchTradeNumber} of {TotalBatchTrades}). **Select the Pokémon you wish to trade!**";
            }

            EmbedHelper.SendTradeSearchingEmbedAsync(Trader, trainer, routine.InGameName, additionalMessage).ConfigureAwait(false);
        }
    }

    public void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
    {
        OnFinish?.Invoke(routine);
        EmbedHelper.SendTradeCanceledEmbedAsync(Trader, msg.ToString()).ConfigureAwait(false);
    }

    public void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
    {
        OnFinish?.Invoke(routine);
        var tradedToUser = Data.Species;

        // Create different messages based on whether this is a single trade or part of a batch
        string message;
        if (TotalBatchTrades > 1)
        {
            // This is part of a batch, but we still want to confirm each individual trade
            message = tradedToUser != 0
                ? $"Trade {BatchTradeNumber}/{TotalBatchTrades} completed! Pokémon sent."
                : $"Trade {BatchTradeNumber}/{TotalBatchTrades} completed!";
        }
        else
        {
            // Standard single trade message
            message = tradedToUser != 0 ? $"Trade finished. Enjoy!" : "Trade finished!";
        }

        Trader.SendMessageAsync(message).ConfigureAwait(false);

        // Always send back the received Pokémon if ReturnPKMs is enabled
        if (result is not null && Hub.Config.Discord.ReturnPKMs)
        {
            // Add more context for batch trades
            string fileMessage = TotalBatchTrades > 1
                ? $"Here's the Pokémon you traded me (Trade {BatchTradeNumber}/{TotalBatchTrades})!"
                : "Here's what you traded me!";

            Trader.SendPKMAsync(result, fileMessage).ConfigureAwait(false);
        }
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
    {
        EmbedHelper.SendNotificationEmbedAsync(Trader, message).ConfigureAwait(false);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
    {
        if (message.ExtraInfo is SeedSearchResult r)
        {
            SendNotificationZ3(r);
            return;
        }

        var msg = message.Summary;
        if (message.Details.Count > 0)
            msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
        Trader.SendMessageAsync(msg).ConfigureAwait(false);
    }

    public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
    {
        if (result.Species != 0 && (Hub.Config.Discord.ReturnPKMs || info.Type == PokeTradeType.Dump))
            Trader.SendPKMAsync(result, message).ConfigureAwait(false);
    }

    private void SendNotificationZ3(SeedSearchResult r)
    {
        var lines = r.ToString();

        var embed = new EmbedBuilder { Color = Color.LighterGrey };
        embed.AddField(x =>
        {
            x.Name = $"Seed: {r.Seed:X16}";
            x.Value = lines;
            x.IsInline = false;
        });
        var msg = $"Here are the details for `{r.Seed:X16}`:";
        Trader.SendMessageAsync(msg, embed: embed.Build()).ConfigureAwait(false);
    }

    public static (string, Embed) CreateLGLinkCodeSpriteEmbed(List<Pictocodes> lgcode)
    {
        int codecount = 0;
        List<System.Drawing.Image> spritearray = [];
        foreach (Pictocodes cd in lgcode)
        {
            var showdown = new ShowdownSet(cd.ToString());
            var sav = SaveUtil.GetBlankSAV(EntityContext.Gen7b, "pip");
            PKM pk = sav.GetLegalFromSet(showdown).Created;
            System.Drawing.Image png = pk.Sprite();
            var destRect = new Rectangle(-40, -65, 137, 130);
            var destImage = new Bitmap(137, 130);
            destImage.SetResolution(png.HorizontalResolution, png.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.DrawImage(png, destRect, 0, 0, png.Width, png.Height, GraphicsUnit.Pixel);
            }
            png = destImage;
            spritearray.Add(png);
            codecount++;
        }
        int outputImageWidth = spritearray[0].Width + 20;

        int outputImageHeight = spritearray[0].Height - 65;

        Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        using (Graphics graphics = Graphics.FromImage(outputImage))
        {
            graphics.DrawImage(spritearray[0], new Rectangle(0, 0, spritearray[0].Width, spritearray[0].Height),
                new Rectangle(new Point(), spritearray[0].Size), GraphicsUnit.Pixel);
            graphics.DrawImage(spritearray[1], new Rectangle(50, 0, spritearray[1].Width, spritearray[1].Height),
                new Rectangle(new Point(), spritearray[1].Size), GraphicsUnit.Pixel);
            graphics.DrawImage(spritearray[2], new Rectangle(100, 0, spritearray[2].Width, spritearray[2].Height),
                new Rectangle(new Point(), spritearray[2].Size), GraphicsUnit.Pixel);
        }
        System.Drawing.Image finalembedpic = outputImage;
        var filename = $"{System.IO.Directory.GetCurrentDirectory()}//finalcode.png";
        finalembedpic.Save(filename);
        filename = System.IO.Path.GetFileName($"{System.IO.Directory.GetCurrentDirectory()}//finalcode.png");
        Embed returnembed = new EmbedBuilder().WithTitle($"{lgcode[0]}, {lgcode[1]}, {lgcode[2]}").WithImageUrl($"attachment://{filename}").Build();
        return (filename, returnembed);
    }
}
