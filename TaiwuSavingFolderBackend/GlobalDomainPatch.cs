using System.IO;
using GameData.Domains.Global;
using GameData.Domains.TaiwuEvent.EventHelper;
using GameData.Utilities;
using HarmonyLib;

namespace TaiwuSavingFolder;

[HarmonyPatch]
public class GlobalDomainPatch
{
    public static FileSystemWatcher watcher = new();
    private static sbyte previousArchiveId = -1;

    private static void StartWatcher(sbyte archiveId)
    {
        watcher = new();
        
        watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
        watcher.IncludeSubdirectories = false;
        
        watcher.Path = SavingFolderCommon.GetSaveDirPath(archiveId);
        watcher.Filter = "local.sa*";
        
        watcher.Created += (sender, args) =>
        {
            if (Directory.Exists(args.FullPath)) return;
            var folderPath = args.FullPath + SavingFolderCommon.FolderPostfix;
            if (Directory.Exists(folderPath))
            {
                AdaptableLog.TagWarning("TaiwuSavingFolder", $"删除不该存在的存档mod数据文件夹：{folderPath}");
                Directory.Delete(folderPath, true);
            }
            
            SavingFolderCommon.CopyDirectory(SavingFolderCommon.GetCurrentModSaveDirPath(), folderPath);
        };
        watcher.Deleted += (sender, args) =>
        {
            if (Directory.Exists(args.FullPath)) return;
            var folderPath = args.FullPath + SavingFolderCommon.FolderPostfix;
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            else
            {
                AdaptableLog.TagWarning("TaiwuSavingFolder", $"文件夹不存在：{folderPath}");
            }
        };
        watcher.Renamed += (sender, args) =>
        {
            if (Directory.Exists(args.FullPath)) return;
            var folderPath = args.FullPath + SavingFolderCommon.FolderPostfix;
            if (Directory.Exists(folderPath))
            {
                AdaptableLog.TagWarning("TaiwuSavingFolder", $"删除不该存在的存档mod数据文件夹：{folderPath}");
                Directory.Delete(folderPath, true);
            }
            
            var oldFolderPath = args.OldFullPath + SavingFolderCommon.FolderPostfix;
            if (Directory.Exists(oldFolderPath))
            {
                SavingFolderCommon.CopyDirectory(oldFolderPath, folderPath);
            }
            else
            {
                AdaptableLog.TagWarning("TaiwuSavingFolder", $"文件夹不存在：{oldFolderPath}");
            }
        };
        
        watcher.EnableRaisingEvents = true;
    }
    
    private static void StopWatcher()
    {
        watcher.EnableRaisingEvents = false;
        watcher.Dispose();
    }
    
    [HarmonyPostfix, HarmonyPatch(typeof(GlobalDomain), "LeaveWorld")]
    public static void LeaveWorldPostfix()
    {
        AdaptableLog.Info("LeaveWorld!");
        StopWatcher();

        SavingFolderEvents.RaiseLeaveWorldEvent();

        // 离开世界后删除原文件夹
        Directory.Delete(SavingFolderCommon.GetCurrentModSaveDirPath(previousArchiveId), true);

        previousArchiveId = -1;
    }

    [HarmonyPrefix, HarmonyPatch(typeof(GlobalDomain), "LoadWorld")]
    public static void LoadWorldPrefix(sbyte archiveId, long backupTimestamp)
    {
        AdaptableLog.Info($"LoadWorld: {archiveId}, {backupTimestamp}");
        // FIXME 未处理太吾梦回的问题

        var folderFullPath = SavingFolderCommon.GetModSaveDirPath(archiveId, backupTimestamp);

        var currentModSaveDirPath = SavingFolderCommon.GetCurrentModSaveDirPath(archiveId);

        if (Directory.Exists(currentModSaveDirPath))
        {
            AdaptableLog.Warning($"删除之前未删除的当前mod数据文件夹{currentModSaveDirPath}");

            // 如果之前的文件夹因故未删除，则先删除
            Directory.Delete(currentModSaveDirPath, true);
        }

        if (Directory.Exists(folderFullPath))
        {
            SavingFolderCommon.CopyDirectory(folderFullPath, currentModSaveDirPath);
        }
        else
        {
            Directory.CreateDirectory(currentModSaveDirPath);
        }

        SavingFolderEvents.RaiseEnterWorldEvent();
        StartWatcher(archiveId);
        previousArchiveId = archiveId;
    }

    [HarmonyPrefix, HarmonyPatch(typeof(GlobalDomain), "EnterNewWorld")]
    public static void EnterNewWorldPrefix(sbyte archiveId)
    {
        AdaptableLog.Info($"EnterNewWorld: {archiveId}");

        var currentModSaveDirPath = SavingFolderCommon.GetCurrentModSaveDirPath(archiveId);

        if (Directory.Exists(currentModSaveDirPath))
        {
            AdaptableLog.Warning($"删除之前未删除的当前mod数据文件夹{currentModSaveDirPath}");

            // 如果之前的文件夹因故未删除，则先删除
            Directory.Delete(currentModSaveDirPath, true);
        }
        
        Directory.CreateDirectory(currentModSaveDirPath);

        SavingFolderEvents.RaiseEnterWorldEvent();
        StartWatcher(archiveId);
        previousArchiveId = archiveId;
    }
}