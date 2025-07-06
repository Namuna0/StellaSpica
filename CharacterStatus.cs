using Discord.WebSocket;

partial class Program
{
    private async Task Login(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?login ".Length);
        var texts = text.Split(" ");

        if (!GetPaseFlag(texts, 3))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        await ConnectDatabase($@"
            INSERT INTO login_status (discord_id, character_name)
            VALUES (@discord_id, @character_name)
            ON CONFLICT (discord_id) DO UPDATE
            SET character_name = EXCLUDED.character_name;",
            parameters =>
            {
                parameters.AddWithValue("discord_id", (long)user.Id);
                parameters.AddWithValue("character_name", texts[0]);
            });

        _currentCharaDic[user.Id] = texts[0];

        await message.Channel.SendMessageAsync($"こんにちは、{texts[0]}さん！");
    }

    private async Task Start()
    {
        await ConnectDatabase("SELECT * FROM login_status;",
            onResponce: async (reader) =>
            {
                do
                {
                    var userId = (ulong)reader.GetInt64(reader.GetOrdinal("discord_id"));
                    var characterName = reader.GetString(reader.GetOrdinal("character_name"));

                    _currentCharaDic[userId] = characterName;
                } while (reader.Read());

                await Task.CompletedTask;
            });
    }

    private async Task Auto(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
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
            $"0章:{past0A} or {past0B} or {past0C}\r\n" +
            $"1章:{past1A} or {past1B} or {past1C}\r\n" +
            $"2章:{past2A} or {past2B} or {past2C}\r\n" +
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