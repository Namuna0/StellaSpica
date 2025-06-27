using Discord.WebSocket;
using System.Text;
using System.Text.RegularExpressions;

partial class Program
{
    class NpcStatus
    {
        public short MaxHp { get; set; }
        public short Hp { get; set; }
        public float BonB { get; set; }
        public float EleB { get; set; }

    }

    private async Task SimpleRoll(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        CalcFormula("1d100", out string culcResult, out string showResult);

        await message.Channel.SendMessageAsync(
        $"<@{user.Id}> :game_die:\r\n" +
        $"{showResult} => {culcResult}");
    }

    private async Task DiceRoll(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?r ".Length);
        var texts = text.Split(" ");

        if (!GetPaseFlag(texts, 3) && !GetPaseFlag(texts, 33))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        CalcFormula(texts[0], out string culcResult, out string showResult);

        string comment = string.Empty;
        if (texts.Length > 1)
        {
            comment = $"：{texts[1]}";
        }

        await message.Channel.SendMessageAsync(
            $"<@{user.Id}> :game_die:{comment}\r\n" +
            $"{showResult} => {culcResult}");
    }

    private async Task DiceMultiRoll(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?rr ".Length);
        var texts = text.Split(" ");

        if (!GetPaseFlag(texts, 31) && !GetPaseFlag(texts, 331))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        StringBuilder stringBuilder = new StringBuilder();

        float total = 0;

        for (int i = 0; i < int.Parse(texts[0]); i++)
        {
            CalcFormula(texts[1], out string culcResult, out string showResult);

            stringBuilder.Append($"\r\n{showResult} => {culcResult}");

            total += float.Parse(culcResult);
        }

        string comment = string.Empty;
        if (texts.Length > 2)
        {
            comment = $"：{texts[2]}";
        }

        await message.Channel.SendMessageAsync(
            $"<@{user.Id}> :game_die:{comment}" +
            $"{stringBuilder.ToString()}\r\n" +
            $"Total：{total.ToString("0.##")}");
    }

    private async Task PickRoll(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?p ".Length);
        var texts = text.Split(" ");

        if (texts.Length <= 2)
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        if (!int.TryParse(texts[0], out int pickCount))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        StringBuilder stringBuilder = new StringBuilder();
        StringBuilder stringBuilder2 = new StringBuilder();

        List<string> list = new List<string>();

        for (int i = 1; i < texts.Length; i++)
        {
            list.Add(texts[i]);

            stringBuilder2.Append(texts[i]);
            if (i < texts.Length - 1) stringBuilder2.Append($", ");
        }

        for (int i = 0; i < pickCount && list.Count > 0; i++)
        {
            int rand = _ms.Next(0, list.Count);

            stringBuilder.Append(list[rand]);
            if (i < pickCount - 1) stringBuilder.Append($", ");

            list.Remove(list[rand]);
        }

        await message.Channel.SendMessageAsync(
        $"<@{user.Id}> :game_die:\r\n" +
        $"{stringBuilder2} => {stringBuilder}");
    }

    private async Task PickMultiRoll(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?pp ".Length);
        var texts = text.Split(" ");

        if (texts.Length <= 3)
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        if (!int.TryParse(texts[0], out int tryCount))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        if (!int.TryParse(texts[1], out int pickCount))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        StringBuilder stringBuilder3 = new StringBuilder();

        for (int tryIndex = 0; tryIndex < tryCount; tryIndex++)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();

            List<string> list = new List<string>();

            for (int i = 2; i < texts.Length; i++)
            {
                list.Add(texts[i]);

                stringBuilder2.Append(texts[i]);
                if (i < texts.Length - 1) stringBuilder2.Append($", ");
            }

            for (int i = 0; i < pickCount && list.Count > 0; i++)
            {
                int rand = _ms.Next(0, list.Count);

                stringBuilder.Append(list[rand]);
                if (i < pickCount - 1) stringBuilder.Append($", ");

                list.Remove(list[rand]);
            }

            stringBuilder3.Append($"\r\n{stringBuilder2} => {stringBuilder}");
        }

        await message.Channel.SendMessageAsync(
        $"<@{user.Id}> :game_die:" +
        $"{stringBuilder3}");
    }

    private void CalcFormula(string originalText, out string culcResult, out string showResult)
    {
        string culcText = originalText;
        string showText = originalText;

        CalcDice(ref culcText, ref showText);

        var expr = new NCalc.Expression(culcText);

        showText = showText.Replace("*", @"\*");

        culcResult = float.Parse(expr.Evaluate()?.ToString() ?? "0").ToString("0.##");
        showResult = showText;
    }

    private void CalcDice(ref string culcText, ref string showText)
    {
        List<int> totals = new List<int>();

        showText = Regex.Replace(showText, @"(\d+)d(\d+)", match =>
        {
            int count = int.Parse(match.Groups[1].Value);
            int sides = int.Parse(match.Groups[2].Value);

            int total = 0;
            string dices = "";

            for (int i = 0; i < count; i++)
            {
                if (i > 0) dices += ", ";

                int dice = _ms.Next(1, sides + 1);

                total += dice;
                dices += dice.ToString();
            }
            totals.Add(total);

            return $"{count}d{sides}({dices})";
        });

        int i = 0;
        culcText = Regex.Replace(culcText, @"(\d+)d(\d+)", match =>
        {
            var result = (totals[i]).ToString();

            i++;

            return result;
        });
    }

    private class Suspect
    {
        public string Name;
        public string Background;
        public int Inference;
        public int Suspicion;
        public int Counter;
    }

    private async Task Reasoning(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var stage = "";
        int stageSuspectCount = 0;
        int stageInference = 0;
        int stageSuspicion = 0;

        int dice = _ms.Next(1, 7);
        if (dice == 1) stage = "1：朝（なにもなし）";
        else if (dice == 2)
        {
            stage = "2：庭園（容疑者数+1）";
            stageSuspectCount = 1;
        }
        else if (dice == 3)
        {
            stage = "3：レストラン（推理基準値+5）";
            stageInference = 5;
        }
        else if (dice == 4)
        {
            stage = "4：クラブ（全員の容疑度+1）";
            stageInference = 5;
            stageSuspicion = 1;
        }
        else if (dice == 5) stage = "5：路地裏（なにもなし）";
        else if (dice == 6)
        {
            stage = "6：密室（推理基準値+10）";
            stageSuspicion = 10;
        }

        var date = "";
        int dateInference = 0;
        int dateSuspicion = 0;
        dice = _ms.Next(1, 6);
        if (dice == 1)
        {
            date = "1：洋館（なにもなし）";
        }
        else if (dice == 2)
        {
            date = "2：昼（推理基準値+5）";
            dateInference = 5;
        }
        else if (dice == 3)
        {
            date = "3：夜（推理基準値+10）";
            dateInference = 10;
        }
        else if (dice == 4)
        {
            date = "4：雨天(全員の容疑度+1、推理基準値+5)";
            dateSuspicion = 1;
            dateInference = 5;
        }
        else if (dice == 5)
        {
            date = "5：吹雪(全員の容疑度+1、推理基準値+10)";
            dateSuspicion = 2;
            dateInference = 10;
        }

        var crime = "";
        int crimeCounter = 0;
        dice = _ms.Next(1, 3);
        if (dice == 1)
        {
            crime = "1：殺人（犯人の反撃値+1）";
            crimeCounter = 1;
        }
        else if (dice == 2)
        {
            crime = "2：連続殺人（犯人の反撃値+2）";
            crimeCounter = 2;
        }

        var suspects = _ms.Next(1, 4) + _ms.Next(1, 4) + 5 + stageSuspectCount;

        List<Suspect> suspectList = new List<Suspect>();

        for (var i = 1; i <= 50; i++)
        {
            Suspect suspect = new Suspect();

            if (i == 1)
            {
                suspect.Background = "①労働者(なにもなし)";
                suspect.Name = "労働者　　　　　";
            }
            else if (i == 2)
            {
                suspect.Background = "②法官◆(推理基準値 + 15、容疑度 - 2)";
                suspect.Name = "法官◆　　　　　";
                suspect.Inference = 15;
                suspect.Suspicion = -2;
            }
            else if (i == 3)
            {
                suspect.Background = "③弁護士◆(推理基準値 + 15、容疑度 - 2)";
                suspect.Name = "弁護士◆　　　　";
                suspect.Inference = 15;
                suspect.Suspicion = -2;
            }
            else if (i == 4)
            {
                suspect.Background = "④警官(容疑度 - 1)";
                suspect.Name = "警官　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = -1;
            }
            else if (i == 5)
            {
                suspect.Background = "⑤代議士◆(推理基準値 + 10)";
                suspect.Name = "代議士◆　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = 0;
            }
            else if (i == 6)
            {
                suspect.Background = "⑥兵卒(反撃値 + 1)";
                suspect.Name = "兵卒　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 0;
                suspect.Counter = 1;
            }
            else if (i == 7)
            {
                suspect.Background = "⑦将校◆(容疑度 - 1 & 反撃値 + 1)";
                suspect.Name = "将校◆　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = -1;
                suspect.Counter = 1;
            }
            else if (i == 8)
            {
                suspect.Background = "⑧官吏◆(容疑度 - 1)";
                suspect.Name = "官吏◆　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 9)
            {
                suspect.Background = "⑨掃除人(容疑度 + 1)";
                suspect.Name = "掃除人　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 10)
            {
                suspect.Background = "⑩溝浚い(容疑度 + 1)";
                suspect.Name = "溝浚い　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 11)
            {
                suspect.Background = "⑪点灯夫(容疑度 + 1)";
                suspect.Name = "点灯夫　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 12)
            {
                suspect.Background = "⑫資産家◆(容疑度 + 1 & 推理基準値 + 5)";
                suspect.Name = "資産家◆　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 13)
            {
                suspect.Background = "⑬賭博師(容疑度 + 1)";
                suspect.Name = "賭博師　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 14)
            {
                suspect.Background = "⑭イカサマ師(容疑度 + 2)";
                suspect.Name = "イカサマ師　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 15)
            {
                suspect.Background = "⑮手品師(推理基準値 + 5)";
                suspect.Name = "手品師　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 16)
            {
                suspect.Background = "⑯こそ泥(容疑度 + 2)";
                suspect.Name = "こそ泥　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 17)
            {
                suspect.Background = "⑰鉱夫(反撃値 + 1)";
                suspect.Name = "鉱夫　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 0;
                suspect.Counter = 1;
            }
            else if (i == 18)
            {
                suspect.Background = "⑱教授◆(推理基準値 + 5、容疑度 - 1)";
                suspect.Name = "教授◆　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 19)
            {
                suspect.Background = "⑲貴族◆(容疑度 - 2)";
                suspect.Name = "貴族◆　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = -2;
                suspect.Counter = 0;
            }
            else if (i == 20)
            {
                suspect.Background = "⑳貴族夫人◆(容疑度 - 1)";
                suspect.Name = "貴族夫人◆　　　";
                suspect.Inference = 0;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 21)
            {
                suspect.Background = "㉑水夫(推理基準値 + 5)";
                suspect.Name = "水夫　　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 22)
            {
                suspect.Background = "㉒船長◆(推理基準値 + 10)";
                suspect.Name = "船長◆　　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 23)
            {
                suspect.Background = "㉓実業家◆(容疑度 + 1、推理基準値 + 5)";
                suspect.Name = "実業家◆　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 24)
            {
                suspect.Background = "㉔パイロット◆(容疑度 - 1、推理基準値 + 5)";
                suspect.Name = "パイロット◆　　";
                suspect.Inference = 5;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 25)
            {
                suspect.Background = "㉕作家(容疑度 - 1、推理基準値 + 10)";
                suspect.Name = "作家　　　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 26)
            {
                suspect.Background = "㉖記者(容疑度 - 1、推理基準値 + 5)";
                suspect.Name = "記者　　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 27)
            {
                suspect.Background = "㉗浮浪者(容疑度 + 1、推理 + 5)";
                suspect.Name = "浮浪者　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 28)
            {
                suspect.Background = "㉘元犯罪者(容疑度 + 2、推理 + 10)";
                suspect.Name = "元犯罪者　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 29)
            {
                suspect.Background = "㉙暴漢(容疑度 + 1、反撃値 + 2)";
                suspect.Name = "暴漢　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 2;
            }
            else if (i == 30)
            {
                suspect.Background = "㉚使用人(容疑度 + 1)";
                suspect.Name = "使用人　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 31)
            {
                suspect.Background = "㉛使用人長◆(容疑度 + 2)";
                suspect.Name = "使用人長◆　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 32)
            {
                suspect.Background = "㉜恋人(容疑度 + 1)";
                suspect.Name = "恋人　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 33)
            {
                suspect.Background = "㉝旧友(容疑度 + 1)";
                suspect.Name = "旧友　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 34)
            {
                suspect.Background = "㉞相続人(容疑度 + 2)";
                suspect.Name = "相続人　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 35)
            {
                suspect.Background = "㉟医者◆(容疑度 - 1、推理基準値 + 5)";
                suspect.Name = "医者◆　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = -1;
                suspect.Counter = 0;
            }
            else if (i == 36)
            {
                suspect.Background = "㊱金貸し(容疑度 + 1、推理 + 5)";
                suspect.Name = "金貸し　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 37)
            {
                suspect.Background = "㊲マフィア(容疑度 + 1、推理基準値5)";
                suspect.Name = "マフィア　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 38)
            {
                suspect.Background = "㊳役者(推理基準値 + 15)";
                suspect.Name = "役者　　　　　　";
                suspect.Inference = 15;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 39)
            {
                suspect.Background = "㊴愛人(容疑度 + 2)";
                suspect.Name = "愛人　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 40)
            {
                suspect.Background = "㊵霊能者(容疑度 + 1、推理基準値 + 5)";
                suspect.Name = "霊能者　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 41)
            {
                suspect.Background = "㊶芸術家(容疑度 + 1、推理基準値 + 5)";
                suspect.Name = "芸術家　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 42)
            {
                suspect.Background = "㊷同僚(容疑度 + 1)";
                suspect.Name = "同僚　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 43)
            {
                suspect.Background = "㊸運転手(容疑度 + 1)";
                suspect.Name = "運転手　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 44)
            {
                suspect.Background = "㊹猟師(推理基準値 + 5)";
                suspect.Name = "猟師　　　　　";
                suspect.Inference = 5;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 45)
            {
                suspect.Background = "㊺座長(推理基準値 + 10)";
                suspect.Name = "座長　　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 46)
            {
                suspect.Background = "㊻劇場主(推理基準値 + 10)";
                suspect.Name = "劇場主　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = 0;
                suspect.Counter = 0;
            }
            else if (i == 47)
            {
                suspect.Background = "㊼マフィアのボス◆(容疑度 + 2、推理基準値 + 10)";
                suspect.Name = "マフィアのボス◆";
                suspect.Inference = 10;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }
            else if (i == 48)
            {
                suspect.Background = "㊽編集長◆(容疑度 - 2、推理基準値 + 10)";
                suspect.Name = "編集長◆　　　　";
                suspect.Inference = 10;
                suspect.Suspicion = -2;
                suspect.Counter = 0;

            }
            else if (i == 49)
            {
                suspect.Background = "㊾客人(容疑度 + 1)";
                suspect.Name = "客人　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 1;
                suspect.Counter = 0;
            }
            else if (i == 50)
            {
                suspect.Background = "㊿奴隷(容疑度 + 2)";
                suspect.Name = "奴隷　　　　　　";
                suspect.Inference = 0;
                suspect.Suspicion = 2;
                suspect.Counter = 0;
            }

            suspectList.Add(suspect);
        }

        StringBuilder sb = new StringBuilder();

        int index = 1;
        for (var i = 0; i < suspects; i++)
        {
            var suspect = suspectList[_ms.Next(0, suspects)];

            int suspiction = suspect.Suspicion + stageSuspicion + dateSuspicion;
            int counter = suspect.Counter + crimeCounter;
            int inference = suspect.Inference + stageInference + dateInference;

            sb.Append($" {index.ToString(" 0")}｜{suspect.Name}｜{suspect.Suspicion.ToString(" 0")}｜{suspect.Counter.ToString(" 0")}｜{suspect.Inference.ToString(" 0")}｜0  \r\n");

            index++;
        }

        string result = "```" +
        $"舞台(1d6):{stage}\r\n" +
        "\r\n" +
        $"時刻(1d5):{date}\r\n" +
        "\r\n" +
        $"犯行内容(1d2):{crime}\r\n" +
        "\r\n" +
        "容疑者数(2d3+5)\r\n" +
        $"{suspects}名 \r\n" +
        "―――【容疑者リスト】――― \r\n" +
        "No｜素性(1d50)        ｜容｜反｜基｜犯  \r\n" +
        "––｜––––––––––––––––––｜––｜––｜––｜––  \r\n" +
        $"{sb}" +
        "―――――\r\n" +
        "証拠リスト(一度出た証拠は別の人からは出ない/都度ダイスから削除)\r\n" +
        "①凶器を持っていた(〇〇〇→〇〇〇)\r\n" +
        "【証拠品：凶器と組み合わせることで証言者を除いた1d容疑者でダイスを行い、該当した名前の容疑者の犯人値+2】\r\n" +
        "\r\n" +
        "②犯行現場をうろついていた(〇〇〇→〇〇〇)\r\n" +
        "【証拠品：足跡を組み合わせることで証言者を除いた1d容疑者でダイスを行い、該当した名前の容疑者の犯人値+1】\r\n" +
        "\r\n" +
        "③被害者への恨み(〇〇〇→〇〇〇)\r\n" +
        "【1d容疑者で該当した名前の容疑者の犯人値+1/複数の被害者がいる場合、1d被害者を行って恨みを持っていた被害者を明示できる】\r\n" +
        "―――――\r\n" +
        "証拠品リスト(最大10個)\r\n" +
        "\r\n" +
        "―――――\r\n" +
        "容疑度付与(1d3)\r\n" +
        "1t目:\r\n" +
        "―――――\r\n" +
        "被害者(殺人:1人/連続殺人:2d5人)\r\n" +
        "-名" +
        "```";

        await message.Channel.SendMessageAsync(result);
    }
}
