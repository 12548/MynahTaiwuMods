using System;
using UnityEngine;

namespace ItemSubtypeFilter;

[RequireComponent(typeof(ItemSortAndFilter))]
public class ReadStateFilterComponent: MonoBehaviour
{
    private ItemSortAndFilter _sortAndFilter;
    private CToggleGroup _toggleGroup;

    private int _key = 0;

    public CToggleGroup ToggleGroup
    {
        set
        {
            _toggleGroup = value;
            _toggleGroup.OnActiveToggleChange = (toggle, cToggle) =>
            {
                _key = toggle.Key;
            };
        }
        get => _toggleGroup;
    }

    private void Start()
    {
        _sortAndFilter = GetComponent<ItemSortAndFilter>();
    }

    private void OnBecameInvisible()
    {
        throw new NotImplementedException();
    }

    private void OnBecameVisible()
    {
        throw new NotImplementedException();
    }
}