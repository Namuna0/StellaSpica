using Discord;
using Discord.WebSocket;
using MathNet.Numerics.Random;
using ScottPlot;
using ScottPlot.Palettes;

partial class Program
{
    class Stock
    {
        private Timer _midnightChecker;
        private bool _alreadySent = false;

        private LinkedList<int> _stock = new LinkedList<int>();
        private LinkedList<int> _price = new LinkedList<int>();

        private enum Economy
        {
            GreatDepression,
            Depression,
            Recession,
            Normal,
            Booming,
            Bubble
        }
        private Economy _economy = Economy.Recession;

        private double[] _xs = { 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

        private int _recount = 0;
        private MersenneTwister _ms;

        public Stock(MersenneTwister ms)
        {
            _ms = ms;

            _stock.AddLast(37);
            _stock.AddLast(50);
            _stock.AddLast(10);
            _price.AddLast((int)(Economy.Recession - 2) * 25);
            _price.AddLast((int)(Economy.Depression - 2) * 25);
        }

        public async Task StartFantasiaEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
        {
            _midnightChecker = new Timer(async _ =>
            {
                var now = DateTime.Now;
                if (now.Hour == 15 && now.Minute == 0 && !_alreadySent)
                {
                    await ShowEconomy(message, guild, user, true);

                    _alreadySent = true;
                }
                if (now.Minute != 0) _alreadySent = false;

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            await Task.CompletedTask;
        }
        public async Task StartNocturneEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
        {
            _midnightChecker = new Timer(async _ =>
            {
                var now = DateTime.Now;
                if (now.Hour == 15 && now.Minute == 0 && !_alreadySent)
                {
                    await ShowEconomy(message, guild, user, false);

                    _alreadySent = true;
                }
                if (now.Minute != 0) _alreadySent = false;

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            await Task.CompletedTask;
        }

        public async Task ShowEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user, bool isFantasia)
        {
            int e = 0;
            string day = "";
            string dice = "";
            string price = "";

            if (_economy == Economy.Normal)
            {
                e = _ms.Next(1, 101) + 20;
                dice = $"1d100({e - 20})+20 => {e}";
                day = "通常";
                if (e < 70) price = "±0%";
                else price = "±10%";
            }
            else if (_economy == Economy.Recession)
            {
                e = _ms.Next(1, 101) - 30;
                dice = $"1d100({e + 30})-30 => {e}";
                day = "不況";
                price = "-10%";
            }
            else if (_economy == Economy.Depression)
            {
                e = _ms.Next(1, 101) - 40;
                dice = $"1d100({e + 40})-40 => {e}";
                day = $"恐慌({3 - _recount}日目)";
                price = "-20%";
            }
            else if (_economy == Economy.GreatDepression)
            {
                e = _ms.Next(1, 101) - 50;
                dice = $"1d100({e + 50})-50 => {e}";
                day = $"大恐慌({3 - _recount}日目)";
                price = "-30%";
            }
            else if (_economy == Economy.Booming)
            {
                e = _ms.Next(1, 101) + 30;
                dice = $"1d100({e - 30})+30 => {e}";
                day = $"好景気({3 - _recount}日目)";
                price = "+30%";
            }
            else if (_economy == Economy.Bubble)
            {
                e = _ms.Next(1, 101) + 50;
                dice = $"1d100({e - 50})+50 => {e}";
                day = $"バブル💃({3 - _recount}日目)";
                price = "+50%";
            }

            if (_recount <= 0)
            {
                if (_economy == Economy.Normal)
                {
                    if (e <= 40)
                    {
                        _economy = Economy.Recession;
                        _recount = 0;
                    }
                    else if (e >= 100)
                    {
                        _economy = Economy.Booming;
                        _recount = 2;
                    }
                    else
                    {
                        _economy = Economy.Normal;
                        _recount = 0;
                    }
                }
                else if (_economy == Economy.Recession)
                {
                    if (e <= 30)
                    {
                        _economy = Economy.Depression;
                        _recount = 2;
                    }
                    else if (e >= 60)
                    {
                        _economy = Economy.Normal;
                        _recount = 0;
                    }
                    else
                    {
                        _economy = Economy.Recession;
                        _recount = 0;
                    }
                }
                else if (_economy == Economy.Depression)
                {
                    if (e <= 20)
                    {
                        _economy = Economy.GreatDepression;
                        _recount = 2;
                    }
                    else if (e >= 35)
                    {
                        _economy = Economy.Normal;
                        _recount = 0;
                    }
                    else
                    {
                        _economy = Economy.Depression;
                        _recount = 2;
                    }
                }
                else if (_economy == Economy.GreatDepression)
                {
                    if (e >= 30)
                    {
                        _economy = Economy.Normal;
                        _recount = 0;
                    }
                    else
                    {
                        _economy = Economy.GreatDepression;
                        _recount = 2;
                    }
                }
                else if (_economy == Economy.Booming)
                {
                    if (e <= 70)
                    {
                        _economy = Economy.Depression;
                        _recount = 2;
                    }
                    else if (e >= 120)
                    {
                        _economy = Economy.Bubble;
                        _recount = 2;
                    }
                    else
                    {
                        _economy = Economy.Normal;
                        _recount = 0;
                    }
                }
                else if (_economy == Economy.Bubble)
                {
                    _economy = Economy.Normal;
                    _recount = 0;
                }
            }
            else
            {
                _recount--;
            }

            string day2 = "";

            if (_economy == Economy.Normal)
            {
                day2 = "通常";
            }
            else if (_economy == Economy.Recession)
            {
                day2 = "不況";
            }
            else if (_economy == Economy.Depression)
            {
                day2 = $"恐慌({3 - _recount}日目)";
            }
            else if (_economy == Economy.GreatDepression)
            {
                day2 = $"大恐慌({3 - _recount}日目)";
            }
            else if (_economy == Economy.Booming)
            {
                day2 = $"好景気({3 - _recount}日目)";
            }
            else if (_economy == Economy.Bubble)
            {
                day2 = $"バブル💃({3 - _recount}日目)";
            }

            _stock.AddLast(e);
            _price.AddLast((int)(_economy - 2) * 25);

            if (_stock.Count > 25)
            {
                _stock.RemoveFirst();
                _price.RemoveFirst();
            }

            if (isFantasia)
            {
                await ShowFantasiaGraph(message, dice, day, day2, e, price);
            }
            else
            {
                await ShowNocturneGraph(message, dice, day, day2, e, price);
            }
        }

        private async Task ShowFantasiaGraph(SocketMessage message, string dice, string day, string day2, int e, string price)
        {
            var plt = new Plot();

            plt.Font.Set("Noto Sans CJK JP");

            var s1 = plt.Add.Scatter(_xs, _stock.ToArray());
            s1.FillY = true;

            s1.LegendText = "株価";
            var s2 = plt.Add.Scatter(_xs, _price.ToArray());
            s2.LegendText = "経済";

            plt.Title("ファンタジア・アスガリア証券取引所");
            plt.Axes.Title.Label.ForeColor = Colors.DarkRed;
            plt.Axes.Title.Label.FontSize = 32;
            plt.Axes.SetLimits(24, 0, -75, 150);

            // レジェンド表示を有効化
            plt.ShowLegend();

            plt.SavePng("ncse_sample.png", 800, 400);

            await message.Channel.SendMessageAsync("@here");
            await message.Channel.SendFileAsync("ncse_sample.png");

            var embed = new EmbedBuilder()
                .WithTitle("Asgaria Stock Exchange - Report\r\n")
            .WithDescription("アスガリア証券取引所:")
                .WithColor(Discord.Color.DarkGrey)
                .AddField(dice, "```――――――――――📊経済情報――――――――――```", inline: false)
                .AddField("```経済状況:```", day, inline: true)
                .AddField("```次回の経済情勢:```", day2, inline: true)
                .AddField("\u200B", "```―――――――🪙株価・物価情報―――――――```", inline: false)
                .AddField("現在株価:", $"{e}G", inline: true)
                .AddField("物価変動:", "+30%", inline: true)
                .Build();

            await message.Channel.SendMessageAsync(embed: embed);
        }

        private async Task ShowNocturneGraph(SocketMessage message, string dice, string day, string day2, int e, string price)
        {
            var plt = new Plot();

            plt.Font.Set("Noto Sans CJK JP");

            var s1 = plt.Add.Scatter(_xs, _stock.ToArray());
            s1.FillY = true;

            s1.LegendText = "株価";
            var s2 = plt.Add.Scatter(_xs, _price.ToArray());
            s2.LegendText = "経済";

            plt.Title("F/N経済");
            plt.Axes.Title.Label.ForeColor = Colors.DarkRed;
            plt.Axes.Title.Label.FontSize = 32;
            plt.Axes.SetLimits(24, 0, -75, 150);

            // レジェンド表示を有効化
            plt.ShowLegend();

            plt.SavePng("ncse_sample2.png", 800, 400);

            await message.Channel.SendMessageAsync("@here");
            await message.Channel.SendFileAsync("ncse_sample2.png");

            var embed = new EmbedBuilder()
                .WithTitle("今日の経済")
                .WithDescription("F/N経済:")
                .WithColor(Discord.Color.DarkGrey)
                .AddField(dice, "```――――――――――📊経済情報――――――――――```", inline: false)
                .AddField("```経済状況:```", day, inline: true)
                .AddField("```次回の経済情勢:```", day2, inline: true)
                .AddField("\u200B", "```―――――――🪙株価・物価情報―――――――```", inline: false)
                .AddField("```現在株価:```", $"{e}G", inline: true)
                .AddField("```物価変動:```", price, inline: true)
                .Build();

            await message.Channel.SendMessageAsync(embed: embed);
        }
    }

    private Stock _nStock;
    private Stock _fStock;

    private async Task StartFantasiaEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        _nStock = new Stock(_ms);

        await _nStock.StartFantasiaEconomy(message, guild, user);
    }
    private async Task StartNocturneEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        _fStock = new Stock(_ms);

        await _fStock.StartNocturneEconomy(message, guild, user);
    }

    private async Task NextFantasiaEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        await _nStock.ShowEconomy(message, guild, user, true);
    }
    private async Task NextNocturneEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        await _fStock.ShowEconomy(message, guild, user, false);
    }

    private async Task A(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var now = DateTime.Now;

        await message.Channel.SendMessageAsync($"時間{now.Hour}");
        await message.Channel.SendMessageAsync($"分{now.Minute}");

    }
}
