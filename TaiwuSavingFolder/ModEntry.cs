using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using GameData.Domains.TaiwuEvent.EventHelper;
using GameData.Utilities;
using TaiwuModdingLib.Core.Plugin;

namespace TaiwuSavingFolder
{
    public class ModEntry : TaiwuRemakeHarmonyPlugin
    {
        public FileSystemWatcher watcher = new();

        private const string FolderPostfix = "_data";
        
        // private const string DbFileName = "TaiwuSqlite.db";
        //
        // public static SQLiteConnection? _currentConn;

        public override void Initialize()
        {
            base.Initialize();
            AdaptableLog.Info($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");
            
            watcher.Path = GameData.ArchiveData.Common.ArchiveBaseDir;
            watcher.Filter = "*.sav";
            watcher.Changed += new FileSystemEventHandler(OnProcess);
            watcher.Created += new FileSystemEventHandler(OnProcess);
            watcher.Deleted += new FileSystemEventHandler(OnProcess);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.IncludeSubdirectories = true;
        }

        public override void OnEnterNewWorld()
        {
            base.OnEnterNewWorld();
        }

        public override void OnLoadedArchiveData()
        {
            base.OnLoadedArchiveData();
        }

        public static string GetSavePath()
        {
            var path = Path.Combine(GameData.ArchiveData.Common.ArchiveBaseDir,
                $"world_{EventHelper.GetCurrentArchiveId() + 1}");
            return path;
        }
        
        private static void OnProcess(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                OnCreated(source, e);
            }
            else if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                OnChanged(source, e);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                OnDeleted(source, e);
            }
        }
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("文件新建事件处理逻辑 {0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
        }
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("文件改变事件处理逻辑{0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
        }
        
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("文件删除事件处理逻辑{0}  {1}   {2}", e.ChangeType, e.FullPath, e.Name);
        }
        
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("文件重命名事件处理逻辑{0}  {1}  {2}", e.ChangeType, e.FullPath, e.Name);
        }
    }
}