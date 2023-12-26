﻿using DiscordBotNet.Database;
using DiscordBotNet.Database.Models;
using DiscordBotNet.Extensions;
using DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;
using DiscordBotNet.LegendaryBot.Entities.Items.Shards;
using DiscordBotNet.LegendaryBot.Rewards;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBotNet.LegendaryBot.Quests;

public class KillSlimesQuest : Quest
{
    public override string Description => "Simple quest. Defeat the slimes that are attacking a village";

    public override async  Task<bool> StartQuest(InteractionContext context,DiscordMessage? message = null)
    {
        var slimeTeam = new CharacterTeam([new Slime(),new Slime(),new Slime(), new Slime()]);
        var postgre = new PostgreSqlContext();
        var userData = await postgre.UserData
            .IncludeTeamWithAllEquipments()
            .FindOrCreateAsync(UserDataId);
        var embed = new DiscordEmbedBuilder()
            .WithUser(context.User)
            .WithColor(userData.Color)
            .WithTitle("Watch out!")
            .WithDescription("A bunch of slimes are attacking!");
        if (message is null)
        {
            message = await context.Channel.SendMessageAsync(embed);
        }
        else
        {
            message = await message.ModifyAsync(new DiscordMessageBuilder {Embed = embed});
        }
        await Task.Delay(2000);
        var playerTeam = await userData.GetCharacterTeam(context.User).LoadAsync();

        var battleSimulator = new BattleSimulator(playerTeam,await slimeTeam.LoadAsync());
        var result = await battleSimulator.StartAsync(context, message);
        QuestRewards = result.BattleRewards;
        return result.Winners == playerTeam;
        
    }

    public override IEnumerable<Reward> QuestRewards { get; protected set; } = [];
}