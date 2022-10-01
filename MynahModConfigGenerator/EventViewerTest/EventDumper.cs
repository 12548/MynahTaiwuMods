using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Config.EventConfig;
using GameData.Utilities;
using Newtonsoft.Json;

namespace MynahModConfigGenerator.EventViewerTest;

public static class EventDumper
{
    public static void Dump()
    {
        var taiwuPath = System.Environment.GetEnvironmentVariable("TAIWU_PATH");

        if (taiwuPath != null)
        {
            var taiwuDllPath = Path.Join(taiwuPath, "Event/EventLib");
            var files = Directory.GetFiles(taiwuDllPath, "*.dll");
            var count = 0;

            Dictionary<string, Dictionary<string, EventDumpInfo>> groups = new(999);

            foreach (var file in files)
            {
                var eventAssembly = Assembly.LoadFrom(file);
                Program.LoadAssemblyReferences(eventAssembly);

                foreach (var t in eventAssembly.GetExportedTypes()
                             .FindAll(it => it.IsSubclassOf(typeof(TaiwuEventItem))))
                {
                    count++;
                    var taiwuEventItem = (TaiwuEventItem)System.Activator.CreateInstance(t)!;
                    var eventDumpInfo = new EventDumpInfo(taiwuEventItem);

                    if (!groups.ContainsKey(taiwuEventItem.EventGroup))
                    {
                        groups.Add(taiwuEventItem.EventGroup, new Dictionary<string, EventDumpInfo>());
                    }
                    
                    groups[taiwuEventItem.EventGroup].Add(eventDumpInfo.Guid, eventDumpInfo);
                }
            }

            File.WriteAllText("dump.json", JsonConvert.SerializeObject(groups));
            Console.WriteLine($"Dump ok {count} events.");
        }
    }
}