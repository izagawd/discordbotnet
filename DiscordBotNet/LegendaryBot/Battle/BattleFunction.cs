﻿namespace DiscordBotNet.LegendaryBot.Battle;

public static  class BattleFunction
{
    /// <summary>
    /// use it to get the amount of exp to gain if a chosen level is defeated
    /// </summary>
    public static ulong ExpGainFormula(int lvl)
    {
        double expGain = 0;
        if (lvl > 161) lvl = 161;
        if (lvl < 50)
        {
            expGain = (Math.Pow(lvl, 3) * (100 - lvl)) / 50;
        }
        else if (lvl < 68)
        {
            expGain = (Math.Pow(lvl, 3) * (150 - lvl)) / 100;
        }
        else if (lvl < 98)
        {
            expGain = (Math.Pow(lvl, 3) * ((1911 - (10 * lvl)) / 3)) / 500;
        }
        else 
        {
            expGain = (Math.Pow(lvl, 3) * (160 - lvl)) / 100;
        }

        return (ulong)Math.Floor(expGain / 10) + 1;
    }
    /// <summary>
    /// use it to estimate the maximum tier a level can reach
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static Tier TierByLevel(int level)
    {
        if (level >= 100) return Tier.Diamond;
        if (level >= 60) return Tier.Platinum;
        if (level >= 30) return Tier.Gold;
        if (level >= 16) return Tier.Silver;
        return Tier.Bronze;
    }
    /// <returns>
    /// The amount of exp required for a character to be able to level up depending on their level
    /// </returns>
    public static ulong NextLevelFormula(int level)
    {
        return (ulong)Math.Round(4 * Math.Pow(level, 3) / 5) + 1;
    }
    ///<param name="potentialDamage">The attacker's attack</param>
    /// <param name="potentialDamage">The attacked defense</param>
    /// <returns>
    /// Potential damage dealt based on stats
    /// </returns>
    /// 
    public static double DamageFormula(double potentialDamage, double defense)
    {


        return (potentialDamage * 600) / (300.0 + defense);
    }
    /// <returns>
    /// The user's level based on their stats
    /// </returns>
    /// <param name="stats">The stats of the character</param>


    public static ElementalAdvantage GetAdvantageLevel(Element a, Element b)
    {

        if (a == Element.Fire && b == Element.Ice) return ElementalAdvantage.Disadvantage;
        if (a == Element.Fire && b == Element.Earth) return ElementalAdvantage.Advantage;
        if (a == Element.Ice && b == Element.Earth) return ElementalAdvantage.Disadvantage;
        if (a == Element.Ice && b == Element.Fire) return ElementalAdvantage.Advantage;
        if (a == Element.Earth && b == Element.Fire) return ElementalAdvantage.Disadvantage;
        if (a == Element.Earth && b == Element.Ice) return ElementalAdvantage.Advantage;
        return ElementalAdvantage.Neutral;
    }
    /// <returns>
    /// The total amount of EXP a character has used based on their level
    /// </returns>
    public static ulong TotalExp(int level)
    {
            
        ulong totalExperience = 0;
        while (level > 1)
        {
            totalExperience += NextLevelFormula(level);

            level -= 1;
        }
        return totalExperience;
    }




}