using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LuaTableSerializer;
using MynahBaseModBase;
using MynahModConfigGenerator.EventViewerTest;
using Neo.IronLua;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MynahModConfigGenerator
{
    static class Program
    {
        const string FrontendPlugins = "FrontendPlugins";
        const string BackendPlugins = "BackendPlugins";
        const string Dependencies = "Dependencies";
        const string DefaultSettings = "DefaultSettings";
        readonly static Dictionary<string, long> KnownDependencies = new()
        {
            ["MynahBaseModBase"] = 2878665107,
            ["MynahBaseModBackend"] = 2878665107,
            ["MynahBaseModFrontend"] = 2878665107,
        };

        static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        public static void LoadAssemblyReferences(Assembly selectedAssembly)
        {
            foreach (AssemblyName reference in selectedAssembly.GetReferencedAssemblies())
            {
                if (File.Exists(
                        Path.GetDirectoryName(selectedAssembly.Location) +
                        @"\" + reference.Name + ".dll"))
                {
                    Assembly.LoadFrom(
                        Path.GetDirectoryName(selectedAssembly.Location) +
                        @"\" + reference.Name + ".dll");
                }
                else
                {
                    var taiwuPath = Environment.GetEnvironmentVariable("TAIWU_PATH");

                    if (taiwuPath != null)
                    {
                        var taiwuDllPath = Path.Join(taiwuPath, "The Scroll of Taiwu_Data", "Managed",
                            reference.Name + ".dll");
                        var taiwuDllPath2 = Path.Join(taiwuPath, "The Scroll of Taiwu_Data", "Managed",
                            reference.Name + ".dll");
                        if (File.Exists(taiwuDllPath))
                        {
                            Assembly.LoadFrom(taiwuDllPath);
                        }
                        else if (File.Exists(taiwuDllPath2))
                        {
                            Assembly.LoadFrom(taiwuDllPath2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将Lua文件的返回值作为表读取，改变其中的"DefaultSettings"内容并输出到文件。
        /// </summary>
        /// <param name="args">第一个是读取的lua文件，后面的参数是读取的dll文件，最后一个参数是输出的lua位置（不填则改变原lua，根据扩展名判断）</param>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (args.Length >= 2)
            {
                var luaFilePath = args[0];
                var luaStr = File.ReadAllText(luaFilePath);

                using (Lua lua = new Lua()) // Create the Lua script engine
                {
                    dynamic env = lua.CreateEnvironment(); // Create a environment
                    env.dochunk($"a = (function() {luaStr} end)();", "test.lua"); // Create a variable in Lua
                    LuaTable table = env.a;

                    var workshopPath = Environment.GetEnvironmentVariable("TAIWU_WORKSHOP_PATH");
                    var fileId = table["FileId"];
                    if (workshopPath != null && fileId != null)
                    {
                        var modPath = Path.Join(workshopPath, fileId.ToString());
                        
                    }

                    // Console.WriteLine(table["FileId"].GetType());
                    
                    Console.WriteLine(table.ToJson()); // Access a variable in C#
                    
                }

                Console.WriteLine("Before: " + luaStr);
                var modAssemblies = args.Where(it => it.EndsWith(".dll")).Select(Assembly.LoadFrom);

                var enumerable = modAssemblies as Assembly[] ?? modAssemblies.ToArray();
                foreach (var assembly in enumerable)
                {
                    LoadAssemblyReferences(assembly);
                }

                var result = ChangeLua(luaStr, enumerable);
                Console.WriteLine("After: " + result);

                File.WriteAllText(args.Last().EndsWith(".lua") ? args.Last() : luaFilePath, result);
            }
            else
            {
                EventDumper.Dump();
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            assemblies.TryGetValue(args.Name, out var assembly);

            return assembly;
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Assembly assembly = args.LoadedAssembly;
            assemblies[assembly.FullName!] = assembly;
        }

        /// <summary>
        /// 将Lua字符串中第一个左大括号和最后一个右大括号之间的内容作为表读取，改变其中的"DefaultSettings"内容并返回。
        /// </summary>
        /// <param name="luastr">Lua字符串</param>
        /// <param name="modAssemblies">要读取ModSetting的程序集</param>
        /// <returns></returns>
        private static string ChangeLua(string luastr, IEnumerable<Assembly> modAssemblies)
        {
            var leftBraceIndex = luastr.IndexOf("{", StringComparison.Ordinal);
            var rightBraceIndex = luastr.LastIndexOf("}", StringComparison.Ordinal);
            var tableStr = luastr[leftBraceIndex..(rightBraceIndex + 1)];

            using (Lua lua = new Lua()) // Create the Lua script engine
            {
                dynamic env = lua.CreateEnvironment(); // Create a environment
                env.dochunk($"a = {tableStr};", "test.lua"); // Create a variable in Lua
                Console.WriteLine(env.a); // Access a variable in C#

                LuaTable table = env.a;

                Console.WriteLine(table.ToJson());

                var data = JsonConvert.DeserializeObject<JObject>(table.ToJson());
                data!.TryGetValue(DefaultSettings, out JToken token);
                var settings = (token) as JArray ?? new JArray();
                settings.Clear();

                data!.TryGetValue(Dependencies, out token);
                var dependencies = (token) as JArray ?? new JArray();

                Dictionary<string, JArray> plugins = new();

                data!.TryGetValue(FrontendPlugins, out token);
                plugins.Add(FrontendPlugins, (token) as JArray ?? new JArray());

                data!.TryGetValue(BackendPlugins, out token);
                plugins.Add(BackendPlugins, (token) as JArray ?? new JArray());
                
                var keyList = new HashSet<string>();
                var deps = new HashSet<long>();
                foreach (var assembly in modAssemblies)
                {
                    foreach (var type in assembly.ExportedTypes)
                    {
                        // var plugin = type.GetCustomAttribute(typeof(TaiwuRemakePlugin));
                        // if (plugin != null)
                        // {
                        //     var taiwuPlugin = (TaiwuRemakePlugin)plugin;
                        //     data["Title"] = taiwuPlugin.PluginName;
                        // }

                        foreach (var fieldInfo in type.GetFields())
                        {
                            var setting = fieldInfo.GetCustomAttribute(typeof(ModSetting), true);
                            var modSetting = (ModSetting)setting;

                            if (modSetting == null) continue;

                            var key = modSetting.GetKey(fieldInfo); // 跳过重复的Key
                            if (keyList.Contains(key)) continue;

                            settings.Add(JsonConvert.DeserializeObject(
                                JsonConvert.SerializeObject(modSetting.ToDictionary(fieldInfo))));
                            keyList.Add(key);
                        }
                    }

                    // 检查Mod的前后端属性
                    string catagory = GeussPluginCatagory(assembly);
                    if (plugins.TryGetValue(catagory, out var list))
                    {
                        string name = Path.GetFileName(assembly.Location);
                        if (!list.Contains(name))
                        {
                            list.Add(name);
                        }
                    }

                    // 检查Mod的依赖项
                    IEnumerable<long> thisDeps = ExtractDependencies(assembly);
                    deps.UnionWith(thisDeps);
                }

                foreach (var dep in deps)
                {
                    if (!dependencies.Contains(dep))
                    {
                        dependencies.Add(dep);
                    }
                }

                data[DefaultSettings] = settings;
                data[Dependencies] = dependencies;
                foreach (var (catagory, pluginList) in plugins)
                {
                    data[catagory] = pluginList;
                }

                var s = JObjToDict(data);

                var result = luastr[..leftBraceIndex] + LuaSerializer.Serialize(s) +
                             luastr[(rightBraceIndex + 1)..];

                return result;
            }
        }

        static string GeussPluginCatagory(Assembly assembly)
        {
            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                switch (reference.Name)
                {
                    case "GameData":
                        return "BackendPlugins";
                    case "Assembly-CSharp":
                        return "FrontendPlugins";
                }
            }
            return "";
        }

        static HashSet<long> ExtractDependencies(Assembly assembly)
        {
            HashSet<long> deps = new();
            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                if (KnownDependencies.TryGetValue(reference.Name, out var id))
                {
                    deps.Add(id);
                }
            }

            return deps;
        }

        static Dictionary<string, object> JObjToDict(JObject obj)
        {
            var res = new Dictionary<string, Object>();

            foreach (var (key, value) in obj)
            {
                if (value == null) continue;
                if (value is JObject jObject)
                {
                    res[key] = JObjToDict(jObject);
                }
                else if (value is JValue jValue)
                {
                    res[key] = jValue.Value;
                }
                else if (value is JArray array)
                {
                    res[key] = JArrToDict(array);
                }
            }

            return res;
        }

        static List<object> JArrToDict(JArray obj)
        {
            var res = new List<object>();

            foreach (var value in obj)
            {
                if (value == null) continue;
                if (value is JObject jObject)
                {
                    res.Add(JObjToDict(jObject));
                }
                else if (value is JValue jValue)
                {
                    res.Add(jValue.Value);
                }
                else if (value is JArray array)
                {
                    res.Add(JArrToDict(array));
                }
            }

            return res;
        }
    }
}