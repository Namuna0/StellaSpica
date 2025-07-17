using Discord;
using Discord.WebSocket;
using ScottPlot;

partial class Program
{
    private Timer _midnightChecker;
    private bool _alreadySent = false;

    private int _stockCount;
    private int _recount = 1;
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
    private Economy _economy = Economy.Booming;

    private double[] _xs = { 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

    public async Task StartEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        _stock.Clear();
        _price.Clear();

        await ConnectDatabase("SELECT * FROM stock;",
        onResponce: async (reader) =>
        {
            do
            {
                _stockCount = reader.GetInt32(reader.GetOrdinal("id"));
                var stock = reader.GetInt32(reader.GetOrdinal("stock"));
                var price = reader.GetInt32(reader.GetOrdinal("price"));
                _recount = reader.GetInt32(reader.GetOrdinal("recount"));

                _stock.AddLast(stock);
                _price.AddLast(price);

                if (_stock.Count > 25)
                {
                    _stock.RemoveFirst();
                    _price.RemoveFirst();
                }
            } while (reader.Read());

            await Task.CompletedTask;
        });

        _midnightChecker = new Timer(async _ =>
        {
            var now = DateTime.Now;
            if (now.Hour == 15 && now.Minute == 0 && !_alreadySent)
            {
                await ShowEconomy(message, guild, user, -1);

                _alreadySent = true;
            }
            if (now.Minute != 0) _alreadySent = false;

        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        await Task.CompletedTask;
    }

    public async Task ShowEconomy(SocketMessage message, SocketGuild guild, SocketGuildUser user, int diceFix)
    {
        int e = 0;
        string day = "";
        string dice = "";
        string price = "";

        if (_economy == Economy.Normal)
        {
            e = _ms.Next(1, 101) + 20;
            if (diceFix > 0) e = diceFix;

            dice = $"1d100({e - 20})+20 => {e}";
            day = "通常";
            if (e < 70) price = "±0%";
            else price = "±0%";
        }
        else if (_economy == Economy.Recession)
        {
            e = _ms.Next(1, 101) - 30;
            if (diceFix > 0) e = diceFix - 30;

            dice = $"1d100({e + 30})-30 => {e}";
            day = "不況";
            price = "-10%";
        }
        else if (_economy == Economy.Depression)
        {
            e = _ms.Next(1, 101) - 40;
            if (diceFix > 0) e = diceFix - 40;

            dice = $"1d100({e + 40})-40 => {e}";
            day = $"恐慌({_recount}日目)";
            price = "-20%";
        }
        else if (_economy == Economy.GreatDepression)
        {
            e = _ms.Next(1, 101) - 50;
            if (diceFix > 0) e = diceFix - 50;

            dice = $"1d100({e + 50})-50 => {e}";
            day = $"大恐慌({_recount}日目)";
            price = "-30%";
        }
        else if (_economy == Economy.Booming)
        {
            e = _ms.Next(1, 101) + 30;
            if (diceFix > 0) e = diceFix + 30;

            dice = $"1d100({e - 30})+30 => {e}";
            day = $"好景気({_recount}日目)";
            price = "+30%";
        }
        else if (_economy == Economy.Bubble)
        {
            e = _ms.Next(1, 101) + 50;
            if (diceFix > 0) e = diceFix + 50;

            dice = $"1d100({e - 50})+50 => {e}";
            day = $"バブル💃({_recount}日目)";
            price = "+50%";
        }

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
                _recount = 0;
            }
        }
        else if (_economy == Economy.Recession)
        {
            if (e <= 30)
            {
                _economy = Economy.Depression;
                _recount = 0;
            }
            else if (e >= 60)
            {
                _economy = Economy.Normal;
                _recount = 0;
            }
        }
        else if (_economy == Economy.Depression)
        {
            if (_recount >= 2)
            {
                if (e <= 20)
                {
                    _economy = Economy.GreatDepression;
                    _recount = 0;
                }
                else if (e >= 35)
                {
                    _economy = Economy.Normal;
                    _recount = 0;
                }
            }
        }
        else if (_economy == Economy.GreatDepression)
        {
            if (_recount >= 2 && e >= 30)
            {
                _economy = Economy.Normal;
                _recount = 0;
            }
        }
        else if (_economy == Economy.Booming)
        {
            if (e >= 120)
            {
                _economy = Economy.Bubble;
                _recount = 0;
            }
            else if (_recount >= 2)
            {
                _economy = Economy.Normal;
                _recount = 0;
            }
        }
        else if (_economy == Economy.Bubble)
        {
            if (e <= 70)
            {
                _economy = Economy.Depression;
                _recount = 0;
            }
            else if (_recount >= 2)
            {
                _economy = Economy.Normal;
                _recount = 0;
            }
        }

        _recount++;

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
            day2 = $"恐慌({_recount}日目)";
        }
        else if (_economy == Economy.GreatDepression)
        {
            day2 = $"大恐慌({_recount}日目)";
        }
        else if (_economy == Economy.Booming)
        {
            day2 = $"好景気({_recount}日目)";
        }
        else if (_economy == Economy.Bubble)
        {
            day2 = $"バブル💃({_recount}日目)";
        }

        _stock.AddLast(e);
        _price.AddLast((int)(_economy - 2) * 25);

        if (_stock.Count > 25)
        {
            _stock.RemoveFirst();
            _price.RemoveFirst();
        }

        _stockCount++;

        await ConnectDatabase($@"INSERT INTO stock (id, stock, price, recount) VALUES (@id, @stock, @price, @recount);",
            parameters =>
            {
                parameters.AddWithValue("id", _stockCount);
                parameters.AddWithValue($"stock", e);
                parameters.AddWithValue($"price", (int)(_economy - 2) * 25);
                parameters.AddWithValue($"recount", _recount);
            });

        await ShowGraph(message, dice, day, day2, e, price);
    }
    private async Task ShowGraph(SocketMessage message, string dice, string day, string day2, int e, string price)
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

    private async Task A(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var now = DateTime.Now;

        await message.Channel.SendMessageAsync($"時間{now.Hour}");
        await message.Channel.SendMessageAsync($"分{now.Minute}");
    }
}
