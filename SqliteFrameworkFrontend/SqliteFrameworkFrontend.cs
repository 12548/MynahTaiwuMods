using System.IO;
using System.Reflection;
using System.Threading;
using GameData.Domains.Mod;
using GameData.GameDataBridge;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;
using Mono.Data.Sqlite;
using SqliteFrameworkBase;

namespace SqliteFrameworkFrontend;

[PluginConfig("SqliteFramework", "myna12548", "0")]
public class SqliteFrameworkFrontend: TaiwuRemakeHarmonyPlugin
{
    public static readonly string FileName = "data.db";
    public static SqliteConnection Connection { get; private set; } = null!;

    private static string _modIdStr = "";
    
    public override void Initialize()
    {
        base.Initialize();
        Debug.Log($"Initialize: SqliteFrameworkFrontend");
        _modIdStr = this.ModIdStr;

    }
  
    public override void OnLoadedArchiveData()
    { 
        base.OnLoadedArchiveData();
    }
    
    [HarmonyPatch]
    public static class Patch
    {
        [HarmonyPatch(typeof(UI_RecordSelect), "OnEnterWorldLoadFinish")]
        [HarmonyPostfix]
        static void OnEnterWorldLoadFinishPostFix()
        {
            // var dir = ModManager.GetLoadedModInfoList().Items.Find(it => it.ModId.ToString() == _modIdStr)
            //     .DirectoryName;
            // var destDir = Path.Combine(Application.dataPath, "The Scroll of Taiwu_Data/Mono");
            // Debug.Log($"Copying from: {dir} to: {destDir}");
            // Directory.CreateDirectory(destDir);
            // File.Copy(Path.Combine(dir, "Plugins\\sqlite3.dll"),
            //     Path.Combine(destDir, "sqlite3.dll"), true);
            //
            // // 好像不能直接用，要等他拷贝完
            // Thread.Sleep(100);
            //
            var path = Path.Combine(Game.GetArchiveDirPath(), "_current_mod_data", FileName);

            SqliteFramework.InitConnection(path);
            
            Debug.Log($"Sqlite Framework: Frontend Connected {path}");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GlobalOperations), "LeaveWorld")]
        static void LeaveWorldPrefix()
        {
            SqliteFramework.CloseConnection();
            
            Debug.Log("Sqlite Framework: Frontend Closed!");
        }
    }
}