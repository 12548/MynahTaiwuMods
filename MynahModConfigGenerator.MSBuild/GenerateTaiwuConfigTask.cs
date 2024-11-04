using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConfigGenerator
{
    public class GenerateTaiwuConfigTask : Task
    {
        public override bool Execute()
        {
            if (PluginDir != null)
            {
                ConfigLuaPath = PluginDir.Of("Config.lua");
                TargetDllDir = PluginDir.Of("Plugins");
            }
            
            if (TargetDlls == null)
            {
                if (TargetDllDir != null)
                {
                    TargetDlls = Directory.GetFiles(TargetDllDir, "*.dll");
                }
                else if (TargetDll != null)
                {
                    TargetDlls = new string[] { TargetDll };
                }
            }

            string generatorPath = Assembly.GetAssembly(typeof(GenerateTaiwuConfigTask)).Location.Parent().Parent().Parent() + "\\tools\\MynahModConfigGenerator.exe";
            if (generatorPath == null)
            {
                return false;
            }

            IEnumerable<string> args = new string[] { ConfigLuaPath }.Concat(TargetDlls);

            string err = null;
            using (Process proc = new Process())
            {
                ProcessStartInfo procInfo = new ProcessStartInfo()
                {
                    FileName = generatorPath,
                    Arguments = args.Select(WrapParam).Join(" "),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                };
                proc.StartInfo = procInfo;
                proc.Start();

                err = proc.StandardError.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(err))
            {
                throw new Exception(err);
            }

            return true;
        }

        /// <summary>
        /// 插件输入路径，假设遵循如下文件结构:
        /// <code>
        /// [PluginDir]
        /// │  Config.lua
        /// │
        /// └─Plugins
        ///     YourMod.dll
        /// </code>
        /// </summary>
        public string PluginDir { get; set; }

        /// <summary>
        /// Mod的配置文件Config.lua路径
        /// </summary>
        public string ConfigLuaPath { get; set; }

        /// <summary>
        /// Mod插件dll文件路径
        /// </summary>
        public string TargetDll { get; set; }

        /// <summary>
        /// Mod插件dll文件夹路径，会自动搜索路径下的全部dll提取配置
        /// </summary>
        public string TargetDllDir { get; set; }

        public string[] TargetDlls { get; set; }

        public static string WrapParam(string p)
        {
            return $"\"{p}\"";
        }
    }

    public static class StringJoinExtension
    {
        public static string Join(this IEnumerable<string> strs, string sep)
        {
            return string.Join(sep, strs);
        }
    }

    public static class PathExtension
    {
        public static string Parent(this string path)
        {
            return Directory.GetParent(path).FullName;
        }

        public static string Of(this string path, string sub)
        {
            return Path.Combine(path, sub);
        }
    }
}