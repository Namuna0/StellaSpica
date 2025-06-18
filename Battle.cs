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
}
