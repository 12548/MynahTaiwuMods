using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using GameData.Utilities;
using HarmonyLib;
using Newtonsoft.Json;
using TaiwuModdingLib.Core.Plugin;

namespace VillagerIdentities
{
    [PluginConfig("VillagerIdentities", "myna12548", "1")]
    public class ModEntry: TaiwuRemakeHarmonyPlugin
    {
        public override void OnModSettingUpdate()
        {
            MynahBaseModBackend.MynahBaseModBackend.OnModSettingUpdate(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            createItems();
        }

        public override void OnEnterNewWorld()
        {
	        base.OnEnterNewWorld();
	        AdaptableLog.Info("OnEnterNewWorld!");
	        initMembers();
        }

        public override void OnLoadedArchiveData()
        {
            base.OnLoadedArchiveData();
            AdaptableLog.Info("OnLoadedArchiveData!");
            // foreach (var member in Organization.Instance.GetItem(Organization.DefKey.Taiwu).Members)
            // {
            //     File.WriteAllText("MyDump.txt", member + " = " + DebugDump(OrganizationMember.Instance[member]));
            // }
            //
            // var villager = OrganizationMember.Instance.Where(it => it.GradeName == "村民");
            // foreach (var organizationMemberItem in villager)
            // {
	           //  AdaptableLog.Info("villager = " + DebugDump(organizationMemberItem));
            // }
            //
            // var graveKeeper = OrganizationMember.Instance.Where(it => it.GradeName == "护冢");
            // foreach (var organizationMemberItem in graveKeeper)
            // {
	           //  AdaptableLog.Info("graveKeeper = " + DebugDump(organizationMemberItem));
            // }
            initMembers();
        }

        void initMembers()
        {
	        var taiwuOrg = Organization.Instance.GetItem(Organization.DefKey.Taiwu)!;
	        var allMembers = OrganizationMember.Instance;
	        // var arr = new short[]
	        // {
		       //  18,
		       //  17,
		       //  16,
		       //  15,
		       //  14,
		       //  13,
		       //  12,
		       //  11,
		       //  10,
	        // };
	        //
	        // typeof(OrganizationItem).GetField("Members")!.SetValue(taiwuOrg, arr);
	        //
	        foreach (var s in taiwuOrg.Members)
	        {
		        AdaptableLog.Info(s + " : " + allMembers[s].GradeName);
	        }
        }

        string DebugDump(object obj)
        {
            var sb = new StringBuilder();
            foreach (var fieldInfo in obj.GetType().GetFields())
            {
                sb.AppendLine($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
            }

            return sb.ToString();
            
        }

        List<OrganizationMemberItem> createItems()
        {
            List<OrganizationMemberItem> dataArray13 = new ();
			short num97 = 12;
			int num98 = 4;
			sbyte b145 = 6;
			sbyte b146 = 1;
			bool flag25 = false;
			sbyte b147 = -1;
			short num99 = -1;
			sbyte b148 = -1;
			sbyte b149 = 0;
			sbyte b150 = 0;
			sbyte b151 = -1;
			sbyte b152 = 0;
			byte b153 = 0;
			int[] array97 = new int[] { -1, -1 };
			short num100 = 0;
			sbyte b154 = 0;
			short num101 = 8000;
			sbyte b155 = 0;
			sbyte b156 = -100;
			var list = new List<short>();
			List<short> list65 = list;
			list = new List<short>();
			List<short> list66 = list;
			int[] array98 = new int[] { -1, -1 };
			bool flag26 = false;
			list = new List<short>();
			List<short> list67 = list;
			short[] array99 = new short[] { 38, 45, 52, 59 };
			PresetEquipmentItemWithProb[] array100 = new PresetEquipmentItemWithProb[]
			{
				new PresetEquipmentItemWithProb("Armor", -1, 0),
				new PresetEquipmentItemWithProb("Armor", -1, 0),
				new PresetEquipmentItemWithProb("Armor", -1, 0),
				new PresetEquipmentItemWithProb("Armor", -1, 0),
				new PresetEquipmentItemWithProb("Accessory", -1, 0),
				new PresetEquipmentItemWithProb("Accessory", -1, 0),
				new PresetEquipmentItemWithProb("Accessory", -1, 0),
				new PresetEquipmentItemWithProb("Carrier", -1, 0)
			};
			PresetEquipmentItem presetEquipmentItem13 = new PresetEquipmentItem("Clothing", -1);
			var list5 = new List<PresetInventoryItem>();
			List<PresetInventoryItem> list68 = list5;
			var list7 = new List<PresetOrgMemberCombatSkill>();
			List<PresetOrgMemberCombatSkill> list69 = list7;
			short[] array101 = new short[] { -50, -50, -50, -50, -50, -50, -50, -50 };
			int num102 = 5000;
			int num103 = 72000;
			int num104 = 9200;
			short[] array102 = new short[]
			{
				0, 0, 0, 0, 0, 90, 90, 90, 0, 0,
				90, 90, 0, 0, 0, 0
			};
			short[] array103 = new short[14];
			short[] array104 = new short[6];
			var list9 = new List<sbyte>();
			dataArray13.Add(TaiwuDomainPatch.createOrganizationMemberItem(num97, num98, b145, b146, flag25, b147, num99, b148, b149, b150, b151, b152, b153, array97, num100, b154, num101, b155, b156, list65, list66, array98, flag26, list67, array99, array100, presetEquipmentItem13, list68, list69, array101, num102, num103, num104, array102, array103, array104, list9, 3));

			return dataArray13;
        }


    }
}
