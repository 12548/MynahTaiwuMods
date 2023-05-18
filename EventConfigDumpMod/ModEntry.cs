using System.IO;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

namespace EventConfigDumpMod
{
    [PluginConfig("DumpMod", "myna12548", "99.99.99.99")]
    public class ModEntry: TaiwuRemakePlugin
    {
        public override void Initialize()
        {
            // ReSharper disable once Unity.UnknownResource
            var eventEditorConfig = Resources.Load<TextAsset>("EventEditor/EventEditorConfig").text;
            File.WriteAllText("EventEditorConfig.lua", eventEditorConfig);
            Debug.Log(eventEditorConfig);
        }

        public override void OnLoadedArchiveData()
        {
            // ReSharper disable once Unity.UnknownResource
            var eventEditorConfig = Resources.Load<TextAsset>("EventEditor/EventEditorConfig").text;
            File.WriteAllText("EventEditorConfig.lua", eventEditorConfig);
            Debug.Log(eventEditorConfig);
        }


        public override void Dispose()
        {
        }
    }
}