using System;
using UnityEngine;

/*
 * This is where stat types are made
 */

public enum StatType
{
    Invalid = -1,

    HealthStat,
    ArmorStat,
    DamageStat,
    HealthRegenStat,
    StrengthStat,
    SpeedStat,
    ManaStat,
    ManaRegenStat,

    Count
}

[Serializable]
public abstract class ConsumableStat : CharacterStat
{
    private float _CurrentValue;

    public float CurrentValue
    {
        get { return _CurrentValue; }
        set {
            _CurrentValue = (value < 0) ? 0 : (value > Value) ? Value : value;
            PrecentValue = CurrentValue / BaseValue;
        }
    }

    public float PrecentValue;

    public delegate void OnStatChange();
    public OnStatChange OnStatChangeCallBack;

    public delegate void OnStatDepleted();
    public OnStatDepleted OnStatDepletedCallBack;

    public ConsumableStat() : base()
    {
        CurrentValue = Value;
    }

    public ConsumableStat(int baseValue) : base(baseValue)
    {
        CurrentValue = Value;
    }

    public void addValue(float mod)
    {
        CurrentValue += mod;

        if (OnStatChangeCallBack != null)
            OnStatChangeCallBack.Invoke();

        if (CurrentValue == 0 && OnStatDepletedCallBack != null)
            OnStatDepletedCallBack.Invoke();
    }
}

[Serializable]
public abstract class EffectsStat : CharacterStat
{
    public CharacterStat EffectedStat;
    public StatType EffectedStatType;

    public EffectsStat(): base(){}

    public EffectsStat(int baseValue) : base(baseValue) { }

    public abstract void EffectStat();
}


public interface IOverTime
{
    void ReacuringAction();
}

[Serializable]
public class HealthStat : ConsumableStat
{


    public HealthStat() : base()
    {
        statType = StatType.HealthStat;
    }

    public HealthStat(int baseValue) : base(baseValue)
    {
        statType = StatType.HealthStat;
    }
}


[Serializable]
public class ArmorStat : CharacterStat
{


    public ArmorStat() : base()
    {
        statType = StatType.ArmorStat;
    }

    public ArmorStat(int baseValue) : base(baseValue)
    {
        statType = StatType.ArmorStat;

    }
}

[Serializable]
public class DamageStat : CharacterStat
{


    public DamageStat() : base()
    {
        statType = StatType.DamageStat;
    }

    public DamageStat(int baseValue) : base(baseValue)
    {
        statType = StatType.DamageStat;
    }
}

[Serializable]
public class HealthRegenStat : EffectsStat, IOverTime
{
    [SerializeField]
    private float timeIncrement = 1;

    public HealthRegenStat() : base()
    {
        statType = StatType.HealthRegenStat;
        EffectedStatType = StatType.HealthStat;
    }

    public HealthRegenStat(int baseValue) : base(baseValue)
    {
        statType = StatType.HealthRegenStat;
        EffectedStatType = StatType.HealthStat;
    }

    public override void EffectStat()
    {
        if (EffectedStat != null)
        {
            ((HealthStat)EffectedStat).addValue((Value / timeIncrement) * Time.fixedDeltaTime);
        }

    }

    public void ReacuringAction()
    {
        EffectStat();
    }
}

[Serializable]
public class StrengthStat : EffectsStat
{
    float multiplyer = 1;
    StatModifier modifier;

    public StrengthStat() : base()
    {
        statType = StatType.StrengthStat;
        EffectedStatType = StatType.DamageStat;

        modifier = new StatModifier(Value * multiplyer, StatModType.Flat, EffectedStatType, this);
    }

    public override void EffectStat()
    {
        if (EffectedStat != null)
            EffectedStat.AddModifier(modifier);
    }

    protected override void OnUpdate()
    {
        if (EffectedStat == null) return;

        Debug.Log(EffectedStat);

        EffectedStat.RemoveModifier(modifier);
        EffectedStat.AddModifier(modifier = new StatModifier(Value * multiplyer, StatModType.Flat, EffectedStatType, this));

        Debug.Log(EffectedStat.Value);
    }
}

[Serializable]
public class SpeedStat : CharacterStat
{
    public SpeedStat() : base()
    {
        statType = StatType.DamageStat;
    }

    public SpeedStat(int baseValue) : base(baseValue)
    {
        statType = StatType.DamageStat;
    }
}

[Serializable]
public class ManaStat : ConsumableStat
{


    public ManaStat() : base()
    {
        statType = StatType.ManaStat;
    }

    public ManaStat(int baseValue) : base(baseValue)
    {
        statType = StatType.ManaStat;
    }
}

[Serializable]
public class ManaRegenStat : EffectsStat, IOverTime
{
    [SerializeField]
    private float timeIncrement = 1;

    public ManaRegenStat() : base()
    {
        statType = StatType.ManaRegenStat;
        EffectedStatType = StatType.ManaStat;
    }

    public ManaRegenStat(int baseValue) : base(baseValue)
    {
        statType = StatType.ManaRegenStat;
        EffectedStatType = StatType.ManaStat;
    }

    public override void EffectStat()
    {
        if (EffectedStat != null)
        {
            ((HealthStat)EffectedStat).addValue((Value / timeIncrement) * Time.fixedDeltaTime);
        }

    }

    public void ReacuringAction()
    {
        EffectStat();
    }
}