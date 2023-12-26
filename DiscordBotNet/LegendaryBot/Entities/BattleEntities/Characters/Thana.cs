﻿using DiscordBotNet.Extensions;
using DiscordBotNet.LegendaryBot.Moves;
using DiscordBotNet.LegendaryBot.Results;
using DiscordBotNet.LegendaryBot.StatusEffects;
using DSharpPlus.Entities;

namespace DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;
public class SoulAttack : BasicAttack
{
    public override string GetDescription(Character character) => "Uses the souls of the dead to attack, with a 25% chance to sleep!";
    protected override UsageResult HiddenUtilize(Character owner, Character target, UsageType usageType)
    {
        var damageResult = target.Damage(new DamageArgs(this)
        {
            Damage = owner.Attack * 1.7,
            Caster = owner,
            DamageText = $"{owner} uses the souls of the dead to attack {target} and dealt $ damage!"
        });
        if (BasicFunction.RandomChance(25))
        {
            target.StatusEffects.Add(new Sleep(owner));
        }
        return new UsageResult(this)
        {
            TargetType = TargetType.SingleTarget,
            User = owner,
            UsageType = usageType,
            DamageResults = new []{damageResult}
        };
    }
}

public class YourLifeEnergyIsMine : Skill
{


    public override string GetDescription(Character character) => "Sucks the life energy out of the enemy, recovering 20% of damage dealt as hp";
    public override IEnumerable<Character> GetPossibleTargets(Character owner)
    {
        return owner.CurrentBattle.Characters.Where(i => i.Team != owner.Team && !i.IsDead);
    }

    protected override UsageResult HiddenUtilize(Character owner, Character target, UsageType usageType)
    {
        var damageResult = target.Damage(new DamageArgs(this)
        {
            Damage = owner.Attack * 2.5,
            Caster = owner,
            DamageText = $"{owner} sucks the life essence out of {target} and deals $ damage!"

        });
        owner.RecoverHealth(damageResult.Damage * 0.2);
        return new UsageResult(this)
        {
            DamageResults = new[]
            {
                damageResult
            },
            Text = "Your lifespan is mine!",
            User = owner,
            TargetType = TargetType.SingleTarget,
            UsageType = usageType
        };
    }

    public override int MaxCooldown { get; } = 3;
}
public class Arise : Surge
{

    public override int MaxCooldown =>6;

    public override string GetDescription(Character character) =>
        $"Grants all allies immortality, increases the caster's attack for 2 turns, and grants her an extra turn";
    public override IEnumerable<Character> GetPossibleTargets(Character owner)
    {
        return owner.Team;
    }
    
    protected override UsageResult HiddenUtilize(Character owner, Character target, UsageType usageType)
    {
        owner.CurrentBattle.AdditionalTexts.Add($"With her necromancy powers, {owner} attempts to bring back all her dead allies!");

        foreach (var i in GetPossibleTargets(owner))
        {
            if(i.IsDead)
                i.Revive();
            var duration = 1;
            if (i == owner)
            {
                duration = 3;
            }
            i.StatusEffects.Add(new Immortality(owner){Duration = duration});
        }
        owner.StatusEffects.Add(new AttackBuff(owner) { Duration = 3 });
        owner.GrantExtraTurn();
        return new UsageResult(this)
        {
            User = owner,
            TargetType = TargetType.AOE,
            Text = "Necromancy!",
            UsageType = usageType
        };
    }
}
public class Thana : Character
{
    public override Rarity Rarity { get; protected set; } = Rarity.FiveStar;
    public override DiscordColor Color { get; protected set; } = DiscordColor.Brown;
    public override int BaseMaxHealth => 1500 + (60 * Level);
    public override int BaseAttack => (100 + (10 * Level));
    public override int BaseDefense => (100 + (6.2 * Level)).Round();
    public override int BaseSpeed => 120;
    public override Element Element { get; protected set; } = Element.Earth;
    public override BasicAttack BasicAttack { get; } = new SoulAttack();

    public override Skill? Skill { get; } = new YourLifeEnergyIsMine();
    public override Surge? Surge { get; } = new Arise();
}