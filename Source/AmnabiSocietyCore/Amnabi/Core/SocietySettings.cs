using HarmonyLib;
using System;
using UnityEngine;
using Verse;

namespace Amnabi {
	public class SocietySettingsMod : Mod
	{
		public static SocietySettings settings;
		public SocietySettingsMod(ModContentPack content) : base(content)
		{
			settings = GetSettings<SocietySettings>();
		}
		public override void DoSettingsWindowContents(Rect inRect)
		{
			settings.DoWindowContents(inRect);
		}
		public override string SettingsCategory()
		{
			return "Amnabi's Society";
		}
	}
    public class SocietySettings : ModSettings {

		public override void ExposeData()
		{
			base.ExposeData();
		}

		public void DoWindowContents(Rect inRect)
		{
			AmnabiSocietyCore.DoWindowContents(inRect);
		}
    }
}
