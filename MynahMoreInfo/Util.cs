﻿using FrameWork;
using UnityEngine;

namespace MynahMoreInfo;

public class Util
{
    public static string GetSpriteStr(string spriteName)
    {
        // return TMPTextSpriteHelper.GetStringWithTextSpriteTag(spriteName);
        return $"<sprite=\"mmiSprites\" name=\"{spriteName}\">";
    }
    
    public static MouseTipDisplayer EnsureMouseTipDisplayer(GameObject obj)
    {
        var mouseTipDisplayer = obj.GetComponent<MouseTipDisplayer>();
        if (mouseTipDisplayer != null) return mouseTipDisplayer;
        obj.AddComponent<MouseTipDisplayer>();
        mouseTipDisplayer = obj.GetComponent<MouseTipDisplayer>();

        return mouseTipDisplayer;
    }

    public static void EnableMouseTipCharacter(MouseTipDisplayer mouseTipDisplayer, int characterId, int type = 1)
    {
        if (type == 1)
        {
            mouseTipDisplayer.Type = TipType.Character;
            if (mouseTipDisplayer.RuntimeParam == null)
            {
                mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
                mouseTipDisplayer.RuntimeParam.Clear();
            }
            // var item = Character.Instance.GetItem(Character.Instance[charDisplayData.TemplateId].TemplateId);

            mouseTipDisplayer.RuntimeParam.Set("charId", characterId);
            mouseTipDisplayer.RuntimeParam.Set("_mmi_no_replace", true);
            mouseTipDisplayer.enabled = true;
        }
        else
        {
            mouseTipDisplayer.Type = TipType.SimpleWide;
            if (mouseTipDisplayer.RuntimeParam == null)
            {
                mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
                mouseTipDisplayer.RuntimeParam.Clear();
            }
            
            mouseTipDisplayer.RuntimeParam.Set("_mmi_charId", characterId);
            mouseTipDisplayer.enabled = true;
        }
    }
}