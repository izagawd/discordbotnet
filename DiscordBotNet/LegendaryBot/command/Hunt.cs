﻿using DiscordBotNet.Extensions;
using DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBotNet.LegendaryBot.command;

public class Hunt : BaseCommandClass
{
    public override BotCommandType BotCommandType { get; } = BotCommandType.Battle;

    [SlashCommand("hunt", "Hunt for mobs to get materials and be stronger!")]
    public async Task Execute(InteractionContext ctx,
        [Option("mob_name", "The name of the mob you want to hunt")] string characterName )
    {
        var author = ctx.User;

  
        var userData = await DatabaseContext.UserData
            .IncludeTeamWithAllEquipments()
            .FindOrCreateAsync((long)author.Id);

        var embedToBuild = new DiscordEmbedBuilder()
            .WithUser(author)
            .WithTitle("Hmm")
            .WithColor(userData.Color)
            .WithDescription($"You cannot hunt because you have not yet become an adventurer with {Tier.Unranked}");
        if (userData.IsOccupied)
        {
            embedToBuild
                .WithDescription("You are occupied!");
            await ctx.CreateResponseAsync(embedToBuild);
            return;
        }

        if (userData.Tier == Tier.Unranked)
        {
            await ctx.CreateResponseAsync(embedToBuild.Build());
            return;
        }
        
        var characterType = Character.CharacterTypes.FirstOrDefault(
            i =>  i.Name.ToLower() == characterName.ToLower().Replace(" ", "") && i.IsSubclassOf(typeof(Character)) && !i.IsRelatedToType(typeof(Player)));
        if (characterType is null)
        {
            embedToBuild =
                embedToBuild.WithDescription($"Mob {characterName} does not exist!");
            await ctx.CreateResponseAsync(embedToBuild.Build());
            return;
        }

        await MakeOccupiedAsync(userData);
        Character enemy =(Character) Activator.CreateInstance(characterType)!;

        embedToBuild = embedToBuild
            .WithTitle($"Keep your guard up!")
            .WithDescription($"A wild {enemy} has appeared!");
        await ctx.CreateResponseAsync(embedToBuild.Build());
        var message = await ctx.GetOriginalResponseAsync();
        var userTeam = await userData.EquippedPlayerTeam.LoadAsync(author);
        enemy.SetLevel(userTeam.Select(i => i.Level).Average().Round());
        var simulator = new BattleSimulator(userTeam, await new CharacterTeam(enemy).LoadAsync());

 

        var battleResult = await simulator.StartAsync(message);



        var expToGain = BattleFunction.ExpGainFormula(enemy.Level);
        if (battleResult.Winners != userTeam)
        {
            expToGain /= 2;
        }
        string expGainText = userTeam.IncreaseExp(expToGain);
        
        if (battleResult.Winners == userTeam)
        {
            var rewardText = userData.ReceiveRewards(author.Username, battleResult.BattleRewards);
            embedToBuild
                .WithTitle($"Nice going bud!")
                .WithDescription("You won!\n" + expGainText  +$"\n{rewardText}")
                .WithImageUrl("");

            await message.ModifyAsync(new DiscordMessageBuilder(){Embed = embedToBuild.Build() });
        }
        else
        {
            string additionalString = "";
            if (battleResult.TimedOut is not null)
                additionalString += "timed out\n";
            if (battleResult.Forfeited is not null)
                additionalString += "forfeited";
            
            embedToBuild
                .WithTitle($"Ah, too bad\n"+additionalString)
                .WithDescription($"You lost boii\n"+expGainText);
            await message.ModifyAsync(new DiscordMessageBuilder(){Embed = embedToBuild.Build()});
            
        }

        await DatabaseContext.SaveChangesAsync();

    }
}