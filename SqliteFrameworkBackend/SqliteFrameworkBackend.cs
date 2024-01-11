using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GameData.Domains;
using GameData.Utilities;
using SqliteFrameworkBase;
using TaiwuModdingLib.Core.Plugin;
using TaiwuSavingFolder;

namespace SqliteFrameworkBackend;

[PluginConfig("SqliteFramework", "myna12548", "99.99.99.99")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class SqliteFrameworkBackend : TaiwuRemakeHarmonyPlugin
{
    public static readonly string FileName = "data.db";
    // public static SQLiteConnection Connection { get; private set; } = null!;

    public override void Initialize()
    {
        base.Initialize();
        
        var cwd = Environment.CurrentDirectory;
        AdaptableLog.Info($"cwd: {cwd}");
 
        var modDirectory = DomainManager.Mod.GetModDirectory(ModIdStr); 
        AdaptableLog.Info($"ModDir: {modDirectory}");

        var sqliteDllSource = Path.Combine(modDirectory, "Plugins/sqlite3.dll");
        var sqliteDllTargetBackend = Path.Combine(cwd, "sqlite3.dll");
        if (!File.Exists(sqliteDllTargetBackend))
        {
            File.Copy(sqliteDllSource,
                sqliteDllTargetBackend, true);
        }

        var sqliteDllTargetFrontend = Path.Combine(cwd, "../The Scroll of Taiwu_Data/Plugins/x86_64/sqlite3.dll");
        if (!File.Exists(sqliteDllTargetFrontend))
        {
            File.Copy(sqliteDllSource, sqliteDllTargetFrontend, true);
        }
        
        Assembly.LoadFile(Path.Combine(GameData.Program.BaseDataDir, "The Scroll of Taiwu_Data/Managed/Mono.Data.Sqlite.dll"));
        //
        //
        // Assembly.LoadFile(Path.Combine(modDirectory, "Plugins/SQLitePCLRaw.core.dll"));
        // Assembly.LoadFile(Path.Combine(modDirectory, "Plugins/SQLitePCLRaw.provider.e_sqlite3.dll"));
        // Assembly.LoadFile(Path.Combine(modDirectory, "Plugins/SQLitePCLRaw.batteries_v2.dll"));
        //
        SavingFolderMain.EnterWorldEvent += archiveId =>
        {
            var databasePath = Path.GetFullPath(Path.Combine(SavingFolderCommon.GetCurrentModSaveDirPath(), FileName));
            // if (!File.Exists(databasePath))
            // {
            //     File.Copy(Path.Combine(modDirectory, FileName),
            //         databasePath);
            // }

            AdaptableLog.Info($"Sqlite Framework: Backend Connecting {databasePath}");
            SqliteFramework.InitConnection(databasePath);
        };
        SavingFolderMain.LeaveWorldEvent += _ =>
        {
            SqliteFramework.CloseConnection();
            AdaptableLog.Info($"Sqlite Framework: Backend Closed");
        };
    }
}