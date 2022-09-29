using FrameWork;
using UnityEngine;

namespace MynahMoreInfo;

public class Util
{
    public static MouseTipDisplayer EnsureMouseTipDisplayer(GameObject obj)
    {
        var mouseTipDisplayer = obj.GetComponent<MouseTipDisplayer>();
        if (mouseTipDisplayer == null)
        {
            obj.AddComponent<MouseTipDisplayer>();
            mouseTipDisplayer = obj.GetComponent<MouseTipDisplayer>();
        }

        return mouseTipDisplayer;
    }

    public static void EnableMouseTipCharacter(MouseTipDisplayer mouseTipDisplayer, int characterId)
    {
        mouseTipDisplayer.Type = TipType.Character;
        if (mouseTipDisplayer.RuntimeParam == null)
        {
            mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
            mouseTipDisplayer.RuntimeParam.Clear();
        }
        // var item = Character.Instance.GetItem(Character.Instance[charDisplayData.TemplateId].TemplateId);

        mouseTipDisplayer.RuntimeParam.Set("charId", characterId);
        mouseTipDisplayer.enabled = true;
    }
}