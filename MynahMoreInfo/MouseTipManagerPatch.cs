using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FrameWork;
using GameData.Domains.Character.Display;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable RedundantAssignment

namespace MynahMoreInfo;

[HarmonyPatch]
public class MouseTipManagerPatch
{

    [HarmonyPrefix, HarmonyPatch(typeof(MouseTipManager), "ShowTips")]
    public static void ShowTipsPrefix(ref TipType type,
        ref ArgumentBox argsBox)
    {
        if (!ModEntry.ReplaceAllCharacterTipToDetail || (type != TipType.Character && type != TipType.LifeCombatSkillValue)) return;
        
        if (argsBox.Get<AvatarRelatedData>("avatar", out _))
        {
            return;
        }
        
        if (argsBox.Get("_mmi_no_replace", out bool _))
        {
            return;
        }
        
        if(!argsBox.Get("charId", out int charId) || charId < 0)
        {
            type = TipType.Character;
            return;
        }
        // Debug.Log("charId: " +  charId);

        type = TipType.SimpleWide;
        argsBox.Set("_mmi_charId", charId);
        argsBox.Set("_mmi_locationShow", type != TipType.LifeCombatSkillValue); // 石屋不显示位置
        // if (argsBox.Get("locationShow", out bool showLocation))
        // {
        //     argsBox.Set("_mmi_locationShow", showLocation);
        // }
    }
    
    /// <summary>
    /// 旧版MouseTipManager#UpdateMouseOverObj
    /// </summary>
    public static IEnumerator UpdateMouseOverObj()
    {
        var __instance = SingletonObject.getInstance<MouseTipManager>();
        var pointerEventData = typeof(MouseTipManager).GetField("_pointerEventData", (BindingFlags)(-1))!;
        var _currMouseOverObj = typeof(MouseTipManager).GetField("_currMouseOverObj", (BindingFlags)(-1))!;
        var _raycastResults = typeof(MouseTipManager).GetField("_raycastResults", (BindingFlags)(-1))!;
        var _currMouseTipDisplayerActive = typeof(MouseTipManager).GetField("_currMouseTipDisplayerActive", (BindingFlags)(-1))!;

        while (true)
        {
            var ____pointerEventData = (PointerEventData)pointerEventData.GetValue(__instance);
            var ____currMouseOverObj = (GameObject)_currMouseOverObj.GetValue(__instance);
            var ____raycastResults = (List<RaycastResult>)_raycastResults.GetValue(__instance);
            var ____currMouseTipDisplayerActive = (bool)_currMouseTipDisplayerActive.GetValue(__instance);

            var screenMousePos = (Vector2) UIManager.Instance.UiCamera.ScreenToViewportPoint(Input.mousePosition);
            var hitObj = (GameObject) null;
            if (screenMousePos.x >= 0.0 && screenMousePos.x <= 1.0 && screenMousePos.y >= 0.0 && screenMousePos.y <= 1.0)
            {
                ____pointerEventData.position = Input.mousePosition;
                EventSystem.current.RaycastAll(____pointerEventData, ____raycastResults);
                if (____raycastResults.Count > 0)
                    hitObj = ____raycastResults[0].gameObject;
            }
            var mouseTipDisplayerActive = hitObj != null && hitObj.GetComponent<MouseTipDisplayer>() != null && hitObj.GetComponent<MouseTipDisplayer>().enabled;
            if (hitObj != ____currMouseOverObj || mouseTipDisplayerActive != ____currMouseTipDisplayerActive)
            {
                if (____currMouseOverObj != null)
                {
                    // Debug.Log($"curr: {_currMouseTipDisplayerActive}");
                }
                if (____currMouseTipDisplayerActive)
                    __instance.HideTips();
                _currMouseOverObj.SetValue(__instance, hitObj);
                _currMouseTipDisplayerActive.SetValue(__instance, mouseTipDisplayerActive && hitObj.GetComponent<MouseTipDisplayer>().ShowTips());
                
                // this._currMouseOverObj = hitObj;
                // this._currMouseTipDisplayerActive = mouseTipDisplayerActive && hitObj.GetComponent<MouseTipDisplayer>().ShowTips();
            }
            yield return null;
            screenMousePos = new Vector2();
            hitObj = null;
        }

        // ReSharper disable once IteratorNeverReturns
    }

}