﻿using System.ComponentModel.DataAnnotations.Schema;
using DiscordBotNet.LegendaryBot.BattleEvents;
using DiscordBotNet.LegendaryBot.BattleEvents.EventArgs;
using DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;
using DiscordBotNet.LegendaryBot.Results;

namespace DiscordBotNet.LegendaryBot.Entities.BattleEntities;

public abstract class BattleEntity : Entity, ICanBeLeveledUp, IBattleEventListener
{
    
    public virtual int Level { get; protected set; } = 1;
    [NotMapped]
    public virtual int MaxLevel { get; }
    public virtual ExperienceGainResult  IncreaseExp(long experience)
    {
        return new ExperienceGainResult();
    }

    public virtual long GetRequiredExperienceToNextLevel(int level)
    {
        return BattleFunction.NextLevelFormula(level);
    }
    public long GetRequiredExperienceToNextLevel()
    {
        return GetRequiredExperienceToNextLevel(Level);
    }
    public long Experience
    {
        get;
        protected set;
    }

    [NotMapped] public virtual Rarity Rarity { get; protected set; } = Rarity.OneStar;
    public virtual void OnBattleEvent(BattleEventArgs eventArgs, Character owner)
    {
       
    }
}