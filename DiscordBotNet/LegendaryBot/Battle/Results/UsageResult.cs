﻿using DiscordBotNet.LegendaryBot.Battle.Entities.BattleEntities.Characters;
using DiscordBotNet.LegendaryBot.Battle.Moves;

namespace DiscordBotNet.LegendaryBot.Battle.Results;

public class UsageResult
{
    /// <summary>
    /// If not null, this text will be used as the main text
    /// </summary>
    public string? Text { get; set; }

    public bool IsAttackUsage => DamageResults.Count > 0;
    public bool IsNonAttackUsage => !IsAttackUsage;

    /// <summary>
    /// if the skill deals any sort of damage, the damage results should be in this list
    /// </summary>
    public List<DamageResult> DamageResults { get; set; } = new List<DamageResult>();
    /// <summary>
    /// The character who used the skill/item
    /// </summary>
    public Character User { get; set; }
    /// <summary>
    /// Determines if the usage was from a normal skill use or a follow up attack.  this must not be null
    /// </summary>
    public UsageType UsageType { get; set; }
    /// <summary>
    /// The move used to execute this skill
    /// </summary>
    public Move MoveUsed { get; set; }
    public UsageResult(string text, UsageType usageType) : this(usageType)
    {
        
        Text = text;
    }

    public UsageResult(UsageType usageType)
    {
        UsageType = usageType;
    }
}