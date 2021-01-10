using System;
using System.Collections.Generic;
using System.IO;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.AI;

namespace Amnabi
{
	public static class AmnabiSocietyCore
	{
		public static bool apply_DesignatorPatch = true;
		public static bool apply_DesignatorPatch_Deconstructable = true;
		public static bool apply_PawnControlPatch = true;
		public static bool apply_PawnControlPatchDraft = true;
		public static bool apply_PawnControlPatchWork = true;
		public static bool apply_FoodPatch = true;
		public static bool apply_ApparelPatch = true;
		public static bool apply_RomancePatch = true;

		public static bool adjustRaiderAgeAndSex = true;
		public static bool enableAutoBuild = true;
		
		public static void DoWindowContents(Rect inRect)
		{
			float previous;
			Rect rect;
			var map = Current.Game?.CurrentMap;
			const float buttonWidth = 80f;
			const float buttonSpace = 4f;
			Listing_Standard list = new Listing_Standard { ColumnWidth = (inRect.width - 34f)};
			list.Begin(inRect);
			list.Gap(16f);
			list.Label("Amnabi.societysettings".Translate());
			
			list.CheckboxLabeled("Amnabi.enableAutoBuild".Translate(), ref AmnabiSocietyCore.enableAutoBuild);
			list.CheckboxLabeled("Amnabi.adjustRaiderAgeAndSex".Translate(), ref AmnabiSocietyCore.adjustRaiderAgeAndSex);

			list.CheckboxLabeled("Amnabi.apply_StructurePatch".Translate(), ref AmnabiSocietyCore.apply_DesignatorPatch);
			if(AmnabiSocietyCore.apply_DesignatorPatch)
			{
				list.CheckboxLabeled("Amnabi.apply_StructurePatch_Deconstructable".Translate(), ref AmnabiSocietyCore.apply_DesignatorPatch_Deconstructable);
			}
			
			list.CheckboxLabeled("Amnabi.apply_PawnControlPatch".Translate(), ref AmnabiSocietyCore.apply_PawnControlPatch);
			if(AmnabiSocietyCore.apply_PawnControlPatch)
			{
				list.CheckboxLabeled("Amnabi.apply_PawnControlPatchDraft".Translate(), ref AmnabiSocietyCore.apply_PawnControlPatchDraft);
				list.CheckboxLabeled("Amnabi.apply_PawnControlPatchWork".Translate(), ref AmnabiSocietyCore.apply_PawnControlPatchWork);
			}
			list.CheckboxLabeled("Amnabi.apply_FoodPatch".Translate(), ref AmnabiSocietyCore.apply_FoodPatch);
			list.CheckboxLabeled("Amnabi.apply_ApparelPatch".Translate(), ref AmnabiSocietyCore.apply_ApparelPatch);
			list.CheckboxLabeled("Amnabi.apply_RomancePatch".Translate(), ref AmnabiSocietyCore.apply_RomancePatch);

			list.End();
		}

		private static bool IsLoaded(string str)
		{
			return LoadedModManager.RunningModsListForReading.Any((ModContentPack x) => x.Name == str);
		}

		public static bool FlagsLoaded;
		public static bool SocietyLoaded;
		public static void CheckLoadedAssemblies()
		{
			FlagsLoaded = IsLoaded("Amnabi's Flags");
			SocietyLoaded = IsLoaded("Amnabi's Society Core");
			foreach(ModContentPack mcp in LoadedModManager.RunningModsListForReading)
			{
				Log.Warning(mcp.Name);
			}
		}

		public static void LoadPref()
		{
			if (!loaded)
			{
				Log.Message("Loading Amnabi's Society!", false);
				loaded = true;
				if (!File.Exists(PrefFilePath))
				{
					Log.Message(PrefFilePath + " is not found.", false);
					return;
				}
				try
				{
					Scribe.loader.InitLoading(PrefFilePath);
					try
					{
						ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.None, true);
						Scribe_Values.Look(ref apply_DesignatorPatch, "apply_DesignatorPatch", true);
						Scribe_Values.Look(ref apply_DesignatorPatch_Deconstructable, "apply_DesignatorPatch_Deconstructable", true);
						Scribe_Values.Look(ref apply_PawnControlPatch, "apply_PawnControlPatch", true);
						Scribe_Values.Look(ref apply_PawnControlPatchDraft, "apply_PawnControlPatchDraft", true);
						Scribe_Values.Look(ref apply_PawnControlPatchWork, "apply_PawnControlPatchWork", true);
						Scribe_Values.Look(ref apply_FoodPatch, "apply_FoodPatch", true);
						
						Scribe_Values.Look(ref adjustRaiderAgeAndSex, "adjustRaiderAgeAndSex", true);
						Scribe_Values.Look(ref enableAutoBuild, "enableAutoBuild", true);
						Scribe.loader.FinalizeLoading();
					}
					catch
					{
						Scribe.ForceStop();
						throw;
					}
				}
				catch (Exception ex)
				{
					Log.Error("Exception loading Race Colors: " + ex.ToString(), false);
					Scribe.ForceStop();
				}
			}
		}

		public static void SavePref()
		{
			try
			{
				SafeSaver.Save(PrefFilePath, "AmnabiSociety", delegate
				{
					ScribeMetaHeaderUtility.WriteMetaHeader();
					Scribe_Values.Look(ref apply_DesignatorPatch, "apply_DesignatorPatch", true);
					Scribe_Values.Look(ref apply_DesignatorPatch_Deconstructable, "apply_DesignatorPatch_Deconstructable", true);
					Scribe_Values.Look(ref apply_PawnControlPatch, "apply_PawnControlPatch", true);
					Scribe_Values.Look(ref apply_PawnControlPatchDraft, "apply_PawnControlPatchDraft", true);
					Scribe_Values.Look(ref apply_PawnControlPatchWork, "apply_PawnControlPatchWork", true);
					Scribe_Values.Look(ref apply_FoodPatch, "apply_FoodPatch", true);
					
					Scribe_Values.Look(ref adjustRaiderAgeAndSex, "adjustRaiderAgeAndSex", true);
					Scribe_Values.Look(ref enableAutoBuild, "enableAutoBuild", true);
				}, false);
			}
			catch (Exception ex)
			{
				Log.Error("Exception while saving world: " + ex.ToString(), false);
			}
		}

		public static string PrefFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "AmnabiSociety.xml");

		public static bool loaded = false;

		public static object field(this object obj, string str)
		{
			return obj.GetType().GetField(str, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(obj);
		}

	}
}
