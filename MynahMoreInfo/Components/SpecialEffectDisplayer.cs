using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace MynahMoreInfo.Components;

public class SpecialEffectDisplayer : MonoBehaviour
{
    public string directEffect;
    public string reverseEffect;
    
    public string directEffect1;
    public string reverseEffect1;
    
    private bool _isAltDown;

    private bool _isBook;

    private TextMeshProUGUI txtDirect, txtReverse;

    public bool IsAltDown
    {
        set
        {
            if (_isAltDown == value) return;
            _isAltDown = value;
            UpdateText();
        }
    }

    public bool ShowDiffed => (CurrentAltDown && ModEntry.HintEffectDiff == 1) || (ModEntry.HintEffectDiff == 2);
    public static bool CurrentAltDown => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    
    private void OnEnable()
    {
        txtDirect = transform.Find("DirectDesc/DirectEffectDesc").GetComponent<TextMeshProUGUI>();
        txtReverse = transform.Find("ReverseDesc/ReverseEffectDesc").GetComponent<TextMeshProUGUI>();

        _isBook = transform.parent.parent.GetComponent<MouseTipBook>() != null;

        UpdateText();
    }

    public void UpdateText()
    {
        UpdateSpecialEffectText(txtDirect, ShowDiffed ? directEffect1 : directEffect);
        UpdateSpecialEffectText(txtReverse, ShowDiffed ? reverseEffect1 : reverseEffect);
    }

    private void Update()
    {
        IsAltDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }
    
    /**
     * 从 MouseTipCombatSkill#UpdateSpecialEffectText 拿来改的
     */
    public void UpdateSpecialEffectText(TextMeshProUGUI effectText, string effectStr)
    {
        if(effectText == null) return;
        // effectStr = "     " + effectStr;
        var x = effectText.rectTransform.sizeDelta.x;

        if (_isBook)
        {
            x = 640;
        } 
        
        var preferredValues = effectText.GetPreferredValues(effectStr, x, float.PositiveInfinity);
        effectText.rectTransform.sizeDelta = preferredValues.SetX(x);
        effectText.text = effectStr;
    }

}