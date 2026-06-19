using System;
using HarmonyLib;

namespace MynahMoreInfo;

[HarmonyPatch(typeof(TMPTextSpriteHelper), nameof(TMPTextSpriteHelper.HandleSpriteReplace))]
public class TMPTextSpriteHelperPatch
{
    static Exception Finalizer()
    {
        return null; // suppresses all exceptions
    }
}