using Discord;
using Discord.WebSocket;
using dotenv.net;
using MathNet.Numerics.Random;

class Program
{
    class UserStatus
    {
        public int MaxHp { get; set; }
        public int MaxSp { get; set; }
        public int MaxMp { get; set; }
        public int Hp { get; set; }
        public int Sp { get; set; }
        public int Mp { get; set; }

        public UserStatus(int hp, int sp, int mp)
        {
            MaxHp = hp;
            MaxSp = sp;
            MaxMp = mp;
            Hp = hp;
            Sp = sp;
            Mp = mp;
        }
    }

    private DiscordSocketClient? _client;
    private Dictionary<string, UserStatus> _userStatusDic = new Dictionary<string, UserStatus>();

    private MersenneTwister _ms = new MersenneTwister();

    static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged |
                     GatewayIntents.MessageContent |
                     GatewayIntents.GuildMessages
        };

        _client = new DiscordSocketClient(config);

        _client.Log += Log;
        _client.MessageReceived += MessageReceivedAsync;

        DotEnv.Load();
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Discordトークンが見つかりません。");
            return;
        }

        long ticks = DateTime.Now.Ticks; // 100ナノ秒単位
        int seed = (int)(ticks & 0xFFFFFFFF); // 下位32ビットを使用
        _ms = new MersenneTwister(seed);

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1); // 永久に実行
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        if (message is SocketUserMessage userMessage && message.Channel is SocketGuildChannel guildChannel)
        {
            var content = message.Content;

            if (content == "?auto")
            {
                var guild = guildChannel.Guild;
                var user = guild.GetUser(message.Author.Id);
                string displayName = user.DisplayName; // ニックネームがない場合はユーザー名を使用

                string past0A = NextPast0();
                string past0B = NextPast0();
                string past0C = NextPast0();
                string past1A = NextPast1();
                string past1B = NextPast1();
                string past1C = NextPast1();
                string past2A = NextPast2();
                string past2B = NextPast2();
                string past2C = NextPast2();

                string repast0 = "";
                string repast1 = "";
                string repast2 = "";
                string repastEnd = "";

                if (past0A == past0B || past0A == past0C || past0B == past0C)
                {
                    repast0 =
                        "``````\r\n" +
                        $"振り直し結果：{NextPast0()}\r\n";
                }

                if (past1A == past1B || past1A == past1C || past1B == past1C)
                {
                    repast1 =
                        "``````\r\n" +
                        $"振り直し結果：{NextPast1()}\r\n";
                }

                if (past2A == past2B || past2A == past2C || past2B == past2C)
                {
                    repast2 =
                        "``````\r\n" +
                        $"振り直し結果：{NextPast2()}\r\n";
                }

                if (!string.IsNullOrEmpty(repast0) || !string.IsNullOrEmpty(repast1) || !string.IsNullOrEmpty(repast2))
                {
                    repastEnd = "``````\r\n";
                }

                var result =
                    $"使用者:<@{user.Id}>\r\n" +
                    "```\r\n" +
                    "名前:\r\n" +
                    "レベル:0\r\n" +
                    $"家柄:[{Next()}, {Next()}, {Next()}, {Next()}, {Next()}]\r\n" +
                    "種族:\r\n" +
                    "年代:\r\n" +
                    "出自:\r\n" +
                    "性別:\r\n" +
                    $"属性:{NextElement()} or {NextElement()}\r\n" +
                    "人種:\r\n" +
                    "[過去]\r\n" +
                    $"0章:{NextPast0()} or {NextPast0()} or {NextPast0()}\r\n" +
                    $"1章:{NextPast1()} or {NextPast1()} or {NextPast1()}\r\n" +
                    $"2章:{NextPast2()} or {NextPast2()} or {NextPast2()}\r\n" +
                    repast0 +
                    repast1 +
                    repast2 +
                    repastEnd +
                    $"筋力:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"知力:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"敏捷:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"精神:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"体格:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"生命:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"容姿:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"芸術:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    $"商才:[{NextStatus()}, {NextStatus()}, {NextStatus()}, {NextStatus()}]\r\n" +
                    "信仰:\r\n" +
                    "``````\r\n" +
                    "HP:\r\n" +
                    "MP:\r\n" +
                    "幸運:\r\n" +
                    "スタミナ:\r\n" +
                    "気絶点:\r\n" +
                    "依存点:\r\n" +
                    "魅力:\r\n" +
                    "知識:\r\n" +
                    "精神限界:\r\n" +
                    "基礎技能P:\r\n" +
                    "母国語(初期判定値70):\r\n" +
                    "AB:\r\n" +
                    "白兵:1d 魔法:1d\r\n" +
                    "``````\r\n" +
                    "サブステータス:\r\n" +
                    "膂力:\r\n" +
                    "叡智:\r\n" +
                    "体力:\r\n" +
                    "持久力:\r\n" +
                    "技量:\r\n" +
                    "神聖:\r\n" +
                    "美学:\r\n" +
                    "商売:\r\n" +
                    "表現力:\r\n" +
                    "探求心:\r\n" +
                    "```";

                await message.Channel.SendMessageAsync(result);
            }
            else if (content == "?こんにちは")
            {
                var guild = guildChannel.Guild;
                var user = guild.GetUser(message.Author.Id);
                string displayName = !string.IsNullOrEmpty(user.Nickname) ? user.Nickname : user.Username; // ニックネームがない場合はユーザー名を使用

                Console.WriteLine($"Display Name: {displayName}");
                await message.Channel.SendMessageAsync($"こんにちは、{displayName}さん！");
            }
            else if (content == "にゃん")
            {
                await message.Channel.SendMessageAsync($"にゃーっ");
            }
            else if (content.StartsWith("?set "))
            {
                var text = content.Substring("?set ".Length);
                var texts = text.Split(" ");
                if (texts.Length >= 4)
                {
                    var status = new UserStatus(int.Parse(texts[1]), int.Parse(texts[2]), int.Parse(texts[3]));
                    _userStatusDic[texts[0]] = status;

                    await ShowStatus(texts[0], status, message);
                }
                else
                {
                    await message.Channel.SendMessageAsync("引数が変です。");
                }
            }
            else if (content.StartsWith("?hp "))
            {
                var text = content.Substring("?hp ".Length);
                var texts = text.Split(" ");
                if (texts.Length < 2)
                {
                    await message.Channel.SendMessageAsync("引数が変です。");
                }
                else if (!_userStatusDic.TryGetValue(texts[0], out var status))
                {
                    await message.Channel.SendMessageAsync("「?set [キャラクター名]」を呼んでください。");
                }
                else
                {
                    status.Hp += int.Parse(texts[1]);
                    await ShowStatus(texts[0], status, message);
                }
            }
            else if (content.StartsWith("?sp "))
            {
                var text = content.Substring("?sp ".Length);
                var texts = text.Split(" ");
                if (texts.Length < 2)
                {
                    await message.Channel.SendMessageAsync("引数が変です。");
                }
                else if (!_userStatusDic.TryGetValue(texts[0], out var status))
                {
                    await message.Channel.SendMessageAsync("「?set [キャラクター名]」を呼んでください。");
                }
                else
                {
                    status.Sp += int.Parse(texts[1]);
                    await ShowStatus(texts[0], status, message);
                }
            }
            else if (content.StartsWith("?mp "))
            {
                var text = content.Substring("?mp ".Length);
                var texts = text.Split(" ");
                if (texts.Length < 2)
                {
                    await message.Channel.SendMessageAsync("引数が変です。");
                }
                else if (!_userStatusDic.TryGetValue(texts[0], out var status))
                {
                    await message.Channel.SendMessageAsync("「?set [キャラクター名]」を呼んでください。");
                }
                else
                {
                    status.Mp += int.Parse(texts[1]);
                    await ShowStatus(texts[0], status, message);
                }
            }
        }
    }

    private async Task ShowStatus(string key, UserStatus status, SocketMessage message)
    {
        await message.Channel.SendMessageAsync(
            $"{key}\r\n" +
            "●リソース\r\n" +
            $"【HP】{status.Hp}/{status.MaxHp}\r\n" +
            $"【SP】{status.Sp}/{status.MaxSp}\r\n" +
            $"【MP】{status.Mp}/{status.MaxMp}");
    }

    private int Next()
    {
        return _ms.Next(1, 101);
    }

    private string NextStatus()
    {
        int random = _ms.Next(1, 46) + 5;
        return random.ToString("D").PadLeft(2, ' ');
    }

    private string NextElement()
    {
        int random = _ms.Next(1, 101);

        if (random >= 1 && random <= 9) return "地属性";
        else if (random >= 10 && random <= 19) return "花属性";
        else if (random >= 20 && random <= 29) return "雪属性";
        else if (random >= 30 && random <= 39) return "氷属性";
        else if (random >= 40 && random <= 49) return "水属性";
        else if (random >= 50 && random <= 59) return "風属性";
        else if (random >= 60 && random <= 69) return "炎属性";
        else if (random >= 70 && random <= 79) return "雷属性";
        else if (random >= 80 && random <= 89) return "光属性";
        else if (random >= 90 && random <= 99) return "夜属性";
        else return "双属性";
    }

    private string NextPast0()
    {
        int random = _ms.Next(1, 9);

        if (random == 1) return "凡庸";
        else if (random == 2) return "生存";
        else if (random == 3) return "悲哀";
        else if (random == 4) return "愚行";
        else if (random == 5) return "才能";
        else if (random == 6) return "血統";
        else if (random == 7) return "復讐";
        else return "不要";
    }

    private string NextPast1()
    {
        int random = _ms.Next(1, 7);

        if (random == 1) return "孤独";
        else if (random == 2) return "平凡";
        else if (random == 3) return "愛情";
        else if (random == 4) return "禁断";
        else if (random == 5) return "特別";
        else return "苦痛";
    }

    private string NextPast2()
    {
        int random = _ms.Next(1, 7);

        if (random == 1) return "暴力";
        else if (random == 2) return "矜持";
        else if (random == 3) return "風詠";
        else if (random == 4) return "勤勉";
        else if (random == 5) return "失意";
        else return "憧憬";
    }
}