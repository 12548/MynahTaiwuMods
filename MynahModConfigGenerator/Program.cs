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
        static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        public static void LoadAssemblyReferences(Assembly selectedAssembly)
        {
            foreach (AssemblyName reference in selectedAssembly.GetReferencedAssemblies())
            {
                if (System.IO.File.Exists(
                        System.IO.Path.GetDirectoryName(selectedAssembly.Location) +
                        @"\" + reference.Name + ".dll"))
                {
                    System.Reflection.Assembly.LoadFrom(
                        System.IO.Path.GetDirectoryName(selectedAssembly.Location) +
                        @"\" + reference.Name + ".dll");
                }
                else
                {
                    var taiwuPath = System.Environment.GetEnvironmentVariable("TAIWU_PATH");

                    if (taiwuPath != null)
                    {
                        var taiwuDllPath = Path.Join(taiwuPath, "The Scroll of Taiwu_Data", "Managed",
                            reference.Name + ".dll");
                        var taiwuDllPath2 = Path.Join(taiwuPath, "The Scroll of Taiwu_Data", "Managed",
                            reference.Name + ".dll");
                        if (File.Exists(taiwuDllPath))
                        {
                            System.Reflection.Assembly.LoadFrom(taiwuDllPath);
                        } else if (File.Exists(taiwuDllPath2))
                        {
                            System.Reflection.Assembly.LoadFrom(taiwuDllPath2);
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// 将Lua文件中第一个左大括号和最后一个右大括号之间的内容作为表读取，改变其中的"DefaultSettings"内容并输出到文件。
        /// </summary>
        /// <param name="args">第一个是读取的lua文件，后面的参数是读取的dll文件，最后一个参数是输出的lua位置（不填则改变原lua，根据扩展名判断）</param>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            if (args.Length >= 2)
            {
                var luaFilePath = args[0];
                var luastr = File.ReadAllText(luaFilePath);

                Console.WriteLine("Before: " + luastr);
                var assemblies = args.Where(it => it.EndsWith(".dll")).Select(Assembly.LoadFrom);
                
                foreach (var assembly in assemblies)
                {
                    LoadAssemblyReferences(assembly);
                }
                
                var result = ChangeLua(luastr, assemblies);
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
            Assembly assembly = null;

            assemblies.TryGetValue(args.Name, out assembly);

            return assembly;
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Assembly assembly = args.LoadedAssembly;
            assemblies[assembly.FullName] = assembly;
        }

        /// <summary>
        /// 将Lua字符串中第一个左大括号和最后一个右大括号之间的内容作为表读取，改变其中的"DefaultSettings"内容并返回。
        /// </summary>
        /// <param name="luastr">Lua字符串</param>
        /// <param name="assemblies">要读取ModSetting的程序集</param>
        /// <returns></returns>
        private static string ChangeLua(string luastr, IEnumerable<Assembly> assemblies)
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

                data!.TryGetValue("DefaultSettings", out JToken token);

                var settings = (token) as JArray ?? new JArray();
                settings.Clear();

                var keyList = new HashSet<string>();
                foreach (var assembly in assemblies)
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
                            if (setting != null && !keyList.Contains(key))
                            {
                                settings.Add(JsonConvert.DeserializeObject(
                                    JsonConvert.SerializeObject(modSetting.ToDictionary(fieldInfo))));
                                keyList.Add(key);
                            }
                        }
                    }
                }

                data["DefaultSettings"] = settings;

                var s = JObjToDict(data);

                var result = luastr[0..leftBraceIndex] + LuaSerializer.Serialize(s) +
                             luastr[(rightBraceIndex + 1)..];

                return result;
            }
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