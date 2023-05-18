using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Config.EventConfig;
using GameData.Utilities;
using MynahModConfigGenerator.EventViewerTest;
using Newtonsoft.Json;


public static class EventDumper
{
    public static void Dump()
    {
        var taiwuPath = Program.TaiwuPath;

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
                    try
                    {
                        count++;
                        var taiwuEventItem = (TaiwuEventItem)Activator.CreateInstance(t)!;
                        var eventDumpInfo = new EventDumpInfo(taiwuEventItem);

                        if (!groups.ContainsKey(taiwuEventItem.EventGroup))
                        {
                            groups.Add(taiwuEventItem.EventGroup, new Dictionary<string, EventDumpInfo>());
                        }
                        
                        groups[taiwuEventItem.EventGroup].Add(eventDumpInfo.Guid, eventDumpInfo);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error dumping {t.Name} from {eventAssembly.Location}");
                        Console.WriteLine(2);
                        continue;
                    }
                }
            }

            File.WriteAllText("dump.json", JsonConvert.SerializeObject(groups));
            Console.WriteLine($"Dump ok {count} events.");
        }
    }
}