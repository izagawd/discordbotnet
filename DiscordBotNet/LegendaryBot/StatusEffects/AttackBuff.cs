﻿using DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;
using DiscordBotNet.LegendaryBot.ModifierInterfaces;

namespace DiscordBotNet.LegendaryBot.StatusEffects;

public class AttackBuff : StatusEffect, IStatsModifier
{
    public override bool IsRenewable => true;

    public override string Description { get; } = "Increases the caster's attack by 50%";

    public override bool HasLevels => true;
    public override int MaxStacks => 1;
    public override int MaxLevel => 3;
    public override StatusEffectType EffectType => StatusEffectType.Buff;

    public AttackBuff(Character caster) : base(caster)
    {

    }

    public float AttackPercentage
    {
        get
        {
            switch (Level)
            {
                case 1:
                    return 30;
                case 2:
                    return 50;
                default:
                    return 70;
            }
        }
    }


    public IEnumerable<StatsModifierArgs> GetAllStatsModifierArgs(Character owner)
    {
        return new StatsModifierArgs[]
        {
            new AttackPercentageModifierArgs
            {
                CharacterToAffect = owner,
                ValueToChangeWith = AttackPercentage,
                WorksAfterGearCalculation = true
            }
        };
    }
}