﻿using DSharpPlus.Entities;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;
using DiscordBotNet.Database;
using DiscordBotNet.Database.Models;
using DiscordBotNet.LegendaryBot;
using DiscordBotNet.LegendaryBot.Battle;
using DiscordBotNet.LegendaryBot.Battle.Entities;
using DiscordBotNet.LegendaryBot.Battle.Entities.BattleEntities.Blessings;
using DiscordBotNet.LegendaryBot.Battle.Entities.BattleEntities.Characters;
using DiscordBotNet.LegendaryBot.Battle.Entities.Gears;
using DiscordBotNet.LegendaryBot.command;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using DSharpPlus.VoiceNext;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ConfigurationManager = System.Configuration.ConfigurationManager;


namespace DiscordBotNet;



public class Bot
{
    
    public static BaseCommandClass[]? CommandArray;
    public static readonly Type[] AllAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes().ToArray();


    private static void Main(string[] args) => new Bot().RunBotAsync(args).GetAwaiter().GetResult();

    public static SlashCommandsExtension SlashCommandsExtension { get; protected set; }
    /// <summary>
    /// This is my discord user Id because it's too long to memorize
    /// </summary>
    public static ulong Izasid => 216230858783326209;
    /// <summary>
    /// this is the discord user Id of another account of mine that i use to test stuff
    /// </summary>
    public static ulong Testersid => 266157684380663809;
    public static DiscordClient Client { get; private set; }

    private async Task DoShit()
    {
        var ctx = new PostgreSqlContext();
        var iza = await ctx.UserData.FindOrCreateAsync(Izasid);
        await ctx.SaveChangesAsync();
        iza.Inventory.AddRange(new Lily() *4);
        await ctx.SaveChangesAsync();
    }
    /// <summary>
    /// this is where the program starts
    /// </summary>
    private async Task RunBotAsync(string[] args)
    {

        await DoShit();
        var commandArrayType = AllAssemblyTypes.Where(t =>  t.IsSubclassOf(typeof(BaseCommandClass))).ToArray();

        Console.WriteLine("Entity images loading...");
        var stopwatch = new Stopwatch(); stopwatch.Start();
        var imagesLoaded = await BasicFunction.LoadEntityImagesAsync(); 
        stopwatch.Stop(); 
        Console.WriteLine($"Took a total of {stopwatch.Elapsed.TotalMilliseconds}ms to load and cache {imagesLoaded} entity images");
        stopwatch.Reset();
        Console.WriteLine("Making all users unoccupied...");
        stopwatch.Start();
        var ctx = new PostgreSqlContext();
    
        
        await ctx.UserData.ForEachAsync(i => i.IsOccupied = false);
        var count = await ctx.UserData.CountAsync();
        await ctx.SaveChangesAsync();
        await ctx.DisposeAsync();
        stopwatch.Stop();
        Console.WriteLine($"Took a total of {stopwatch.Elapsed.TotalMilliseconds}ms to make {count} users unoccupied");
        
        CommandArray = Array.ConvertAll(commandArrayType, element => (BaseCommandClass)Activator.CreateInstance(element)!)!;
        var config = new DiscordConfiguration
        {
            Token = ConfigurationManager.AppSettings["BotToken"]!,
            Intents = DiscordIntents.All,
            AutoReconnect = true,
            
            
        };
        
        var client = new DiscordClient(config);
        Client = client;
        SlashCommandsExtension = client.UseSlashCommands();
        
        SlashCommandsExtension.RegisterCommands(Assembly.GetExecutingAssembly());
        
        SlashCommandsExtension.SlashCommandErrored += OnSlashCommandError;
        client.UseVoiceNext(new VoiceNextConfiguration { AudioFormat = AudioFormat.Default});
        var interactivityConfiguration = new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromMinutes(2),
        };
        client.UseInteractivity(interactivityConfiguration);
        client.SocketOpened += OnReady;
        await client.ConnectAsync();

        await Website.Start(args);
        
    }


    private async  Task OnSlashCommandError(SlashCommandsExtension extension,SlashCommandErrorEventArgs ev)
    {
        Console.WriteLine(ev.Exception);
        var databaseContext = new PostgreSqlContext();
        var involvedUsers = new List<DiscordUser>();
        involvedUsers.Add(ev.Context.User);
        if (ev.Context.ResolvedUserMentions is not null)
            involvedUsers.AddRange(ev.Context.ResolvedUserMentions);
        var involvedIds = involvedUsers.Select(i => i.Id).ToArray();
        await databaseContext.UserData.Where(i => involvedIds.Contains(i.Id))
            .ForEachAsync(i =>
                i.IsOccupied = false);
        var color = await databaseContext.UserData.FindOrCreateSelectAsync(ev.Context.User.Id, i => i.Color);
        await databaseContext.SaveChangesAsync();

        await databaseContext.DisposeAsync();

        await ev.Context.Channel.SendMessageAsync(new DiscordEmbedBuilder()
            .WithColor(color)
            .WithTitle("hmm")
            .WithDescription("Something went wrong"));
    }
    

    private Task OnReady(DiscordClient client, SocketEventArgs e)
    {

        Console.WriteLine("Ready!");
        return Task.CompletedTask;
    }
}