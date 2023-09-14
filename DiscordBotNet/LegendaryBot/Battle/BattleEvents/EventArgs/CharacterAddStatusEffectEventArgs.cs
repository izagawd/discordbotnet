﻿using DiscordBotNet.LegendaryBot.Battle.Entities.BattleEntities.Characters;
using DiscordBotNet.LegendaryBot.Battle.StatusEffects;

namespace DiscordBotNet.LegendaryBot.Battle.BattleEvents.EventArgs;

public class CharacterAddStatusEffectEvent : System.EventArgs
{
    public Character CharacterThatCausedTheAdding { get; }
    public Character CharacterToAddTo { get;}
    public bool Succeeded { get;  } 
    public StatusEffect AddedStatusEffect { get; }

    public CharacterAddStatusEffectEvent(Character characterToAddTo, Character characterThatCausedTheAdding,
        StatusEffect addedStatusEffect, bool succeeded = true)
    {
        Succeeded = succeeded;
        AddedStatusEffect = addedStatusEffect;
        CharacterToAddTo = characterToAddTo;
        CharacterThatCausedTheAdding = characterThatCausedTheAdding;
    }
}