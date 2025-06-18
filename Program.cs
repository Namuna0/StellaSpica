﻿using Discord;
using Discord.WebSocket;
using dotenv.net;
using MathNet.Numerics.Random;

partial class Program
{
    private DiscordSocketClient? _client;

    private Dictionary<ulong, string> _currentCharaDic = new Dictionary<ulong, string>();

    private MersenneTwister _ms = new MersenneTwister();

    static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        await ConnectServer();

        await Task.Delay(-1); // 永久に実行
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task ConnectServer()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
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

        //await Create();
        await Start();
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        if (message is SocketUserMessage userMessage && message.Channel is SocketGuildChannel guildChannel)
        {
            var guild = guildChannel.Guild;
            var user = guild.GetUser(message.Author.Id);
            var content = message.Content;

            if (content.StartsWith("?show data ")) await ShowData(message, guild, user);
            else if (content.StartsWith("?set data")) await SetData(message, guild, user);

            // キャラクター
            else if (content.StartsWith("?login ")) await Login(message, guild, user);
            else if (content == "?auto") await Auto(message, guild, user);

            // 装備
            else if (content == "?show res") await ShowRes(message, guild, user);
            else if (content.StartsWith("?set res ")) await SetRes(message, guild, user);
            else if (content == "?reset res") await ResetRes(message, guild, user);
            else if (content.StartsWith("?hp ")) await UpdateRes("hp", message, guild, user);
            else if (content.StartsWith("?sp ")) await UpdateRes("sp", message, guild, user);
            else if (content.StartsWith("?san ")) await UpdateRes("san", message, guild, user);
            else if (content.StartsWith("?mp ")) await UpdateRes("mp", message, guild, user);

            // バトル
            else if (content == "?r") await SimpleRoll(message, guild, user);
            else if (content.StartsWith("?r ")) await DiceRoll(message, guild, user);
            else if (content.StartsWith("?rr ")) await DiceMultiRoll(message, guild, user);
            else if (content.StartsWith("?p ")) await PickRoll(message, guild, user);
            else if (content.StartsWith("?pp ")) await PickMultiRoll(message, guild, user);

            // 採取
            else if (content.StartsWith("?gather ")) await Gather(message, guild, user);
        }
    }

    public async Task Command(string[] texts, long flag, SocketMessage message, SocketGuildUser user, Func<string, Task> onCompleted)
    {
        if (!GetPaseFlag(texts, flag))
        {
            await message.Channel.SendMessageAsync("引数が変です。");
            return;
        }

        if (!_currentCharaDic.TryGetValue(user.Id, out var currentChara))
        {
            await message.Channel.SendMessageAsync("「?login [キャラクター名]」を呼んでください。");
            return;
        }

        await onCompleted.Invoke(currentChara);
    }

    private bool GetPaseFlag(string[] texts, long target)
    {
        if (texts.Length != target.ToString().Length)
        {
            return false;
        }

        long digit = 1;
        foreach (string text in texts)
        {
            int flag = 0;

            if (int.TryParse(text, out _)) flag = 1;
            else if (float.TryParse(text, out _)) flag = 2;
            else flag = 3;

            long num = target % (digit * 10) / digit;
            if (flag > num) return false;

            digit *= 10;
        }

        return true;
    }

}
