using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Utilities;

namespace MynahMoreInfo;

public class GroupCallBuilder
{
    private readonly Dictionary<string, (int, RawDataPool)> _resultMap = new();
    private readonly HashSet<string> _keys = new();

    public Action<Dictionary<string, (int, RawDataPool)>> OnAllOver = null;

    public Action<(int, RawDataPool)> AddAction(string key)
    {
        _keys.Add(key);
        return tuple =>
        {
            _resultMap[key] = tuple;

            if (_keys.Any(s => !_resultMap.ContainsKey(s)))
            {
                return;
            }

            OnAllOver?.Invoke(_resultMap);
        };
    }

}