using Discord.WebSocket;
using System.Text;

partial class Program
{
    private async Task Gather(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?gather ".Length);
        var texts = text.Split(" ");

        await Command2(texts, 113, message, user, async () =>
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
                else if (texts[0] == "白亜の森")
                {
                    if (rand >= 1 && rand <= 50) item[0]++;
                    else if (rand >= 51 && rand <= 70) item[1]++;
                    else if (rand >= 71 && rand <= 85) item[2]++;
                    else if (rand >= 86 && rand <= 90) item[3]++;
                    else if (rand >= 91 && rand <= 95) item[4]++;
                    else if (rand >= 96 && rand <= 100) item[5]++;
                }
                else if (texts[0] == "休火山の麓")
                {
                    if (rand >= 1 && rand <= 40) item[0]++;
                    else if (rand >= 41 && rand <= 75) item[1]++;
                    else if (rand >= 76 && rand <= 95) item[2]++;
                    else if (rand >= 96 && rand <= 100) item[3]++;
                }
                else if (texts[0] == "妖精の森")
                {
                    if (rand >= 1 && rand <= 50) item[0]++;
                    else if (rand >= 51 && rand <= 60) item[1]++;
                    else if (rand >= 61 && rand <= 70) item[2]++;
                    else if (rand >= 71 && rand <= 80) item[3]++;
                    else if (rand >= 81 && rand <= 95) item[4]++;
                    else if (rand >= 96 && rand <= 100) item[5]++;
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
            else if (texts[0] == "白亜の森")
            {
                area = "🌳白亜の森（Lv2から利用可能）\r\n【採取《1d100》1-50：雑草/51-70：薬草/71-85：木立のトウガラシ/86-90：青癒草/91-95:ハチミツ/96-100：カカオ】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"雑草×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",薬草×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",木立のトウガラシ×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",青癒草×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",ハチミツ×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",カカオ×{item[5]}");
            }
            else if (texts[0] == "休火山の麓")
            {
                area = "🌋休火山の麓\r\n【採取《1d100》1-40：石ころ/41-75：硫黄/76-95：硝石/96-100：魔水晶の破片】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",硫黄×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",硝石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",魔水晶の破片×{item[3]}");
            }
            else if (texts[0] == "妖精の森")
            {
                area = "🌲妖精の森（Lv4〜）\r\n【採取《1d100》1-50：なにもなし/51-60：緑癒草/61-70：ヒラタケ/71-80：妖精トンボ/81-95：マボロシドングリ/96-100：マンドラゴラ】\r\n";

                if (item[1] > 0) stringBuilder3.Append($",緑癒草×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",ヒラタケ×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",妖精トンボ×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",マボロシドングリ×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",マンドラゴラ×{item[5]}");
            }
            
            await message.Channel.SendMessageAsync(
            $"<@{user.Id}> :game_die:\r\n" +
            $"```{area}```" +
            $"```【俊敏{clear}】{stringBuilder}\r\n" +
            $"【{count}回達成】{stringBuilder2}\r\n" +
            $"【結果】{stringBuilder3}```");
        });
    }

    private async Task Fell(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?fell ".Length);
        var texts = text.Split(" ");

        await Command2(texts, 113, message, user, async () =>
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
                    else if (rand >= 51 && rand <= 95) item[1]++;
                    else if (rand >= 96 && rand <= 100) item[2]++;
                }
                else if (texts[0] == "エール湖湖畔")
                {
                    if (rand >= 1 && rand <= 50) item[0]++;
                    else if (rand >= 51 && rand <= 95) item[1]++;
                    else if (rand >= 96 && rand <= 100) item[2]++;
                }
                else if (texts[0] == "妖精の森")
                {
                    if (rand >= 1 && rand <= 50) item[0]++;
                    else if (rand >= 51 && rand <= 95) item[1]++;
                    else if (rand >= 96 && rand <= 100) item[2]++;
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
                area = "🌳都市近郊の草原（Lv0から利用可能）\r\n【伐採《1d100》1-50：何もなし/51-95：杉材/96-100：リンゴ（果物）】\r\n";

                if (item[1] > 0) stringBuilder3.Append($",杉材×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",リンゴ×{item[2]}");
            }
            else if (texts[0] == "エール湖湖畔")
            {
                area = "🏕️エール湖湖畔（Lv1から利用可能）\r\n【伐採《1d100》1-50：何もなし/51-95：杉材/96-100：リンゴ（果物）】\r\n";

                if (item[1] > 0) stringBuilder3.Append($",白樺材×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",ルチェの実（果物）×{item[2]}");
            }
            else if (texts[0] == "妖精の森")
            {
                area = "🌲妖精の森（Lv4〜）\r\n【伐採《1d100》1-50：何もなし/51-95：ブナ材/96-100：妖精樹の枝】\r\n";
               
                if (item[1] > 0) stringBuilder3.Append($",ブナ材×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",妖精樹の枝×{item[2]}");
            }

            await message.Channel.SendMessageAsync(
            $"<@{user.Id}> :game_die:\r\n" +
            $"```{area}```" +
            $"```【筋力{clear}】{stringBuilder}\r\n" +
            $"【{count}回達成】{stringBuilder2}\r\n" +
            $"【結果】{stringBuilder3}```");
        });
    }

    private async Task Mine(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?mine ".Length);
        var texts = text.Split(" ");

        await Command2(texts, 113, message, user, async () =>
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

                if (texts[0] == "都市近郊")
                {
                    if (rand >= 1 && rand <= 35) item[0]++;
                    else if (rand >= 36 && rand <= 64) item[1]++;
                    else if (rand >= 65 && rand <= 80) item[2]++;
                    else if (rand >= 81 && rand <= 95) item[3]++;
                    else if (rand >= 96 && rand <= 99) item[4]++;
                    else if (rand >= 100) item[5]++;
                }
                else if (texts[0] == "ミドガルネ北方")
                {
                    if (rand >= 1 && rand <= 40) item[0]++;
                    else if (rand >= 41 && rand <= 70) item[1]++;
                    else if (rand >= 71 && rand <= 85) item[2]++;
                    else if (rand >= 86 && rand <= 95) item[3]++;
                    else if (rand >= 96 && rand <= 99) item[4]++;
                    else if (rand >= 100) item[5]++;
                }
                else if (texts[0] == "ミドガルネ南方")
                {
                    if (rand >= 1 && rand <= 40) item[0]++;
                    else if (rand >= 41 && rand <= 60) item[1]++;
                    else if (rand >= 61 && rand <= 70) item[2]++;
                    else if (rand >= 71 && rand <= 80) item[3]++;
                    else if (rand >= 81 && rand <= 95) item[4]++;
                    else if (rand >= 96 && rand <= 99) item[5]++;
                    else if (rand >= 100) item[6]++;
                }
                else if (texts[0] == "竜山地帯\r\n")
                {
                    if (rand >= 1 && rand <= 40) item[0]++;
                    else if (rand >= 41 && rand <= 50) item[1]++;
                    else if (rand >= 51 && rand <= 70) item[2]++;
                    else if (rand >= 71 && rand <= 80) item[3]++;
                    else if (rand >= 81 && rand <= 95) item[4]++;
                    else if (rand >= 96 && rand <= 99) item[5]++;
                    else if (rand >= 100) item[6]++;
                }
                else if (texts[0] == "ドラゴンズエッジ")
                {
                    if (rand >= 1 && rand <= 40) item[0]++;
                    else if (rand >= 41 && rand <= 60) item[1]++;
                    else if (rand >= 61 && rand <= 70) item[2]++;
                    else if (rand >= 71 && rand <= 80) item[3]++;
                    else if (rand >= 81 && rand <= 95) item[4]++;
                    else if (rand >= 96 && rand <= 99) item[5]++;
                    else if (rand >= 100) item[6]++;
                }
                else if (texts[0] == "古の坑道")
                {
                    if (rand >= 1 && rand <= 40) item[0]++;
                    else if (rand >= 41 && rand <= 60) item[1]++;
                    else if (rand >= 61 && rand <= 70) item[2]++;
                    else if (rand >= 71 && rand <= 80) item[3]++;
                    else if (rand >= 81 && rand <= 90) item[4]++;
                    else if (rand >= 91 && rand <= 95) item[5]++;
                    else if (rand >= 96 && rand <= 100) item[6]++;
                }

                stringBuilder2.Append(rand);

                if (i < count - 1)
                {
                    stringBuilder2.Append(",");
                }
            }

            var area = "";

            if (texts[0] == "都市近郊")
            {
                area = ":rock:都市近郊（Lv0から利用可能）\r\n【《1d100》1-35：石ころ/36-64：石炭/65-80：硝石/81-95：鉄鉱石/96-99：銅鉱石/100：金鉱石】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",石炭×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",硝石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",鉄鉱石×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",銅鉱石×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",金鉱石×{item[5]}");
            }
            else if (texts[0] == "🏔️ミドガルネ北方")
            {
                area = "🏔️ミドガルネ北方（Lv1から利用可能）\r\n【《1d100》1-40：石ころ/41-70：石炭/71-85：鉄鉱石/86-95：銅鉱石/96-99：銀鉱石/100：金鉱石】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",石炭×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",鉄鉱石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",銅鉱石×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",銀鉱石×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",金鉱石×{item[5]}");
            }
            else if (texts[0] == "🏔️ミドガルネ南方")
            {
                area = "🏔️ミドガルネ南方（Lv2から利用可能）\r\n【《1d100》1-40：石ころ/41-60：石炭/61-70：鉄鉱石/71-80：銅鉱石/81-95：銀鉱石/96-99：月長石/100：金鉱石】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",石炭×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",鉄鉱石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",銅鉱石×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",銀鉱石×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",月長石×{item[5]}");
                if (item[6] > 0) stringBuilder3.Append($",金鉱石×{item[6]}");
            }
            else if (texts[0] == "竜山地帯")
            {
                area = "🏔️竜山地帯\r\n【《1d100》1-40：石ころ/41-50：石炭/51-70：鉄鉱石/71-80：銅鉱石/81-95：銀鉱石/96-99：蒼鉄晶/100：金鉱石】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",石炭×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",鉄鉱石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",銅鉱石×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",銀鉱石×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",蒼鉄晶×{item[5]}");
                if (item[6] > 0) stringBuilder3.Append($",金鉱石×{item[6]}");
            }
            else if (texts[0] == "ドラゴンズエッジ")
            {
                area = "🏔️ドラゴンズエッジ（Lv4から利用可能）\r\n【《1d100》1-40：石ころ/41-60：鉄鉱石/61-70：銅鉱石/71-80：銀鉱石/81-95：蒼鉄晶/96-99：アルブム・クリスタル/100：金鉱石】\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",鉄鉱石×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",銅鉱石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",銀鉱石×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",蒼鉄晶×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",アルブム・クリスタル×{item[5]}");
                if (item[6] > 0) stringBuilder3.Append($",金鉱石×{item[6]}");
            }
            else if (texts[0] == "古の坑道")
            {
                area = "🏔️古の坑道（Lv4から利用可能）\r\n【《1d100》1-40：石ころ/41-60：鉄鉱石/61-70：銅鉱石/71-80：銀鉱石/81-90：蒼鉄晶/91-95：水銀鉱/96-100：灰銀石\r\n";

                if (item[0] > 0) stringBuilder3.Append($"石ころ×{item[0]}");
                if (item[1] > 0) stringBuilder3.Append($",鉄鉱石×{item[1]}");
                if (item[2] > 0) stringBuilder3.Append($",銅鉱石×{item[2]}");
                if (item[3] > 0) stringBuilder3.Append($",銀鉱石×{item[3]}");
                if (item[4] > 0) stringBuilder3.Append($",蒼鉄晶×{item[4]}");
                if (item[5] > 0) stringBuilder3.Append($",水銀鉱×{item[5]}");
                if (item[6] > 0) stringBuilder3.Append($",灰銀石×{item[6]}");
            }

            await message.Channel.SendMessageAsync(
            $"<@{user.Id}> :game_die:\r\n" +
            $"```{area}```" +
            $"```【筋力{clear}】{stringBuilder}\r\n" +
            $"【{count}回達成】{stringBuilder2}\r\n" +
            $"【結果】{stringBuilder3}```");
        });
    }
}
