using Discord.WebSocket;
using SkiaSharp;

partial class Program
{
    private Timer _midnightChecker2;
    private bool _alreadySent2 = false;


    public async Task StartTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        _midnightChecker2 = new Timer(async _ =>
        {
            var now = DateTime.Now;
            if (now.Hour == 15 && now.Minute == 0 && !_alreadySent2)
            {
                await ShowTrade(message, guild, user);

                _alreadySent2 = true;
            }
            if (now.Minute != 0) _alreadySent2 = false;

        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        await Task.CompletedTask;
    }

    public async Task ShowTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        // 例: テーブルデータ
        string[][] data =
        {
            new string[] { "", "AIL", "AME", "LLL", "INA", "RUZ", "N. U" },
            new string[] { "衣類", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "酒類", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "機械", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "資源", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "武器", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "装飾", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "食料", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "書籍", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "絵画", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "宝石", _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString(), _ms.Next(1, 301).ToString() },
            new string[] { "ギャング", _ms.Next(1, 101) <= 30 ? "○" : "×", _ms.Next(1, 101) <= 30 ? "○" : "×", _ms.Next(1, 101) <= 30 ? "○" : "×", _ms.Next(1, 101) <= 30 ? "○" : "×", _ms.Next(1, 101) <= 30 ? "○" : "×", _ms.Next(1, 101) <= 30 ? "○" : "×" },
        };

        // 列ごとの幅（最大文字数×フォント幅、などで調整してもOK）
        int[] colWidths = { 120, 120, 120, 120, 120, 120, 120 };
        int rowHeight = 40;
        int width = colWidths.Sum() + (colWidths.Length + 1) * 2;
        int height = rowHeight * data.Length + (data.Length + 1) * 2;

        // SkiaSharpで画像生成
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        // フォントなど設定
        var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            TextSize = 24,
            Typeface = SKTypeface.FromFamilyName("Meiryo") // 日本語
        };

        // テーブルを描画
        int y = 2;
        for (int i = 0; i < data.Length; i++)
        {
            int x = 2;
            for (int j = 0; j < data[i].Length; j++)
            {
                // セル枠
                var rect = new SKRect(x, y, x + colWidths[j], y + rowHeight);
                canvas.DrawRect(rect, new SKPaint { Color = SKColors.LightGray, Style = SKPaintStyle.Stroke });

                // テキスト（中央揃え）
                var text = data[i][j];
                var bounds = new SKRect();
                paint.MeasureText(text, ref bounds);
                float textX = x + (colWidths[j] - bounds.Width) / 2;
                float textY = y + rowHeight / 2 + bounds.Height / 2;
                canvas.DrawText(text, textX, textY, paint);

                x += colWidths[j] + 2;
            }
            y += rowHeight + 2;
        }

        // 画像をファイルに保存（MemoryStreamでもOK）
        using var image = SKImage.FromBitmap(bitmap);
        using var dataStream = image.Encode(SKEncodedImageFormat.Png, 100);

        string filePath = "table.png";
        using (var fileStream = File.OpenWrite(filePath))
        {
            dataStream.SaveTo(fileStream);
        }

        await message.Channel.SendMessageAsync("ノクターン - 国際情勢");
        await message.Channel.SendFileAsync("table.png");
    }
}
