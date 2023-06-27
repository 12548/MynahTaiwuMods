using System.IO;
using GameData.Domains.TaiwuEvent.EventHelper;

namespace TaiwuSavingFolder;

public static class SavingFolderCommon
{
    public static readonly string FolderPostfix = "_mod_data";
    
    /// <summary>
    /// 获取原版指定编号的存档目录的完整路径
    /// </summary>
    /// <param name="archiveId">从0开始的存档编号（当前最大为4，即共5个存档）</param>
    /// <returns></returns>
    public static string GetSaveDirPath(sbyte archiveId)
    {
        return Path.Combine(GameData.ArchiveData.Common.ArchiveBaseDir,
            $"world_{archiveId + 1}");
    }
    public static string GetSaveDirPath()
    {
        return GetSaveDirPath(EventHelper.GetCurrentArchiveId());
    }
    
    /// <summary>
    /// 获取原版存档文件的完整路径
    /// </summary>
    /// <param name="archiveId">从0开始的存档编号（当前最大为4，即共5个存档）</param>
    /// <param name="backupTimestamp">存档备份时间戳，非备份存档为-1</param>
    /// <returns></returns>
    public static string GetSaveFilePath(sbyte archiveId, long backupTimestamp = -1)
    {
        return Path.Combine(GetSaveDirPath(archiveId), backupTimestamp == -1L ? "local.sav" : $"local.sav.bak.{backupTimestamp}");
    }
    public static string GetSaveFilePath(long backupTimestamp = -1)
    {
        return GetSaveFilePath(EventHelper.GetCurrentArchiveId(), backupTimestamp);
    }
    
    /// <summary>
    /// 获取存档对应的本mod创建的文件夹的完整路径
    /// </summary>
    /// <param name="archiveId">从0开始的存档编号（当前最大为4，即共5个存档）</param>
    /// <param name="backupTimestamp">存档备份时间戳，非备份存档为-1</param>
    /// <returns></returns>
    public static string GetModSaveDirPath(sbyte archiveId, long backupTimestamp = -1)
    {
        return GetSaveFilePath(archiveId, backupTimestamp) + FolderPostfix;
    }
    public static string GetModSaveDirPath(long backupTimestamp = -1)
    {
        return GetModSaveDirPath(EventHelper.GetCurrentArchiveId(), backupTimestamp);
    }
    
    /// <summary>
    /// 获取当前正在运行的游戏对应的本mod创建的文件夹的完整路径
    /// <param name="archiveId">从0开始的存档编号（当前最大为4，即共5个存档）</param>
    /// </summary>
    /// <returns>当前正在运行的游戏对应的本mod创建的文件夹的完整路径</returns>
    public static string GetCurrentModSaveDirPath(sbyte archiveId)
    {
        return Path.Combine(GetSaveDirPath(archiveId), "_current_mod_data");
    }
    /// <summary>
    /// 获取当前正在运行的游戏对应的本mod创建的文件夹的完整路径
    /// </summary>
    /// <returns>当前正在运行的游戏对应的本mod创建的文件夹的完整路径</returns>
    public static string GetCurrentModSaveDirPath()
    {
        return GetCurrentModSaveDirPath(EventHelper.GetCurrentArchiveId());
    }
    
    /// <summary>
    /// 复制文件夹
    /// </summary>
    /// <param name="sourceDir">源文件夹</param>
    /// <param name="destinationDir">目标文件夹</param>
    /// <param name="recursive">递归复制子目录</param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}