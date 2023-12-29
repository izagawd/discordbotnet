﻿using DiscordBotNet.Extensions;
using DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;

namespace DiscordBotNet.LegendaryBot.Stats;

public class CriticalChanceGearStat : GearStat
{
    public override int GetMainStat(Rarity rarity, int level)
    {

        return (((int) rarity + 1) * (level / 15.0) * 9).Round() + 10;
    }

    public override void AddStats(Character character)
    {
        character.TotalCriticalChance += Value;
    }

    public override int GetMaximumSubstatLevelIncrease(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.OneStar:
                return 3;
            case Rarity.TwoStar:
            case Rarity.ThreeStar:
                return 3;
            case Rarity.FourStar:
                return 4;
            default:
                return 6;
        }
    }

    public override int GetMinimumSubstatLevelIncrease(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.OneStar:
                return 1;
            case Rarity.TwoStar:
            case Rarity.ThreeStar:
            case Rarity.FourStar:
                return 3;
            default:
                return 3;
        }
    }
}