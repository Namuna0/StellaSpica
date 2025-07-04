using Discord;
using Discord.WebSocket;
using HarfBuzzSharp;
using ScottPlot;
using SkiaSharp;

partial class Program
{
    private Timer _midnightChecker2;
    private bool _alreadySent2 = false;

   // private int _stockCount;
  //  private int _recount = 0;


    public async Task StartTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
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

    public async Task ShowTrade(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        await ShowGraph2(message);
    }
    
    private async Task ShowGraph2(SocketMessage message)
    {
        // 例: テーブルデータ
        string[][] data =
        {
            new string[] { "名前", "年齢", "点数" },
            new string[] { "山田", "22", "89" },
            new string[] { "佐藤", "21", "95" },
            new string[] { "鈴木", "23", "78" }
        };

        // 列ごとの幅（最大文字数×フォント幅、などで調整してもOK）
        int[] colWidths = { 100, 80, 80 };
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

        await message.Channel.SendFileAsync("ncse_sample2.png");
    }
}
