using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.SpecialEffect.CombatSkill.XiangShu.Attack.JiuHan;

namespace VillagerIdentities;

public class SafeArrayMap : Dictionary<int, short>
{
    private List<int> keys = new();
    private List<short> values = new();

    public void Add(int key, short value)
    {
        var findIndex = keys.ToList().FindIndex(it => it == key);
        if (findIndex >= 0)
        {
            values[findIndex] = value;
        }
        else
        {
            keys.Add(key);
            values.Add(value);
        }
    }

    public new short this[int key]
    {
        get
        {
            var findIndex = keys.ToList().FindIndex(it => it == key);
            if (findIndex >= 0)
            {
                return values[findIndex];
            }

            return default;
        }
        set
        {
            Add(key, value);
        }
    }
}