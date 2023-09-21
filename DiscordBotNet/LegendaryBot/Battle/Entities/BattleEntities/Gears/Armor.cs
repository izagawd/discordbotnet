﻿using DiscordBotNet.LegendaryBot.Battle.Stats;

namespace DiscordBotNet.LegendaryBot.Battle.Entities.Gears;

public class Armor : Gear
{ 
    public sealed override IEnumerable<Type> PossibleMainStats { get; } = new[] { GearStat.DefenseFlatType };
}