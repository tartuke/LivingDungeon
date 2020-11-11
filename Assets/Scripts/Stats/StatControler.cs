using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[DisallowMultipleComponent]
public class StatControler : MonoBehaviour
{
    private readonly Dictionary<StatType, CharacterStat> stats;

    public readonly ReadOnlyDictionary<StatType, CharacterStat> Stats;

    private readonly List<IOverTime> ReacuringActions;

    public BaseStats baseStats;

    public StatControler()
    {
        stats = new Dictionary<StatType, CharacterStat>();
        Stats = new ReadOnlyDictionary<StatType, CharacterStat>(stats);

        ReacuringActions = new List<IOverTime>();
    }

    public void Start()
    {
        foreach (BaseStats.statinfo info in baseStats.Stats)
        {
            CharacterStat stat = NewStatOfType(info.type, info.value);
            AddStat(stat);
        }
    }

    private void Update()
    {
        foreach (IOverTime action in ReacuringActions)
            action.ReacuringAction();
    }


    public bool AddStat(CharacterStat stat)
    {
        if (Stats.ContainsKey(stat.statType)) return false;

        stats[stat.statType] = stat;
        //Debug.Log(stat.statType);
        OnInsertion();

        return true;
    }


    public void AddModifier(StatModifier mod)
    {
        CharacterStat stat;
        if ((stat = GetStatOfType(mod.StatType)) == null)
        {
            stat = NewStatOfType(mod.StatType, 0);
            AddStat(stat);
        }
        stat.AddModifier(mod);
    }

    public void RemoveModifier(StatModifier mod)
    {
        CharacterStat stat;
        if ((stat = GetStatOfType(mod.StatType)) != null)
        {
            stat.RemoveModifier(mod);
        }
    }

    public void RemoveAllModifiersFromSource(object source)
    {
        foreach (CharacterStat stat in stats.Values)
            stat.RemoveAllModifiersFromSource(source);
    }

    public CharacterStat GetStatOfType(StatType statType)
    {
        CharacterStat stat;
        return (Stats.TryGetValue(statType, out stat)) ? stat : null;
    }

    public CharacterStat NewStatOfType(StatType statType, int value)
    {
        return Activator.CreateInstance(Type.GetType(statType.ToString()), args: value) as CharacterStat;
    }

    public bool LinkStat(EffectsStat stat)
    {
        var link = stat.EffectedStat = GetStatOfType(stat.EffectedStatType);

        return link != null;
    }

    public void OnInsertion()
    {
        ReacuringActions.Clear();
        foreach (CharacterStat stat in Stats.Values)
        {
            if (stat is EffectsStat stat1)
                LinkStat(stat1);
            if (typeof(IOverTime).IsAssignableFrom(stat.GetType()))
                ReacuringActions.Add((IOverTime)stat);
        }
    }
}
