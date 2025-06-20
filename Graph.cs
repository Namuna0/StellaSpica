using Discord;
using Discord.WebSocket;
using ScottPlot;

partial class Program
{

    private async Task G(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var plt = new Plot();

        double[] xs = { 0, 1, 2, 3, 4 };
        double[] ys1 = { 10, 20, 15, 25, 18 };
        double[] ys2 = { 12, 22, 13, 28, 15 };

        plt.Font.Set("Noto Sans CJK JP");

        var s1 = plt.Add.Scatter(xs, ys1);
        s1.Label = "株価";
        var s2 = plt.Add.Scatter(xs, ys2);
        s2.Label = "経済";

        plt.Title("ノクターン・NCSE総合指数");

        // レジェンド表示を有効化
        plt.ShowLegend();

        plt.SavePng("ncse_sample.png", 800, 400);

        await message.Channel.SendFileAsync("ncse_sample.png", "グラフ画像です！");

    }
}
