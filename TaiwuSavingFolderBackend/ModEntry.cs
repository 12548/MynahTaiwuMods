using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using GameData.Domains.TaiwuEvent.EventHelper;
using GameData.Utilities;
using TaiwuModdingLib.Core.Plugin;

namespace TaiwuSavingFolder
{
    [PluginConfig("TaiwuSavingFolder", "myna12548", "99.99.99.99")]
    public class ModEntry : TaiwuRemakeHarmonyPlugin
    {

        
        // private const string DbFileName = "TaiwuSqlite.db";
        //
        // public static SQLiteConnection? _currentConn;

        public override void Initialize()
        {
            base.Initialize();
            AdaptableLog.Info($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");
            AdaptableLog.Info($"ArchiveBaseDir: {GameData.ArchiveData.Common.ArchiveBaseDir}");
        }

        public override void OnEnterNewWorld()
        {
            base.OnEnterNewWorld();
            AdaptableLog.Info($"OnEnterNewWorld");
        }

        public override void OnLoadedArchiveData()
        {
            base.OnLoadedArchiveData();
            AdaptableLog.Info($"OnLoadedArchiveData");
        }

        public static string GetSavePath()
        {
            var path = Path.Combine(GameData.ArchiveData.Common.ArchiveBaseDir,
                $"world_{EventHelper.GetCurrentArchiveId() + 1}");
            return path;
        }
    }
}