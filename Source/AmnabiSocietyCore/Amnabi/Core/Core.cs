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
