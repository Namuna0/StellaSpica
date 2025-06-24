using Discord;
using Discord.WebSocket;
using MathNet.Numerics.Random;
using ScottPlot;

partial class Program
{
    class Trade
    {
        private Timer _midnightChecker;
        private bool _alreadySent = false;

        private MersenneTwister _ms;

        public Trade(MersenneTwister ms)
        {
            _ms = ms;
        }

        public async Task StartTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
        {
            _midnightChecker = new Timer(async _ =>
            {
                var now = DateTime.Now;
                if (now.Hour == 15 && now.Minute == 0 && !_alreadySent)
                {
                    await ShowTrade(message, guild, user);

                    _alreadySent = true;
                }
                if (now.Minute != 0) _alreadySent = false;

            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            await Task.CompletedTask;
        }

        public async Task ShowTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
        {
            await ShowTrade(message);
        }
        private async Task ShowTrade(SocketMessage message)
        {
            var plt = new Plot();
            plt.Font.Set("Noto Sans CJK JP");

            // テーブルデータ（例）
            string[,] table = {
    {"AIL", "AME", "L.L.L", "INA", "RUZ", "N.U"},
    {"60", "97", "147", "248", "173", "28"},
    {"72", "200", "9", "116", "210", ""},
    {"157", "228", "105", "45", "298", ""},
    {"180", "248", "208", "133", "227", ""}
};
            string[] rowHeaders = { ">衣類", ">酒類", ">機械", ">資源" };

            // レイアウト調整用
            int cols = table.GetLength(1);
            int rows = table.GetLength(0);
            float cellW = 80, cellH = 35;
            float left = 80, top = 80;

            // タイトル
            var title = plt.Add.Text("ノクターン - 国際市場：", left + cellW * (cols / 2), top - 40);
            title.Alignment = Alignment.UpperCenter;
            title.FontSize = 22;
            title.Bold = true;
            title.Color = Colors.Red;

            // 日付
            plt.Add.Text("2024/12/31", new Coordinates(left + cellW * cols, top - 35));

            // ヘッダー行
            for (int j = 0; j < cols; j++)
                plt.Add.Text(table[0, j], new Coordinates(left + cellW * j + cellW / 2, top));

            // 本体
            for (int i = 1; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    plt.Add.Text(table[i, j], new Coordinates(left + cellW * j + cellW / 2, top + cellH * i));

            // 行ラベル
            for (int i = 1; i < rows; i++)
                plt.Add.Text(rowHeaders[i - 1], new Coordinates(left - 40, top + cellH * i));

            // 枠線（横）
            for (int i = 0; i <= rows; i++)
                plt.Add.Line(new Coordinates(left, top + cellH * i), new Coordinates(left + cellW * cols, top + cellH * i));
            // 枠線（縦）
            for (int j = 0; j <= cols; j++)
                plt.Add.Line(new Coordinates(left + cellW * j, top), new Coordinates(left + cellW * j, top + cellH * rows));

            // 余白などの調整
            plt.Axes.SetLimits(0, left + cellW * cols + 40, top - 50, top + cellH * rows + 20);
            //plt.Frameless();

            // レジェンド表示を有効化
            plt.ShowLegend();

            plt.SavePng("trade.png", 600, 400);

            await message.Channel.SendFileAsync("trade.png");
        }
    }

    private Trade _trade;

    private async Task StartTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        _trade = new Trade(_ms);

        await _trade.StartTrade(message, guild, user);
    }

    private async Task NextTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        await _trade.ShowTrade(message, guild, user);
    }
}
