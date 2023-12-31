﻿using System.Collections;
using DiscordBotNet.LegendaryBot.Entities.BattleEntities.Characters;

namespace DiscordBotNet.LegendaryBot.StatusEffects;
public class StatusEffectSet : ISet<StatusEffect>
{
    private readonly HashSet<StatusEffect> statusEffects = new HashSet<StatusEffect>();

    public int Count => statusEffects.Count;
    
    /// <summary>
    /// The character who has the status effects
    /// </summary>
    public Character Affected { get;  }
    public bool IsReadOnly => false;
    public StatusEffectSet(Character affected)
    {
        Affected = affected;
    }
/// <summary>
/// if using this, effect resistance of the affected will be ignored
/// </summary>
/// <returns>true if the status effect was successfully added</returns>
    public bool Add(StatusEffect item)
    {
        return Add(item, null);
    }

  

    public void AddRange(IEnumerable<StatusEffect> statusEffectEnumerable)
    {
        foreach (var i in statusEffectEnumerable)
        {
            Add(i);
        }
    }
    /// <param name="effectiveness">the effectiveness of the caster</param>
/// <returns>true if the status effect was successfully added</returns>
    public bool Add(StatusEffect statusEffect,int? effectiveness)
    {
        
        if (Affected.IsDead && !Affected.RevivePending) return false;
        var arrayOfType =
            this.Where(i => i.GetType() == statusEffect.GetType())
                .ToArray();

        if (arrayOfType.Length < statusEffect.MaxStacks)
        {
            bool added = false;
            if (effectiveness is not null && statusEffect.EffectType == StatusEffectType.Debuff)
            {
                var percentToResistance =Affected.Resistance -effectiveness;
                
                if (percentToResistance < 0) percentToResistance = 0;
                if (!BasicFunction.RandomChance((int)percentToResistance))
                {
                    added = statusEffects.Add(statusEffect);
                }
                    
                
            }
            else
            {
                added = statusEffects.Add(statusEffect);
                
            }

            if (added)
            {
                Affected.CurrentBattle.AddAdditionalText($"{statusEffect.Name} has been inflicted on {Affected}!");
            }
            else
            {
                Affected.CurrentBattle.AddAdditionalText($"{Affected} resisted {statusEffect.Name}!");
            }

            return added;
        }
        if (!statusEffect.IsStackable && arrayOfType.Any() && statusEffect.IsRenewable)
        {
            StatusEffect onlyStatus = arrayOfType.First();
            if (statusEffect.Level > onlyStatus.Level)
            {
                onlyStatus.Level = statusEffect.Level;
            }
            if (statusEffect.Duration > onlyStatus.Duration)
            {
                onlyStatus.Duration = statusEffect.Duration;
            }
            onlyStatus.RenewWith(statusEffect);
            Affected.CurrentBattle.AddAdditionalText($"{statusEffect.Name} has been optimized on {Affected}!");


            return true;
        }
        Affected.CurrentBattle.AddAdditionalText($"{Affected} cannot take any more {statusEffect.Name}!");
        return false;
    }



    public void Clear()
    {
        statusEffects.Clear();
    }

    public bool Contains(StatusEffect item)
    {
        return statusEffects.Contains(item);
    }

    public void CopyTo(StatusEffect[] array, int arrayIndex)
    {
        statusEffects.CopyTo(array, arrayIndex);
    }

    public void ExceptWith(IEnumerable<StatusEffect> other)
    {
        statusEffects.ExceptWith(other);
    }

    public IEnumerator<StatusEffect> GetEnumerator()
    {
        return statusEffects.GetEnumerator();
    }

    public void IntersectWith(IEnumerable<StatusEffect> other)
    {
        statusEffects.IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<StatusEffect> other)
    {
        return statusEffects.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<StatusEffect> other)
    {
        return statusEffects.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<StatusEffect> other)
    {
        return statusEffects.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<StatusEffect> other)
    {
        return statusEffects.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<StatusEffect> other)
    {
        return statusEffects.Overlaps(other);
    }
    /// <summary>
    /// Dispells (removes) a debuff from the character
    /// </summary>
    /// <param name="statusEffect">The status effect to remove</param>
    /// <param name="effectiveness">If not null, will do some rng based on effectiveness to see whether or not to dispell debuff</param>
    /// <returns>true if status effect was successfully dispelled</returns>
    public bool Dispell(StatusEffect statusEffect, int? effectiveness = null)
    {
        if (effectiveness is null || statusEffect.EffectType == StatusEffectType.Debuff)
            return statusEffects.Add(statusEffect);
        var percentToResistance = Affected.Resistance - effectiveness;
                
        if (percentToResistance < 0) percentToResistance = 0;
        if (!BasicFunction.RandomChance((int)percentToResistance))
        {
            return statusEffects.Add(statusEffect);
        }
        return false;
        
    }
    public bool Remove(StatusEffect item)
    {
        return statusEffects.Remove(item);
    }

    public bool SetEquals(IEnumerable<StatusEffect> other)
    {
        return statusEffects.SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<StatusEffect> other)
    {
        statusEffects.SymmetricExceptWith(other);
    }

    public void UnionWith(IEnumerable<StatusEffect> other)
    {
        statusEffects.UnionWith(other);
    }

    void ICollection<StatusEffect>.Add(StatusEffect item)
    {
        statusEffects.Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

