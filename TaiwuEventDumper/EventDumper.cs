﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Config.EventConfig;
using GameData.Utilities;
using LuaTableSerializer;
using MynahModConfigGenerator.EventViewerTest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaiwuEventDumper;


public static class EventDumper
{
    public static void Dump()
    {
        var taiwuPath = Program.TaiwuPath;

        if (taiwuPath != null)
        {
            var taiwuDllPath = Path.Join(taiwuPath, "Event/EventLib");
            var dllFiles = Directory.GetFiles(taiwuDllPath, "*.dll");
            // Key: Taiwu_EventPackage_{EventPackageName}_Language_CN.txt
            // Dictionary<string, TaiWuTemplate> parsedTemplates;
            Dictionary<string, EventData> parsedLangs = new();
            Dictionary<string, string> groupNames = new();

            var count = 0;

            // 读取语言文件
            // 来自：https://github.com/TaiwuCommunityTranslation/taiwu-language-files-zh-hans/
            {
                var eventsDirectory = Path.Join(taiwuPath, "Event", "EventLanguages");
                if (!Directory.Exists(eventsDirectory))
                {
                    Console.Error.WriteLine($"Invalid events directory: {eventsDirectory}!");
                    Environment.Exit(1);
                }

                Console.WriteLine("[+] saving EventLanguages...");

                DirectoryInfo d = new DirectoryInfo(eventsDirectory); //Assuming Test is your Folder
                Console.WriteLine("Loading files");
                Dictionary<string, FileInfo>
                    files = d.GetFiles("*.txt").ToDictionary(file => file.Name); //Getting Text files
                Console.WriteLine("Generating Templates");
                var parsedTemplates =
                    files.ToDictionary(f => f.Key, f => new TaiWuTemplate(f.Value));
                foreach (var (key, value) in parsedTemplates)
                {
                    groupNames[value.group] = value.groupName;
                    foreach (var (s, eventData) in value.eventMap)
                    {
                        parsedLangs.Add(s, eventData);
                    }
                }
            }

            var targetMainDir = Path.Join(taiwuPath, "ModFactory", "ConchShip", "WorkSpace/EventEditorData/EventCore");

            Dictionary<string, Dictionary<string, EventDumpInfo>> groups = new(999);

            foreach (var file in dllFiles)
            {
                var eventAssembly = Assembly.LoadFrom(file);
                Program.LoadAssemblyReferences(eventAssembly);

                var exportedTypes = eventAssembly.GetExportedTypes();

                var packageType = exportedTypes.Find(it => it.IsSubclassOf(typeof(EventPackage)));
                var package = (EventPackage)Activator.CreateInstance(packageType)!;

                List<TaiwuEventItem> eventList =
                    (List<TaiwuEventItem>)typeof(EventPackage).GetField("EventList", (BindingFlags)(-1))!.GetValue(
                        package);

                var packageDir = Path.Join(targetMainDir, package.Key);

                var indexData = new IndexContentData(package.Key, groupNames[package.Group], package.Author);

                foreach (var taiwuEventItem in eventList)
                {
                    var eventGuid = taiwuEventItem.Guid.ToString();
                    var twe = new TweData(
                        eventGuid,
                        taiwuEventItem.EventGroup,
                        taiwuEventItem.ForceSingle,
                        "None",
                        taiwuEventItem.EventSortingOrder,
                        taiwuEventItem.MainRoleKey,
                        taiwuEventItem.TargetRoleKey,
                        taiwuEventItem.EventBackground,
                        true,
                        "",
                        taiwuEventItem.MaskControl != 0,
                        taiwuEventItem.MaskControl,
                        taiwuEventItem.MaskTweenTime.ToString(),
                        taiwuEventItem.EventAudio,
                        taiwuEventItem.EscOptionKey,
                        taiwuEventItem.EventOptions
                            .Select(it => new TweEventOption(new Guid().ToString(), it.OptionKey)).ToArray()
                    );

                    var serializeObject = JsonConvert.SerializeObject(twe);
                    var redeObj = JsonConvert.DeserializeObject<JObject>(serializeObject);
                    var tod = JObjToDict(redeObj);
                    var table = LuaSerializer.Serialize(tod);
                    var tweDir = Path.Join(packageDir, eventGuid);
                    var twePath = Path.Join(tweDir, eventGuid + ".twe");

                    Directory.CreateDirectory(tweDir);
                    
                    Console.WriteLine($"正在导出：{twePath}");
                    File.WriteAllText(twePath, "return " + table);

                    Dictionary<object, string> t = new();
                    var eventLang = parsedLangs[eventGuid];
                    t["EventName"] = eventLang.name;
                    t["EventContent"] = eventLang.content;
                    
                    for (var i = 0; i < taiwuEventItem.EventOptions.Length; i++)
                    {
                        t[i + 1] = eventLang.options[i];
                    }
                    
                    indexData.AllEventContent[eventGuid] = t;
                }
                
                {                    
                    var serializeObject = JsonConvert.SerializeObject(indexData);
                    var redeObj = JsonConvert.DeserializeObject<JObject>(serializeObject);
                    var tod = JObjToDict(redeObj);
                    var table = LuaSerializer.Serialize(tod);
                    var twePath = Path.Join(packageDir, "index.lua");
                    
                    Console.WriteLine($"正在导出：{twePath}");
                    File.WriteAllText(twePath, "return " + table);}

                //
                // foreach (var t in exportedTypes
                //              .FindAll(it => it.IsSubclassOf(typeof(TaiwuEventItem))))
                // {
                //     try
                //     {
                //         count++;
                //         var taiwuEventItem = (TaiwuEventItem)Activator.CreateInstance(t)!;
                //         var eventDumpInfo = new EventDumpInfo(taiwuEventItem);
                //
                //         if (!groups.ContainsKey(taiwuEventItem.EventGroup))
                //         {
                //             groups.Add(taiwuEventItem.EventGroup, new Dictionary<string, EventDumpInfo>());
                //         }
                //         
                //         groups[taiwuEventItem.EventGroup].Add(eventDumpInfo.Guid, eventDumpInfo);
                //     }
                //     catch (Exception e)
                //     {
                //         Console.WriteLine($"Error dumping {t.Name} from {eventAssembly.Location}");
                //         Console.WriteLine(2);
                //         continue;
                //     }
                // }
            }

            // File.WriteAllText("dump.json", JsonConvert.SerializeObject(groups));
            // Console.WriteLine($"Dump ok {count} events.");
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