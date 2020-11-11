
public enum StatModType
{
    Flat,
    PercentAdd,
    PercentMult,
}


public class StatModifier
{
    public readonly float Value;
    public readonly StatModType Type;
    public readonly StatType StatType;
    public readonly int Order;
    public readonly object Source;

    public StatModifier(float value, StatModType type, StatType statType, int order, object source)
    {
        Value = value;
        Type = type;
        StatType = statType;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModType type, StatType statType) : this(value, type, statType, (int)type, null) { }

    public StatModifier(float value, StatModType type, StatType statType, int order) : this(value, type, statType, order, null) { }

    public StatModifier(float value, StatModType type, StatType statType, object source) : this(value, type, statType, (int)type, source) { }
}