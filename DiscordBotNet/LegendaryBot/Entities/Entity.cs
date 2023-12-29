﻿using System.ComponentModel.DataAnnotations.Schema;
using DiscordBotNet.Database.Models;
using SixLabors.ImageSharp.PixelFormats;

namespace DiscordBotNet.LegendaryBot.Entities;

public abstract class Entity : ICloneable
{
    public DateTime DateAcquired { get; set; } = DateTime.UtcNow;
    public override string ToString()
    {
        return Name;
    }
    public virtual Task<Image<Rgba32>> GetDetailsImageAsync()
    {
        return Task.FromResult(new Image<Rgba32>(100,100));

    }

    public virtual Task LoadAsync() => Task.CompletedTask;
    public object Clone()
    {
        var clone =(Entity) MemberwiseClone();
        clone.Id = Guid.NewGuid();
        return clone;
    }

    [NotMapped]
    public virtual string Name
    {
        get => BasicFunction.Englishify(GetType().Name);
        protected set {}
    }
    public static IEnumerable<Entity> operator *(Entity a, int number)
    {
        if (number <= 0)
        {
            throw new Exception("Entity times a negative number or 0 doesn't make sense");
            
        }

        List<Entity> clonedEntities = [];
        foreach (var i in Enumerable.Range(0, number))
        {
            clonedEntities.Add((Entity) a.Clone());
        }

        return clonedEntities;
    }
    public UserData? UserData { get; set; }

    [NotMapped]
    public virtual string IconUrl { get; protected set; }

    public Guid Id { get; protected set; } = Guid.NewGuid();
    public ulong UserDataId { get; set; } 
}