using System;
using System.Collections.Generic;
using GameData.Utilities;

namespace MynahMoreInfo;

public class GroupCallBuilder
{
    private Dictionary<string, (int, RawDataPool)> resultMap = new();
    private HashSet<string> keys = new();

    public Action<Dictionary<string, (int, RawDataPool)>> OnAllOver = null;

    Action<(int, RawDataPool)> AddAction(string key)
    {
        keys.Add(key);
        return tuple =>
        {
            resultMap[key] = tuple;

            foreach (var s in keys)
            {
                if(!resultMap.ContainsKey(s)) return;
            }

            OnAllOver?.Invoke(resultMap);
        };
    }

}