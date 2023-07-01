using System;
using System.IO;
using GameData.Domains.Global;
using GameData.Utilities;
using HarmonyLib;

namespace TaiwuSavingFolder;

public static class SavingFolderMain
{
    /// <summary>
    /// 离开世界（回到主菜单）时触发此事件。此时应当关闭文件夹中的所有文件，以便删除。
    /// </summary>
    public static event Action<sbyte>? LeaveWorldEvent;

    /// <summary>
    /// 进入世界时触发此事件。此时可以开始使用文件夹了。
    /// </summary>
    public static event Action<sbyte>? EnterWorldEvent;

    /// <summary>
    /// 保存（备份）世界时触发此事件。此时应当暂时停止占用写文件，避免备份出错。
    /// </summary>
    public static event Action<sbyte>? BeforeSaveWorldEvent;

    public static event Action<sbyte>? AfterSaveWorldEvent;

    private static FileSystemWatcher _watcher = new();
    private static sbyte _previousArchiveId = -1;

    private static void StartWatcher(sbyte archiveId)
    {
        _watcher = new FileSystemWatcher();

        _watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                                NotifyFilters.FileName | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
        _watcher.IncludeSubdirectories = false;

        _watcher.Path = SavingFolderCommon.GetSaveDirPath(archiveId);
        _watcher.Filter = "local.sa*";

        _watcher.Created += (_, args) =>
        {
            if (Directory.Exists(args.FullPath)) return;
            AdaptableLog.TagInfo("TaiwuSavingFolder", $"File Created: {args.Name}");
            var folderPath = args.FullPath + SavingFolderCommon.FolderPostfix;
            if (Directory.Exists(folderPath))
            {
                AdaptableLog.TagWarning("TaiwuSavingFolder", $"删除不该存在的存档mod数据文件夹：{folderPath}");
                Directory.Delete(folderPath, true);
            }

            SavingFolderCommon.CopyDirectory(SavingFolderCommon.GetCurrentModSaveDirPath(), folderPath);
        };
        _watcher.Deleted += (_, args) =>
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
        _watcher.Renamed += (_, args) =>
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

        _watcher.EnableRaisingEvents = true;
    }

    private static void StopWatcher()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
    }


    [HarmonyPatch]
    public static class GlobalDomainPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GlobalDomain), "LeaveWorld")]
        public static void LeaveWorldPostfix()
        {
            AdaptableLog.Info("LeaveWorld!");
            StopWatcher();

            LeaveWorldEvent?.Invoke(_previousArchiveId);

            // 离开世界后删除原文件夹
            Directory.Delete(SavingFolderCommon.GetCurrentModSaveDirPath(), true);

            _previousArchiveId = -1;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GlobalDomain), "LoadWorld")]
        public static void LoadWorldPrefix(sbyte archiveId, long backupTimestamp)
        {
            AdaptableLog.Info($"LoadWorld: {archiveId}, {backupTimestamp}");
            // FIXME 未处理太吾梦回的问题

            var folderFullPath = SavingFolderCommon.GetModSaveDirPath(archiveId, backupTimestamp);

            var currentModSaveDirPath = SavingFolderCommon.GetCurrentModSaveDirPath();

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

            EnterWorldEvent?.Invoke(archiveId);
            StartWatcher(archiveId);
            _previousArchiveId = archiveId;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GlobalDomain), "EnterNewWorld")]
        public static void EnterNewWorldPrefix(sbyte archiveId)
        {
            AdaptableLog.Info($"EnterNewWorld: {archiveId}");

            var currentModSaveDirPath = SavingFolderCommon.GetCurrentModSaveDirPath();

            if (Directory.Exists(currentModSaveDirPath))
            {
                AdaptableLog.Warning($"删除之前未删除的当前mod数据文件夹{currentModSaveDirPath}");

                // 如果之前的文件夹因故未删除，则先删除
                Directory.Delete(currentModSaveDirPath, true);
            }

            Directory.CreateDirectory(currentModSaveDirPath);

            EnterWorldEvent?.Invoke(archiveId);
            StartWatcher(archiveId);
            _previousArchiveId = archiveId;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GlobalDomain), "SetSavingWorld")]
        public static void SetSavingWorldPostfix(bool value)
        {
            AdaptableLog.Info($"SetSavingWorld: {value}");
            if (value)
            {
                BeforeSaveWorldEvent?.Invoke(_previousArchiveId);
            }
            else
            {
                AfterSaveWorldEvent?.Invoke(_previousArchiveId);
            }
        }
    }
}