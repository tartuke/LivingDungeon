using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;



[CreateAssetMenu(fileName = "New Base Stats", menuName = "Stats/Base Stats")]
public class BaseStats : ScriptableObject
{
    public statinfo[] Stats;

    [Serializable]
    public class statinfo
    {
        public StatType type;
        public int value;
    }
}
