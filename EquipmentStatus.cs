using Discord.WebSocket;

partial class Program
{
    class EquipmentStatus
    {
        public short MaxHp { get; set; }
        public short MaxSp { get; set; }
        public short MaxSan { get; set; }
        public short MaxMp { get; set; }
        public short Hp { get; set; }
        public short Sp { get; set; }
        public short San { get; set; }
        public short Mp { get; set; }
    }

    private async Task ShowRes(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        if (!_currentCharaDic.TryGetValue(user.Id, out var currentChara))
        {
            await message.Channel.SendMessageAsync("「?login [キャラクター名]」を呼んでください。");
            return;
        }

        await DisplayResource(currentChara, message);
    }

    private async Task SetRes(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring("?set res ".Length);
        var texts = text.Split(" ");
        await Command(texts, 1111, message, user, async (currentChara) =>
        {
            var status = new EquipmentStatus();
            status.MaxHp = short.Parse(texts[0]);
            status.MaxSp = short.Parse(texts[1]);
            status.MaxSan = short.Parse(texts[2]);
            status.MaxMp = short.Parse(texts[3]);
            status.Hp = status.MaxHp;
            status.Sp = status.MaxSp;
            status.San = status.MaxSan;
            status.Mp = status.MaxMp;

            await ConnectDatabase(
                @"INSERT INTO character_equipment (id, max_hp, max_sp, max_san, max_mp, hp, sp, san, mp)" +
                @"VALUES (@id, @max_hp, @max_sp, @max_san, @max_mp, @hp, @sp, @san, @mp)" +
                @"ON CONFLICT (id) DO UPDATE SET max_hp = EXCLUDED.max_hp, max_sp = EXCLUDED.max_sp, max_san = EXCLUDED.max_san, max_mp = EXCLUDED.max_mp, hp = EXCLUDED.hp, sp = EXCLUDED.sp, san = EXCLUDED.san, mp = EXCLUDED.mp;",
                parameters =>
                {
                    parameters.AddWithValue("id", currentChara);
                    parameters.AddWithValue("max_hp", status.MaxHp);
                    parameters.AddWithValue("max_sp", status.MaxSp);
                    parameters.AddWithValue("max_san", status.MaxSan);
                    parameters.AddWithValue("max_mp", status.MaxMp);
                    parameters.AddWithValue("hp", status.Hp);
                    parameters.AddWithValue("sp", status.Sp);
                    parameters.AddWithValue("san", status.San);
                    parameters.AddWithValue("mp", status.Mp);
                });

            await DisplayResource(currentChara, status, message);
        });
    }

    private async Task ResetRes(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        if (!_currentCharaDic.TryGetValue(user.Id, out var currentChara))
        {
            await message.Channel.SendMessageAsync("「?login [キャラクター名]」を呼んでください。");
            return;
        }

        await ConnectDatabase(
            @"INSERT INTO character_equipment (id, max_hp, max_sp, max_san, max_mp, hp, sp, san, mp)" +
            @"VALUES (@id, 0, 0, 0, 0, 0, 0, 0, 0)" +
            @"ON CONFLICT (id) DO UPDATE SET hp = character_equipment.max_hp, sp = character_equipment.max_sp, san = character_equipment.max_san, mp = character_equipment.max_mp;",
            parameters =>
            {
                parameters.AddWithValue("id", currentChara);
            });

        await DisplayResource(currentChara, message);
    }

    private async Task UpdateRes(string res, SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring($"?{res} ".Length);
        var texts = text.Split(" ");
        await Command(texts, 1, message, user, async (currentChara) =>
        {
            await ConnectDatabase(
                @$"INSERT INTO character_equipment (id, {res})" +
                @$"VALUES (@id, @{res})" +
                @$"ON CONFLICT (id) DO UPDATE SET {res} = LEAST(character_equipment.{res} + EXCLUDED.{res}, character_equipment.max_{res});",
                parameters =>
                {
                    parameters.AddWithValue("id", currentChara);
                    parameters.AddWithValue($"{res}", short.Parse(texts[0]));
                });

            await DisplayResource(currentChara, message);
        });
    }

    private async Task ShowMasterRes(SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring($"?show master res ".Length);
        var texts = text.Split(" ");

        if (texts.Length < 1)
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        await DisplayResource(texts[0], message);
    }

    private async Task UpdateMasterRes(string res, SocketMessage message, SocketGuild guild, SocketGuildUser user)
    {
        var text = message.Content.Substring($"?master {res} ".Length);
        var texts = text.Split(" ");
        await Command(texts, 13, message, user, async (currentChara) =>
        {
            await ConnectDatabase(
                @$"INSERT INTO character_equipment (id, {res})" +
                @$"VALUES (@id, @{res})" +
                @$"ON CONFLICT (id) DO UPDATE SET {res} = LEAST(character_equipment.{res} + EXCLUDED.{res}, character_equipment.max_{res});",
                parameters =>
                {
                    parameters.AddWithValue("id", texts[0]);
                    parameters.AddWithValue($"{res}", short.Parse(texts[1]));
                });

            await DisplayResource(texts[0], message);
        });
    }

    private async Task DisplayResource(string currentChara, SocketMessage message)
    {
        await ConnectDatabase(
@"WITH upsert AS (
    INSERT INTO character_equipment (id, max_hp, max_sp, max_san, max_mp, hp, sp, san, mp)
    VALUES (@id, 0, 0, 0, 0, 0, 0, 0, 0)
    ON CONFLICT (id) DO NOTHING
)
SELECT max_hp, max_sp, max_san, max_mp, hp, sp, san, mp
FROM character_equipment
WHERE id = @id;",
            parameters =>
            {
                parameters.AddWithValue("id", currentChara);
            },
            async (reader) =>
            {
                await message.Channel.SendMessageAsync(
                    $"{currentChara}\r\n" +
                    "●リソース\r\n" +
                    $"【HP】{reader.GetInt16(4)}/{reader.GetInt16(0)}【SP】{reader.GetInt16(5)}/{reader.GetInt16(1)}\r\n" +
                    $"【SAN】{reader.GetInt16(6)}/{reader.GetInt16(2)}【MP】{reader.GetInt16(7)}/{reader.GetInt16(3)}");
            },
            async () =>
            {
                await message.Channel.SendMessageAsync(
                    $"{currentChara}\r\n" +
                    "●リソース\r\n" +
                    $"【HP】0/0【SP】0/0\r\n" +
                    $"【SAN】0/0【MP】0/0");
            });
    }

    private async Task DisplayResource(string currentChara, EquipmentStatus status, SocketMessage message)
    {
        await message.Channel.SendMessageAsync(
                $"{currentChara}\r\n" +
                "●リソース\r\n" +
                $"【HP】{status.Hp}/{status.MaxHp}【SP】{status.Sp}/{status.MaxSp}\r\n" +
                $"【SAN】{status.San}/{status.MaxSan}【MP】{status.Mp}/{status.MaxMp}");
    }
}
