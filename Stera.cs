using Discord.WebSocket;
using System.Text;

partial class Program
{
    private async Task Gather(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?gather ".Length);
        var texts = text.Split(" ");

        await Command(texts, 113, message, user, async (currentChara) =>
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            StringBuilder stringBuilder3 = new StringBuilder();

            List<string> list = new List<string>();

            int size = int.Parse(texts[1]);
            int clear = int.Parse(texts[2]);
            int count = 0;
            for (int i = 0; i < size; i++)
            {
                int rand = _ms.Next(1, 101);
                if (rand <= clear) count++;

                stringBuilder.Append(rand);

                if (i < size - 1)
                {
                    stringBuilder.Append(",");
                }
            }

            int[] item = new int[8];
            for (int i = 0; i < count; i++)
            {
                int rand = _ms.Next(1, 101);

                if (texts[0] == "都市近郊の草原")
                {
                    if (rand >= 1 && rand <= 50) item[0]++;
                    else if (rand >= 51 && rand <= 70) item[1]++;
                    else if (rand >= 71 && rand <= 90) item[2]++;
                    else if (rand >= 91 && rand <= 100) item[3]++;
                }
                else if (texts[0] == "エール湖湖畔")
                {
                    if (rand >= 1 && rand <= 30) item[0]++;
                    else if (rand >= 31 && rand <= 40) item[1]++;
                    else if (rand >= 41 && rand <= 50) item[2]++;
                    else if (rand >= 51 && rand <= 60) item[3]++;
                    else if (rand >= 61 && rand <= 70) item[4]++;
                    else if (rand >= 71 && rand <= 80) item[5]++;
                    else if (rand >= 81 && rand <= 90) item[6]++;
                    else if (rand >= 91 && rand <= 100) item[7]++;
                }

                stringBuilder2.Append(rand);

                if (i < count - 1)
                {
                    stringBuilder2.Append(",");
                }
            }

            var area = "";

            if (texts[0] == "都市近郊の草原")
            {
                area = "🌳都市近郊の草原（Lv0から利用可能）\r\n【採取《1d100》1-50：雑草/51-70：キノコ/71-90：薬草/91-100：タマゴ】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"雑草×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",キノコ×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",薬草×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",タマゴ×{item[3]}");
            }
            else if (texts[0] == "エール湖湖畔")
            {
                area = "🏕️エール湖湖畔（Lv1から利用可能）\r\n【採取《1d100》1-30：雑草/31-40：綺麗な砂/41-50：海藻/51-60：カワガニ/61-70：小エビ/71-80：エビ/81-90：清水/91-100：砂鉄】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"雑草×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",綺麗な砂×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",海藻×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",カワガニ×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",小エビ×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",エビ×{item[5]}");
                if (item[6] > 0) stringBuilder3.Append($",清水×{item[6]}");
                if (item[7] > 0) stringBuilder3.Append($",砂鉄×{item[7]}");
            }

            await message.Channel.SendMessageAsync(
            $"<@{user.Id}> :game_die:\r\n" +
            $"```{area}```" +
            $"```【俊敏{clear}】{stringBuilder}\r\n" +
            $"【{count}回達成】{stringBuilder2}\r\n" +
            $"【結果】{stringBuilder3}```");
        });
    }
}
